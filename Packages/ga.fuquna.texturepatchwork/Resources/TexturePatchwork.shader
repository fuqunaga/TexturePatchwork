Shader "TexturePatchwork"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		[Enum(UnityEngine.Rendering.BlendMode)]
		_SrcFactor("Src Factor", Float) = 1     // One

		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstFactor("Dst Factor", Float) = 0    // Zero
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Blend [_SrcFactor][_DstFactor]

		Pass
		{
			Name "RectArrangement"
			
			HLSLPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			
			sampler2D _MainTex;
			
			struct Attributes{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};

			struct Varyings
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			Varyings vert(const Attributes input )
			{
				Varyings output;

				output.pos = TransformObjectToHClip(input.vertex);
				output.uv = input.texcoord;
				return output;
			}
			
			half4 frag (const Varyings input) : SV_Target
			{
				return tex2D(_MainTex, input.uv);
			}
			
			ENDHLSL
		}
	}
}
