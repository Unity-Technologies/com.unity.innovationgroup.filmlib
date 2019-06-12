// TODO: Investigate | Internal error communicating with the shader compiler process
//CBUFFER_START(UnityPerMaterial)

// Map Inputs
TEXTURE3D(_GeometryDistanceField);
SAMPLER(sampler_GeometryDistanceField);

TEXTURE2D(_GroomDensityMap);
SAMPLER(sampler_GroomDensityMap);

// TODO: Note on comb maps
TEXTURE2D(_GroomCombMap);
SAMPLER(sampler_GroomCombMap);

TEXTURE2D(_GroomHeightMap);
SAMPLER(sampler_GroomHeightMap);

// Geometry Inputs 
float4 _GeometryParams;

// Grooming Inputs
float4 _GroomParams;

// Shading Inputs 
float4 _ShadingParams;

// Color Inputs
float3 _SpecularTint;
float3 _SecondarySpecularTint;
float3 _TransmissionTint;
float3 _RootColor;
float3 _TipColor;

// Unpack macros
#define GEOMETRY_MODE          _GeometryParams.x
#define STRAND_CURL            _GeometryParams.y
#define STRAND_OFFSET          _GeometryParams.z
#define ALPHA_CUTOFF           _GeometryParams.w

#define DENSITY                _GroomParams.x
#define HEIGHT                 _GroomParams.y
#define MIN_HEIGHT             _GroomParams.z
#define COMB_STRENGTH          _GroomParams.w

#define SPECULAR_SHIFT_0       _ShadingParams.x
#define SPECULAR_SHIFT_1       _ShadingParams.y
#define SPECULAR_SMOOTHNESS_1  _ShadingParams.z
#define TRANSMISSION_INTENSITY _ShadingParams.w

// NOTE: Currently, shells are drawn one-by-one outward -> inwward (since we render opaque, reduce overdraw)
//       For alpha blending, shells must be draw inward -> outward
float _FurShellLayer;

// TODO: Shell Instancing
/*
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
UNITY_INSTANCING_BUFFER_START(Fur)
    UNITY_DEFINE_INSTANCED_PROP(float, _FurShellLayer)
UNITY_INSTANCING_BUFFER_END(Fur)
*/

//CBUFFER_END