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

        void LateUpdate()
        {
            if (_renderers  == null || _renderers.Length  == 0) return;
            if (_coatLayers == null || _coatLayers.Length == 0) return;

            if (_sheet == null) _sheet = new MaterialPropertyBlock();

            foreach (var renderer in _renderers)
            {
                if (renderer == null || renderer.sharedMaterials.Length == 0) continue;
                renderer.GetPropertyBlock(_sheet);

                // TODO: For now, we render 0th layer until move to FurSystem
                for(int i = 0; i < 1; ++i)
                {
                    FurCoatLayer _ = _coatLayers[i];
                    
                    // Map inputs
                    // Can't enable shader keyword from material prop block, so we fall back to default map type if none assigned
                    if(_.distanceField != null) _sheet.SetTexture(ShaderIDs._GeometryDistanceField, _.distanceField);
                    SetMap(ShaderIDs._GroomDensityMap, _.densityMap, Texture2D.whiteTexture);
                    SetMap(ShaderIDs._GroomHeightMap,  _.heightMap,  Texture2D.whiteTexture);
                    SetMap(ShaderIDs._GroomCombMap,    _.combMap,    Texture2D.blackTexture);
                    
                    // Param Inputs
                    _sheet.SetVector(ShaderIDs._GeometryParams, new Vector4((int)_.geometryMode, _.strandCurl, _.strandOffset, 0f)); // W: Unused 
                    _sheet.SetVector(ShaderIDs._AlphaParams,    new Vector4((int)_.alphaMode, _.alphaCutoff, _.alphaFeather, 1.0f - _.selfShadow)); // W: Self Shadow
                    _sheet.SetVector(ShaderIDs._GroomParams,    new Vector4(_.density, _.height, _.minimumHeight, _.combStrength));
                    _sheet.SetVector(ShaderIDs._ShadingParams,  new Vector4(_.specularShift, _.secondarySpecularShift, _.secondarySpecularSmoothness, _.scatterAmount));

                    // Color Inputs
                    _sheet.SetColor(ShaderIDs._SpecularTint,                _.specularTint);
                    _sheet.SetColor(ShaderIDs._SecondarySpecularTint,       _.secondarySpecularTint);
                    _sheet.SetColor(ShaderIDs._TransmissionTint,            _.scatterTint);
                    _sheet.SetColor(ShaderIDs._RootColor,                   _.rootColor);
                    _sheet.SetColor(ShaderIDs._TipColor,                    _.tipColor);

                    // HDRP Lit Overrides Inputs
                    SetMap(ShaderIDs._BaseColorMap, _.albedoMap, Texture2D.whiteTexture);
                    _sheet.SetFloat(ShaderIDs._Smoothness,                  _.specularSmoothness);

                }

                renderer.SetPropertyBlock(_sheet);
            }
        }
    }
}