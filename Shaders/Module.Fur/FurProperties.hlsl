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
float4 _GeometryParams[2];

// Alpha Inputs
float4 _AlphaParams[2];

// Grooming Inputs
float4 _GroomParams[2];

// Shading Inputs 
float4 _ShadingParams[2];

// NOTE: Currently, shells are drawn one-by-one outward -> inwward (since we render opaque, reduce overdraw)
//       For alpha blending, shells must be draw inward -> outward
float4 _FurSystemParams;

#define SHELL_LAYER            _FurSystemParams.x
#define SHELL_DELTA            _FurSystemParams.y
#define SHELL_COAT_INDEX       _FurSystemParams.z

// TODO: Shell Instancing
/*
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
UNITY_INSTANCING_BUFFER_START(Fur)
    UNITY_DEFINE_INSTANCED_PROP(float4, _FurSystemParams)
UNITY_INSTANCING_BUFFER_END(Fur)
*/

// Color Inputs
float3 _SpecularTint[2];
float3 _SecondarySpecularTint[2];
float3 _TransmissionTint[2];
float3 _RootColor[2];
float3 _TipColor[2];

// Unpack macros
#define GEOMETRY_MODE          _GeometryParams        [SHELL_COAT_INDEX].x
#define STRAND_CURL            _GeometryParams        [SHELL_COAT_INDEX].y
#define STRAND_OFFSET          _GeometryParams        [SHELL_COAT_INDEX].z
        
#define ALPHA_MODE             _AlphaParams           [SHELL_COAT_INDEX].x
#define ALPHA_CUTOFF           _AlphaParams           [SHELL_COAT_INDEX].y
#define ALPHA_FEATHER          _AlphaParams           [SHELL_COAT_INDEX].z
#define SELF_SHADOW            _AlphaParams           [SHELL_COAT_INDEX].w
        
#define DENSITY                _GroomParams           [SHELL_COAT_INDEX].x
#define HEIGHT                 _GroomParams           [SHELL_COAT_INDEX].y
#define MIN_HEIGHT             _GroomParams           [SHELL_COAT_INDEX].z
#define COMB_STRENGTH          _GroomParams           [SHELL_COAT_INDEX].w
         
#define SPECULAR_SHIFT_0       _ShadingParams         [SHELL_COAT_INDEX].x
#define SPECULAR_SHIFT_1       _ShadingParams         [SHELL_COAT_INDEX].y
#define SPECULAR_SMOOTHNESS_1  _ShadingParams         [SHELL_COAT_INDEX].z
#define TRANSMISSION_INTENSITY _ShadingParams         [SHELL_COAT_INDEX].w

#define SPECULAR_TINT          _SpecularTint          [SHELL_COAT_INDEX]
#define SPECULAR_TINT_2        _SecondarySpecularTint [SHELL_COAT_INDEX]
#define TRANSMISSION_TINT      _TransmissionTint      [SHELL_COAT_INDEX]
#define ROOT_COLOR             _RootColor             [SHELL_COAT_INDEX]
#define TIP_COLOR              _TipColor              [SHELL_COAT_INDEX] 

//CBUFFER_END