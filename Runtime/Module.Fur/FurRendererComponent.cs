using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    // Fur coat layer abstraction, to be converted to SurfaceData for Hair shading.
    // Since we allow for opaque alpha dither (and converge with TAA or super sampling), we allow for multiple coats.
    // TODO: Fur.cs GenerateHLSL?
    public enum FurGeometryMode
    {
        Baked,
        Analytic
    }

    public enum FurAlphaMode
    {
        Cutout,
        Dither
    }

    [Serializable]
    public sealed class FurCoatLayer
    {
        public FurGeometryMode geometryMode;
        
        // Baked distance field (for baked case).
        public Texture3D distanceField;

        [Range(0f, 1f)] public float strandOffset;

        [Range(0f, 1f)] public float strandCurl;

        public FurAlphaMode alphaMode;

        [Range(0f, 1f)] public float alphaCutoff;

        [Range(0f, 1f)] public float alphaFeather;

        // Density
        public Texture2D densityMap;
        [Range(0f, 1f)] public float density;

        // Height
        public Texture2D heightMap;
        [Range(0f, 1f)] public float height;
        [Range(0f, 1f)] public float minimumHeight;

        // Comb
        public Texture2D combMap;
        [Range(-2f, 2f)] public float combStrength;

        // Shading
        [Range(-1f, 1f)] public float specularShift;
        [Range(-1f, 1f)] public float secondarySpecularShift;

        // TODO: ColorUsage
        public Color specularTint;
        public Color secondarySpecularTint;

        [Range(0f, 1f)] public float specularSmoothness;
        [Range(0f, 1f)] public float secondarySpecularSmoothness;

        [Range(0f, 1f)] public float scatterAmount;
        public Color scatterTint;

        public Texture2D albedoMap;
        public Color rootColor;
        public Color tipColor; 

        [Range(0f, 1f)] public float selfShadow;
    }

    [ExecuteInEditMode]
    public sealed class FurRendererComponent : MonoBehaviour
    {
        [SerializeField] Renderer[] _renderers = null;

        public FurCoatLayer[] _coatLayers;
        
        // Layer sheet.
        // TODO: Move to FurSystem
        MaterialPropertyBlock _sheet;

        // Shader input arrays
        private Vector4[] geometryParams;
        private Vector4[] alphaParams;
        private Vector4[] groomParams;
        private Vector4[] shadingParams;

        private Color[] specularTints;
        private Color[] secondarySpecularTints;
        private Color[] scatterTints;
        private Color[] rootColors;
        private Color[] tipColors;

        static class ShaderIDs
        {
            // Map Inputs
            public static readonly int _GeometryDistanceField       = Shader.PropertyToID("_GeometryDistanceField");
            public static readonly int _GroomDensityMap             = Shader.PropertyToID("_GroomDensityMap");
            public static readonly int _GroomHeightMap              = Shader.PropertyToID("_GroomHeightMap");
            public static readonly int _GroomCombMap                = Shader.PropertyToID("_GroomCombMap");

            // Pack Groom + Shading float inputs
            public static readonly int _GeometryParams = Shader.PropertyToID("_GeometryParams");
            public static readonly int _AlphaParams    = Shader.PropertyToID("_AlphaParams");
            public static readonly int _GroomParams    = Shader.PropertyToID("_GroomParams");
            public static readonly int _ShadingParams  = Shader.PropertyToID("_ShadingParams");
            
            // Color Inputs
            public static readonly int _SpecularTint                = Shader.PropertyToID("_SpecularTint");
            public static readonly int _SecondarySpecularTint       = Shader.PropertyToID("_SecondarySpecularTint");
            public static readonly int _TransmissionTint            = Shader.PropertyToID("_TransmissionTint");
            public static readonly int _RootColor                   = Shader.PropertyToID("_RootColor");
            public static readonly int _TipColor                    = Shader.PropertyToID("_TipColor");

            // HDRP Lit Input Overrides
            public static readonly int _BaseColorMap                = Shader.PropertyToID("_BaseColorMap");
            public static readonly int _Smoothness                  = Shader.PropertyToID("_Smoothness");
        }

        void SetMap(int shaderID, Texture map, Texture defaultMap)
        {
            if(map != null)
            {
                _sheet.SetTexture(shaderID, map);
            }
            else
            {
                _sheet.SetTexture(shaderID, defaultMap);
            }
        }

        Vector4 GetLayerGeometryParams(FurCoatLayer layer)
        {
            return new Vector4((int)layer.geometryMode, layer.strandCurl, layer.strandOffset, 0f);
        }

        Vector4 GetLayerAlphaParams(FurCoatLayer layer)
        {
            return new Vector4((int)layer.alphaMode, layer.alphaCutoff, layer.alphaFeather, 1.0f - layer.selfShadow);
        }

        Vector4 GetLayerGroomParams(FurCoatLayer layer)
        {
            return new Vector4(layer.density, layer.height, layer.minimumHeight, layer.combStrength);
        }

        Vector4 GetLayerShadingParams(FurCoatLayer layer)
        {
            return new Vector4(layer.specularShift, layer.secondarySpecularShift, layer.secondarySpecularSmoothness, layer.scatterAmount);
        }

        void LateUpdate()
        {
            if (_renderers  == null || _renderers.Length  == 0) return;
            if (_coatLayers == null || _coatLayers.Length == 0) return;

            if (_sheet == null) _sheet = new MaterialPropertyBlock();

            foreach (var renderer in _renderers)
            {
                if (renderer == null || renderer.sharedMaterials.Length == 0) continue;
                renderer.GetPropertyBlock(_sheet);

                // TODO: For now, we render 0th and 1st layer until move to FurSystem
                FurCoatLayer u = _coatLayers[0];
                FurCoatLayer o = _coatLayers[1];
                
                // Map inputs
                // Can't enable shader keyword from material prop block, so we fall back to default map type if none assigned
                if(u.distanceField != null) _sheet.SetTexture(ShaderIDs._GeometryDistanceField, u.distanceField);
                SetMap(ShaderIDs._GroomDensityMap, u.densityMap, Texture2D.whiteTexture);
                SetMap(ShaderIDs._GroomHeightMap,  u.heightMap,  Texture2D.whiteTexture);
                SetMap(ShaderIDs._GroomCombMap,    u.combMap,    Texture2D.blackTexture);
                
                // Param Inputs
                _sheet.SetVectorArray(ShaderIDs._GeometryParams, new Vector4[2] { GetLayerGeometryParams(u), GetLayerGeometryParams(o) });
                _sheet.SetVectorArray(ShaderIDs._AlphaParams,    new Vector4[2] { GetLayerAlphaParams(u),    GetLayerAlphaParams(o)    });
                _sheet.SetVectorArray(ShaderIDs._GroomParams,    new Vector4[2] { GetLayerGroomParams(u),    GetLayerGroomParams(o)    });
                _sheet.SetVectorArray(ShaderIDs._ShadingParams,  new Vector4[2] { GetLayerShadingParams(u),  GetLayerShadingParams(o)  });

                // Color Inputs
                _sheet.SetVectorArray(ShaderIDs._SpecularTint,                new Vector4[2] { u.specularTint,          o.specularTint          });
                _sheet.SetVectorArray(ShaderIDs._SecondarySpecularTint,       new Vector4[2] { u.secondarySpecularTint, o.secondarySpecularTint });
                _sheet.SetVectorArray(ShaderIDs._TransmissionTint,            new Vector4[2] { u.scatterTint,           o.scatterTint           });
                _sheet.SetVectorArray(ShaderIDs._RootColor,                   new Vector4[2] { u.rootColor,             o.rootColor             });
                _sheet.SetVectorArray(ShaderIDs._TipColor,                    new Vector4[2] { u.tipColor,              o.tipColor              });

                // HDRP Lit Overrides Inputs
                SetMap(ShaderIDs._BaseColorMap, u.albedoMap, Texture2D.whiteTexture);
                _sheet.SetFloat(ShaderIDs._Smoothness,                  u.specularSmoothness);

                renderer.SetPropertyBlock(_sheet);
            }
        }
    }
}