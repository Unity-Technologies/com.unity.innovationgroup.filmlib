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

    [Serializable]
    public sealed class FurCoatLayer
    {
        public FurGeometryMode mode;
        
        // Baked distance field (for baked case).
        public Texture3D distanceField;

        [Range(0f, 1f)] public float strandOffset;

        [Range(0f, 1f)] public float strandCurl;

        [Range(0f, 1f)] public float alphaCutoff;

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

        [Range(0f, 1f)] public float secondarySpecularSmoothness;

        [Range(0f, 1f)] public float scatterAmount;
        public Color scatterTint;

        public Color rootColor;
        public Color tipColor; 
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
            // TODO: Consolidate to vector inputs
            public static readonly int _GeometryDistanceField       = Shader.PropertyToID("_GeometryDistanceField");
            public static readonly int _AlphaRemap                  = Shader.PropertyToID("_AlphaRemap");
            public static readonly int _GroomDensityMap             = Shader.PropertyToID("_GroomDensityMap");
            public static readonly int _GroomHeightMap              = Shader.PropertyToID("_GroomHeightMap");
            public static readonly int _GroomCombMap                = Shader.PropertyToID("_GroomCombMap");
            public static readonly int _SpecularTint                = Shader.PropertyToID("_SpecularTint");
            public static readonly int _SecondarySpecularTint       = Shader.PropertyToID("_SecondarySpecularTint");
            public static readonly int _TransmissionTint            = Shader.PropertyToID("_TransmissionTint");
            public static readonly int _RootColor                   = Shader.PropertyToID("_RootColor");
            public static readonly int _TipColor                    = Shader.PropertyToID("_TipColor");

            // Pack Groom + Shading float inputs
            public static readonly int _GeometryParams = Shader.PropertyToID("_GeometryParams");
            public static readonly int _GroomParams    = Shader.PropertyToID("_GroomParams");
            public static readonly int _ShadingParams  = Shader.PropertyToID("_ShadingParams");
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
                    if(_.distanceField != null) _sheet.SetTexture(ShaderIDs._GeometryDistanceField, _.distanceField);
                    if(_.densityMap    != null) _sheet.SetTexture(ShaderIDs._GroomDensityMap,       _.densityMap);
                    if(_.heightMap     != null) _sheet.SetTexture(ShaderIDs._GroomHeightMap,        _.heightMap);
                    if(_.combMap       != null) _sheet.SetTexture(ShaderIDs._GroomCombMap,          _.combMap);

                    _sheet.SetVector(ShaderIDs._GeometryParams, new Vector4((int)_.mode, _.strandCurl, _.strandOffset, _.alphaCutoff));
                    _sheet.SetVector(ShaderIDs._GroomParams,    new Vector4(_.density, _.height, _.minimumHeight, _.combStrength));
                    _sheet.SetVector(ShaderIDs._ShadingParams,  new Vector4(_.specularShift, _.secondarySpecularShift, _.secondarySpecularSmoothness, _.scatterAmount));

                    _sheet.SetColor(ShaderIDs._SpecularTint,                _.specularTint);
                    _sheet.SetColor(ShaderIDs._SecondarySpecularTint,       _.secondarySpecularTint);
                    _sheet.SetColor(ShaderIDs._TransmissionTint,            _.scatterTint);
                    _sheet.SetColor(ShaderIDs._RootColor,                   _.rootColor);
                    _sheet.SetColor(ShaderIDs._TipColor,                    _.tipColor);

                }

                renderer.SetPropertyBlock(_sheet);
            }
        }
    }
}