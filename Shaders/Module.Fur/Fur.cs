﻿using System;
using UnityEngine.Experimental.Rendering.HDPipeline.Attributes;
using UnityEngine.Rendering;

// Fur Shading is based off HDRP Kajiya-Kay Hair shading implementation.

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    public partial class Fur : RenderPipelineMaterial
    {
        [GenerateHLSL(PackingRules.Exact)]
        public enum MaterialFeatureFlags
        {
            HairKajiyaKay = 1 << 0,
        };

        //-----------------------------------------------------------------------------
        // SurfaceData
        //-----------------------------------------------------------------------------

        // Main structure that store the user data (i.e user input of master node in material graph)
        [GenerateHLSL(PackingRules.Exact, false, true, 1500)]
        public struct SurfaceData
        {
            [SurfaceDataAttributes("MaterialFeatures")]
            public uint materialFeatures;

            [SurfaceDataAttributes("Ambient Occlusion")]
            public float ambientOcclusion;

            // Standard
            [SurfaceDataAttributes("Diffuse", false, true)]
            public Vector3 diffuseColor;
            [SurfaceDataAttributes("Specular Occlusion")]
            public float specularOcclusion;

            [SurfaceDataAttributes(new string[] {"Normal", "Normal View Space"}, true)]
            public Vector3 normalWS;

            [SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true)]
            public Vector3 geomNormalWS;

            [SurfaceDataAttributes("Smoothness")]
            public float perceptualSmoothness;

            [SurfaceDataAttributes("Transmittance")]
            public Vector3 transmittance;

            [SurfaceDataAttributes("RimTransmissionIntensity")]
            public float rimTransmissionIntensity;

            // Anisotropic
            [SurfaceDataAttributes("Hair Strand Direction", true)]
            public Vector3 hairStrandDirectionWS;

            // Kajiya kay
            [SurfaceDataAttributes("Secondary Smoothness")]
            public float secondaryPerceptualSmoothness;

            // Specular Color
            [SurfaceDataAttributes("Specular Tint", false, true)]
            public Vector3 specularTint;

            [SurfaceDataAttributes("Secondary Specular Tint", false, true)]
            public Vector3 secondarySpecularTint;

            [SurfaceDataAttributes("Specular Shift")]
            public float specularShift;

            [SurfaceDataAttributes("Secondary Specular Shift")]
            public float secondarySpecularShift;
        };

        //-----------------------------------------------------------------------------
        // BSDFData
        //-----------------------------------------------------------------------------

        [GenerateHLSL(PackingRules.Exact, false, true, 1550)]
        public struct BSDFData
        {
            public uint materialFeatures;

            public float ambientOcclusion;
            public float specularOcclusion;

            [SurfaceDataAttributes("", false, true)]
            public Vector3 diffuseColor;
            public Vector3 fresnel0;

            public Vector3 specularTint;

            [SurfaceDataAttributes(new string[] { "Normal WS", "Normal View Space" }, true)]
            public Vector3 normalWS;

            [SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true)]
            public Vector3 geomNormalWS;

            public float perceptualRoughness;

            public Vector3 transmittance;
            public float   rimTransmissionIntensity;

            // Anisotropic
            [SurfaceDataAttributes("", true)]
            public Vector3 hairStrandDirectionWS;
            public float roughnessT;
            public float roughnessB;
            public float anisotropy;

            // Kajiya kay
            public float secondaryPerceptualRoughness;
            public Vector3 secondarySpecularTint;
            public float specularExponent;
            public float secondarySpecularExponent;
            public float specularShift;
            public float secondarySpecularShift;
        };


        //-----------------------------------------------------------------------------
        // Init precomputed texture
        //-----------------------------------------------------------------------------

        public Fur() {}

        public override void Build(HDRenderPipelineAsset hdAsset)
        {
            PreIntegratedFGD.instance.Build(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
            LTCAreaLight.instance.Build();
        }

        public override void Cleanup()
        {
            PreIntegratedFGD.instance.Cleanup(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
            LTCAreaLight.instance.Cleanup();
        }

        public override void RenderInit(CommandBuffer cmd)
        {
            PreIntegratedFGD.instance.RenderInit(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse, cmd);
        }

        public override void Bind(CommandBuffer cmd)
        {
            PreIntegratedFGD.instance.Bind(cmd, PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
            LTCAreaLight.instance.Bind(cmd);
        }
    }
}