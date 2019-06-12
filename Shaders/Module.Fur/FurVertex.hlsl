float3 Dynamics(AttributesMesh input)
{
    // TODO
    return 0;
}

AttributesMesh ApplyMeshModification(AttributesMesh input)
{
#if defined(ATTRIBUTES_NEED_NORMAL) && defined(ATTRIBUTES_NEED_TANGENT) && defined(ATTRIBUTES_NEED_TEXCOORD0) && defined(ATTRIBUTES_NEED_COLOR) 

    //Construct TBN.
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
    float4 tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
    real3x3 worldToTangent = CreateWorldToTangent(normalWS, tangentWS.xyz, tangentWS.w);

    float4 combSample = SAMPLE_TEXTURE2D_LOD(_GroomCombMap, sampler_GroomCombMap, input.uv0, 0.0);
    float3 combDirectionTS = UnpackNormalmapRGorAG(combSample, COMB_STRENGTH);
    combDirectionTS = float3(-combDirectionTS.x, combDirectionTS.yz);
    float3 combDirectionWS = normalize(TransformTangentToWorld(combDirectionTS, worldToTangent));
    //combDirectionWS = -Orthonormalize(combDirectionWS, worldToTangent[2]);

    float h = SAMPLE_TEXTURE2D_LOD(_GroomHeightMap, sampler_GroomHeightMap, input.uv0.xy, 2);
    h = lerp(0.3, 1.0, smoothstep(0.2, 0.3 , h));

    // Final position is derived from a quadratic blending function.
    {
        //We perform shell offsets in world space.
        float3 positionWS = TransformObjectToWorld(input.positionOS);

        //Set up the quadratic blend.
        float3 P0, P1, PC, D;
        float  U;

        //Blend inputs
        D  = HEIGHT * 0.1 * (normalWS + 0);
        P0 = positionWS;
        P1 = P0 +  D;
        PC = P0 + (D * 0.5);
        U  = _FurShellLayer;

        // Final position is derived from a quadratic blending function.
        positionWS = (P0 * pow(1 - U, 2.0)) + (P1 * pow(U, 2.0)) + (PC * 2 * U * (1 - U));

        // We store strand tangents in vertex color.
        U = _FurShellLayer + (1.0 / 128.0); // TODO: Send delta.
        float3 nextShellPositionWS = (P0 * pow(1 - U, 2.0)) + (P1 * pow(U, 2.0)) + (PC * 2 * U * (1 - U));

        //Apply Wind
        ApplyWindDisplacement(positionWS,          normalWS, P0, 0.2, 0.9, 0.3, 0.3, 1, 0.05 * h * _FurShellLayer, _Time);
        ApplyWindDisplacement(nextShellPositionWS, normalWS, P0, 0.2, 0.9, 0.3, 0.3, 1, 0.05 * h * _FurShellLayer, _Time);

        // TODO: Note on tangent calculation.
        input.color.xyz = normalize(nextShellPositionWS - positionWS);
        
        //Transfer back to object space.
        input.positionOS = TransformWorldToObject(positionWS);
    }


#endif
    return input;
}