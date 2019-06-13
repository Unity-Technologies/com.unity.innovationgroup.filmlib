float3 Dynamics(AttributesMesh input)
{
    // NOTE: Dynamics are not yet implemented for the fur shells, but they can be brought to life here.
    
    // For example, they could be plugged into HDRP Wind System (deprecated) for basic response to wind environment.
    //ApplyWindDisplacement(positionWS,          normalWS, P0, 0.2, 0.9, 0.3, 0.3, 1, 0.05 * h * _FurShellLayer, _Time);

    return 0;
}

// TODO: Note on comb map authoring.
float3 GetCombTS(float2 texcoord)
{
    float4 combSample = SAMPLE_TEXTURE2D_LOD(_GroomCombMap, sampler_GroomCombMap, texcoord, 0.0);
    return UnpackNormalmapRGorAG(combSample, COMB_STRENGTH);
}

float GetHeight(float2 texcoord)
{
    float height = SAMPLE_TEXTURE2D_LOD(_GroomHeightMap, sampler_GroomHeightMap, texcoord, 2);
    return HEIGHT * height;//lerp(0.3, 1.0, smoothstep(0.2, 0.3, height));
}

AttributesMesh ApplyMeshModification(AttributesMesh input)
{
#if defined(ATTRIBUTES_NEED_NORMAL) && defined(ATTRIBUTES_NEED_TANGENT) && defined(ATTRIBUTES_NEED_TEXCOORD0) && defined(ATTRIBUTES_NEED_COLOR) 

    // Construct TBN.
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
    float4 tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
    real3x3 worldToTangent = CreateWorldToTangent(normalWS, tangentWS.xyz, tangentWS.w);

    // Comb direction
    float3 combDirectionTS = GetCombTS(input.uv0.xy);
    float3 combDirectionWS = normalize(TransformTangentToWorld(combDirectionTS, worldToTangent));

    // Height
    float height = GetHeight(input.uv0.xy);

    // Final position is derived from a quadratic blending function.
    {
        //We perform shell offsets in world space.
        float3 positionWS = TransformObjectToWorld(input.positionOS);

        //Set up the quadratic blend.
        float3 P0, P1, PC, D;
        float  U;

        //Blend inputs
        D  = height * 0.1 * (normalWS + combDirectionWS);
        P0 = positionWS;
        P1 = P0 +  D;
        PC = P0 + (D * 0.5);
        U  = _FurShellLayer;

        // Final position is derived from a quadratic blending function.
        positionWS = (P0 * pow(1 - U, 2.0)) + (P1 * pow(U, 2.0)) + (PC * 2 * U * (1 - U));

        // We store strand tangents in vertex color.
        U = _FurShellLayer + (1.0 / 32.0); // TODO: Send delta.
        float3 nextShellPositionWS = (P0 * pow(1 - U, 2.0)) + (P1 * pow(U, 2.0)) + (PC * 2 * U * (1 - U));

        // TODO: Note on tangent calculation.
        input.color.xyz = normalize(nextShellPositionWS - positionWS);
        
        //Transfer back to object space.
        input.positionOS = TransformWorldToObject(positionWS);
    }


#endif
    return input;
}