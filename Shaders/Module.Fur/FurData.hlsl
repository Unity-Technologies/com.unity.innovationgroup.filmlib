//-------------------------------------------------------------------------------------
// Fill SurfaceData/Builtin data function
//-------------------------------------------------------------------------------------
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Sampling/SampleUVMapping.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"

// TODO: Find dither pattern that converges with TAA (blue noise?)
// TODO: Note on dither + super sampling.
float _SuperSampleSubFrame;
void DitherTransparency(float alpha, float2 position)
{
    float sampleJitterAngle = InterleavedGradientNoise(position, _SuperSampleSubFrame) * 2.0 * PI;
    float2 seed = position + sampleJitterAngle;
    float nrnd  = frac(sin(dot(seed.xy, float2(12.9898, 78.233))) * 43758.5453) * saturate(pow(1 - alpha, 0.25));
    clip(alpha - nrnd);
}

// TODO: More exotic distance field to capture curling, etc.
float SignedDistanceField_Cone(float q, float z, float2 c)
{
   return dot(c, float2(q, z));
}

float2 hash2( float2  p, float strength) { p = float2( dot(p,float2(127.1,311.7)), dot(p,float2(269.5,183.3)) ); return frac(sin(p)*43758.5453) * strength; }

float VoronoiDistance( in float2 x, float w, float strength)
{
    float2 n = floor( x );
    float2 f = frac( x );

    float m = 8.0;
    for( int j = -2; j <= 2; j++ )
    {
        for( int i = -2; i <= 2; i++ )
        {
            float2 g = float2( float(i), float(j) );
            float2 o = hash2( n + g, strength);
        
            // Distance to cell        
            float d = length(g - f + o);
            float h = smoothstep( 0.0, 1.0, 0.5 + 0.5 * (m - d) / w );
        
            m = lerp( m, d, h ) - h * (1.0 - h) * w / (1.0 + 3.0 * w); // Distance
        }
    }
    
    return m;
}

// NOTE: Force analytic always on until we can use keywords.
#if 1 // TODO: Directive for analytic / baked.
    #define SAMPLE_FUR SampleFur_Analytic
#else
    #define SAMPLE_FUR SampleFur_Baked
#endif

float SampleFur_Analytic(in float3 p)
{
   // TODO: Parametrize the cone geometric description
   float2 c = normalize(float2(10, 2));

   // Break up the distance field repetition pattern.
   float sd = VoronoiDistance(p.xy, 1.0, STRAND_OFFSET);

   // Submit to SDF
   return 1.0 - SignedDistanceField_Cone(sd, p.z, c);
}

float SampleFur_Baked(in float3 p)
{
    // TODO
    return 0;
}

// TODO : Move off forward to intermediate pass.
float3 NormalFromDepth(uint2 positionCS)
{
    uint2 pixCoord = (uint2)positionCS.xy >> 0;
    pixCoord += 1;
                    
    float depth     = LOAD_TEXTURE2D(_DepthPyramidTexture, pixCoord).r;
    float depth_ddx = LOAD_TEXTURE2D(_DepthPyramidTexture, pixCoord + int2(1,0)).r;
    float depth_ddy = LOAD_TEXTURE2D(_DepthPyramidTexture, pixCoord + int2(0,1)).r;

    float2 uv = (pixCoord * _ScreenSize.zw);
    float2 uv_ox = uv + float2(1.0 / _ScreenSize.x, 0.0);
    float2 uv_oy = uv + float2(0.0, 1.0 / _ScreenSize.y);

    float2 ndc = (uv - 0.5) * 2.0;
    float2 ndc_ddx = (uv_ox - 0.5) * 2.0;
    float2 ndc_ddy = (uv_oy - 0.5) * 2.0;

    float3 wp     = ComputeWorldSpacePosition(ndc,     depth,     UNITY_MATRIX_I_VP);
    float3 wp_ddx = ComputeWorldSpacePosition(ndc_ddx, depth_ddx, UNITY_MATRIX_I_VP);
    float3 wp_ddy = ComputeWorldSpacePosition(ndc_ddy, depth_ddy, UNITY_MATRIX_I_VP);

    float3 vec_ddx = (wp_ddx - wp);
    float3 vec_ddy = (wp_ddy - wp);
    float3 N = cross(vec_ddx, vec_ddy);
    return -normalize(N);
}

