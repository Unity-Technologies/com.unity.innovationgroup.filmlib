// TODO: Investigate | Internal error communicating with the shader compiler process
//CBUFFER_START(UnityPerMaterial)

// TODO: Organize undercoats / overcoats etc. into layer input

// Map Inputs
TEXTURE3D(_FurGeometrySDF);
SAMPLER(sampler_FurGeometrySDF);

TEXTURE2D(_FurGroomMap);
SAMPLER(sampler_FurGroomMap);

TEXTURE2D(_FurHeightMap);
SAMPLER(sampler_FurHeightMap);

// Alpha 
float _FurAlphaFalloff;
float _FurAlphaFalloffBias;

float2 _FurOvercoatAlphaRemap;
float  _FurOvercoatSilhouette;

// Grooming Inputs
float _FurDensity;
float _FurOvercoatDensity;

float _FurShellHeight;
float _FurShellMinimumHeight;

float _FurCombStrength;
float _FurOvercoatCombStrength;

// Shading Inputs (Currently shared by overcoat + undercoat)
float  _SpecularShift;
float3 _SpecularTint;

float  _SecondaryPerceptualSmoothness;
float  _SecondarySpecularShift;
float3 _SecondarySpecularTint;

float  _FurScatter;
float3 _FurScatterTint;

float _FurSelfOcclusionMultiplier;
float _FurOvercoatSelfOcclusionMultiplier;

// Currently shells are drawn one-by-one outward -> inwward (since we render opaque, reduce overdraw)
// For alpha blending, shells must be draw inward -> outward
float _FurShellLayer;

// TODO: Shell Instancing
/*
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
UNITY_INSTANCING_BUFFER_START(Fur)
    UNITY_DEFINE_INSTANCED_PROP(float, _FurShellLayer)
UNITY_INSTANCING_BUFFER_END(Fur)
*/

//CBUFFER_END