//-------------------------------------------------------------------------------------
// Fill SurfaceData/Builtin data function
//-------------------------------------------------------------------------------------
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Sampling/SampleUVMapping.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"

#define DENSITY 200

float _SuperSampleSubFrame;
void DitherTransparency(float alpha, float2 position)
{
    float sampleJitterAngle = InterleavedGradientNoise(position, _SuperSampleSubFrame) * 2.0 * PI;
    float2 seed = position + sampleJitterAngle;
    float nrnd  = frac(sin(dot(seed.xy, float2(12.9898, 78.233))) * 43758.5453) * saturate(pow(1 - alpha, 0.25));
    clip(alpha - nrnd);
}

float sdCone( float3 p, float2 c )
{
   // c must be normalized
   float q = length(p.xy);
   return dot(c, float2(q, p.z));
}

float FurPatch(in float3 p)
{
   float3 r = float3(0.4, 0.4, -5);
   float3 q = fmod(p, r) - 0.5 * r;

   return 1.0 - sdCone(q, normalize(float2(10, 2)));
}

float3 NormalFromDepth(uint2 positionCS)
{
    uint2 pixCoord = (uint2)positionCS.xy >> 0;
    //pixCoord.y = _ScreenSize.y - pixCoord.y;
                    
    pixCoord += 1;
                    
    float depth = LOAD_TEXTURE2D(_DepthPyramidTexture, pixCoord).r;
    float depth_ddx = LOAD_TEXTURE2D(_DepthPyramidTexture, pixCoord + int2(1,0)).r;
    float depth_ddy = LOAD_TEXTURE2D(_DepthPyramidTexture, pixCoord + int2(0,1)).r;

    float2 uv = (pixCoord * _ScreenSize.zw);
    float2 uv_ox = uv + float2(1.f/_ScreenSize.x,0);
    float2 uv_oy = uv + float2(0,1.f/_ScreenSize.y);
    float2 ndc = (uv - 0.5) * 2.f;
    float2 ndc_ddx = (uv_ox - 0.5) * 2.f;
    float2 ndc_ddy = (uv_oy - 0.5) * 2.f;

    float3 wp     = ComputeWorldSpacePosition(ndc, depth, UNITY_MATRIX_I_VP);
    float3 wp_ddx = ComputeWorldSpacePosition(ndc_ddx, depth_ddx, UNITY_MATRIX_I_VP);
    float3 wp_ddy = ComputeWorldSpacePosition(ndc_ddy, depth_ddy, UNITY_MATRIX_I_VP);

    float3 vec_ddx = (wp_ddx - wp);
    float3 vec_ddy = (wp_ddy - wp);
    float3 N = cross(vec_ddx, vec_ddy);
    return -normalize(N);
}

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"

void GetSurfaceAndBuiltinData(FragInputs input, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
{

    float alpha = 1;

// Switch between Analytic and Prebaked SDF.
    //float3 vertexNormal = input.worldToTangent[2].xyz;
    //input.color.xyz = Orthonormalize(input.color.xyz, vertexNormal);
    //input.worldToTangent = BuildWorldToTangent(float4(input.color.xyz, 1.0), vertexNormal);
#if 1
    //FurShellGeometryDefinition(input.texCoord0.xy);

    //Compute N in R3
    float3 P = FurPatch(float3(DENSITY * (input.texCoord0.xy + 0.5), _FurShellLayer * 2.0));
    float3 N = normalize
               (
                   float3
                   (
                       FurPatch( float3(P.x + 0.001, P.y, P.z) ) - FurPatch( float3(P.x - 0.001, P.y, P.z) ),
                       FurPatch( float3(P.x, P.y + 0.001, P.z) ) - FurPatch( float3(P.x, P.y - 0.001, P.z) ),
                       FurPatch( float3(P.x, P.y, P.z + 0.001) ) - FurPatch( float3(P.x, P.y, P.z - 0.001) )
                   )
               );

    //Alpha.
    //alpha = 1.0 - smoothstep(0.1, 1.0, _FurShellLayer);
    //alpha *= lerp(0.5, 1.0, P);
    //DitherTransparency(alpha, posInput.positionSS);

    clip(P - 0.3);

    //Negate the volume gradient.
    N = TransformTangentToWorld(-N, input.worldToTangent);
    real3 furGeometryGradient = SurfaceGradientFromPerturbedNormal(input.worldToTangent[2].xyz, N);
    N = SurfaceGradientResolveNormal(input.worldToTangent[2].xyz, furGeometryGradient * 2);

#else
    float4 BakedSDF = SAMPLE_TEXTURE3D(_FurGeometrySDF, sampler_FurGeometrySDF, float3(input.texCoord0.x * 10, min(_FurShellLayer, 1.0), input.texCoord0.y  * 10));
    //float4 BakedSDF = LOAD_TEXTURE3D(_FurGeometrySDF, float3(input.texCoord0.x * 1, _FurShellLayer, input.texCoord0.y  * 1));

    //alpha  = 1.0 - _FurShellLayer;
    //alpha *= saturate(0.7 - BakedSDF.a * 150);
    alpha = smoothstep(0.995, 1.0, 1.0 - BakedSDF.a); //pow(1.0 - BakedSDF.a, 1000);
    clip(alpha - 0.4);

    float3 N = BakedSDF.xyz; //Gradient is precomputed.
    //N = normalize(TransformTangentToWorld(-N.xyz, input.worldToTangent));

    //real3 furGeometryGradient = SurfaceGradientFromPerturbedNormal(input.worldToTangent[2].xyz, N);
    //N = SurfaceGradientResolveNormal(input.worldToTangent[2].xyz, furGeometryGradient);
#endif

    N = NormalFromDepth(posInput.positionSS);
    float3 T = normalize(input.color.xyz); //Orthonormalize(input.color.xyz, N);

    // Init Fur Data
    surfaceData.materialFeatures = MATERIALFEATUREFLAGS_HAIR_KAJIYA_KAY;
    surfaceData.ambientOcclusion = 1;
    surfaceData.diffuseColor = pow(_FurShellLayer, 5.0) * _BaseColor; 
    surfaceData.specularOcclusion = 1;
    surfaceData.normalWS = N;
    surfaceData.geomNormalWS = input.worldToTangent[2];
    surfaceData.perceptualSmoothness = _Smoothness;
    surfaceData.transmittance = float3(1, 1, 1);
    surfaceData.rimTransmissionIntensity = 0.5;
    surfaceData.hairStrandDirectionWS = T; 
    surfaceData.secondaryPerceptualSmoothness = _SecondaryPerceptualSmoothness;
    surfaceData.specularTint = float3(1, 1, 1);
    surfaceData.secondarySpecularTint = float3(1, 1, 1);
    surfaceData.specularShift = _SpecularShift;
    surfaceData.secondarySpecularShift = _SecondarySpecularShift;

    // Builtin Data
    // For back lighting we use the oposite vertex normal 
    InitBuiltinData(alpha, surfaceData.normalWS, -N, input.positionRWS, input.texCoord1, input.texCoord2, builtinData);
}