float SelfOcclusionTerm()
{
    float shadowMin = SELF_SHADOW;
    return SHELL_LAYER * (1 - shadowMin) + shadowMin;
}

float3 DiffuseColor(float2 uv)
{
    float3 diffuseColor = SAMPLE_TEXTURE2D(_BaseColorMap, sampler_BaseColorMap, uv);
    diffuseColor *= lerp(ROOT_COLOR, TIP_COLOR, SHELL_LAYER);
    return diffuseColor;
}

float2 DensityUV(float2 texcoord)
{
    float densitySample = SAMPLE_TEXTURE2D(_GroomDensityMap, sampler_GroomDensityMap, texcoord);
    return 500.0 * (densitySample * DENSITY) * texcoord;
}

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"

void GetSurfaceAndBuiltinData(FragInputs input, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
{
#ifdef _DOUBLESIDED_ON
    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
#else
    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
#endif

    ApplyDoubleSidedFlipOrMirror(input, doubleSidedConstants);

    float2 texcoord = input.texCoord0.xy;

    // Sample the fur distance field (analytic or baked).
    // TODO: Note on distance fields.
    float3 distanceFieldInput = float3(DensityUV(texcoord), SHELL_LAYER);
    float alpha = SAMPLE_FUR(distanceFieldInput);

#if SHADERPASS == SHADERPASS_DEPTH_ONLY
    // TODO: Note on alpha modes.
    // NOTE: Currently force alpha dither always on until we can use keywords
    #if 1
        // We smoothstep + dither to achieve soft falloffs.
        // Dither can be combined with a convergence engine (TAA, Super Sample, etc. for OIT)
        DitherTransparency(smoothstep(ALPHA_CUTOFF, min(ALPHA_CUTOFF + ALPHA_FEATHER, 1), alpha), posInput.positionSS);
    #else
        clip(step(ALPHA_CUTOFF, alpha) - 0.0001);
    #endif
#endif

    // NOTE: Currently we derive normals from depth, in future investigate deriving combed normal from
    //       distance field gradient.
    float3 N = NormalFromDepth(posInput.positionSS);

    // NOTE: We calculate tangents on vertex stage, derived from the delta between shells.
    float3 T = Orthonormalize(input.color.xyz, N);

    // Init Fur Coat Layer Data
    surfaceData.materialFeatures              = MATERIALFEATUREFLAGS_HAIR_KAJIYA_KAY;
    surfaceData.diffuseColor                  = DiffuseColor(texcoord); 
    surfaceData.normalWS                      = N;
    surfaceData.geomNormalWS                  = input.worldToTangent[2];
    surfaceData.transmittance                 = TRANSMISSION_TINT;
    surfaceData.rimTransmissionIntensity      = TRANSMISSION_INTENSITY;
    surfaceData.hairStrandDirectionWS         = T; 
    surfaceData.perceptualSmoothness          = _Smoothness;
    surfaceData.secondaryPerceptualSmoothness = SPECULAR_SMOOTHNESS_1;
    surfaceData.specularTint                  = SPECULAR_TINT;
    surfaceData.secondarySpecularTint         = SPECULAR_TINT_2;
    surfaceData.specularShift                 = SPECULAR_SHIFT_0;
    surfaceData.secondarySpecularShift        = SPECULAR_SHIFT_1;
    surfaceData.ambientOcclusion              = SelfOcclusionTerm(); 
    surfaceData.specularOcclusion             = GetSpecularOcclusionFromAmbientOcclusion(ClampNdotV(dot(surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));

    // Builtin Data
    // For back lighting we use the oposite vertex normal 
    InitBuiltinData(alpha, surfaceData.normalWS, -N, input.positionRWS, input.texCoord1, input.texCoord2, builtinData);
    PostInitBuiltinData(V, posInput, surfaceData, builtinData);
}
