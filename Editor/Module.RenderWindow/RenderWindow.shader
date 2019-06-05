Shader "Hidden/RenderWindow"
{
	SubShader
	{
		Tags{ "RenderPipeline" = "HDRenderPipeline" }
		Pass
		{
			Cull   Off
			ZTest  Always
			ZWrite Off
			Blend  Off

			HLSLPROGRAM
			#pragma target 4.5
			#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

			#pragma vertex   Vert
			#pragma fragment Frag

			//-------------------------------------------------------------------------------------
			// Include
			//-------------------------------------------------------------------------------------

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#define USE_LEGACY_UNITY_MATRIX_VARIABLES
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			//-------------------------------------------------------------------------------------
			// Inputs & outputs
			//-------------------------------------------------------------------------------------

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);

			float _CompositeProgress;

			//-------------------------------------------------------------------------------------
			// Implementation
			//-------------------------------------------------------------------------------------

			struct Attributes
			{
				float3 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct Varyings
			{
				float4 vertex   : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			Varyings Vert(Attributes input)
			{
				Varyings output;

				//TODO: For multipass approach must modify the projection.
				output.vertex = TransformWorldToHClip(input.vertex);
				output.texcoord = input.texcoord.xy;
				
				return output;
			}

            void DisplayLoadingBar(inout float3 s, float2 uv)
            {
				//TODO
            }

			float4 Frag(Varyings input) : SV_Target
			{
				float2 uv = input.texcoord;

                float3 s = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;
                
                DisplayLoadingBar(s, uv);

				return float4(LinearToSRGB(s), 1);
			}
			ENDHLSL
		}
	}
	Fallback Off
}
