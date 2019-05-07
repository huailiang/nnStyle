Shader "nn/fade"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DistortFactor("DistortFactor",Range(0, 0.5))=0
		_DistortCenter("DistortCenter", Vector)=(0.5,0.5,0,0)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define SAMPLE_COUNT 6
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _DistortFactor;
			float4 _DistortCenter;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 uv01 : TEXCOORD1;	
				float4 uv23 : TEXCOORD2;	
				float4 uv45 : TEXCOORD3;
			};

			v2f vert (appdata v)
			{
				_DistortFactor = _DistortFactor + 0.05f;
				_DistortFactor = clamp(_DistortFactor, 0.0f, 0.5f);
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				float scale = 32 * (0.5f - _DistortFactor);
				o.uv01 = v.uv.xyxy +  float4(0, 1, 0, -1) *  0.02f * scale;
				o.uv23 = v.uv.xyxy +  float4(1, 0, -1, 0) *  0.02f * scale;
				o.uv45 = v.uv.xyxy +  float4(1, 1, -1, -1) * 0.02f * scale;
				return o;
			}
			
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 dir = i.uv - _DistortCenter.xy;
				float2 offset = (float2(0.5f, 0.5f) - _DistortFactor) * normalize(dir) * (1 - length(dir));
				float2 uv = i.uv + offset;
				fixed4 col = 0.4f * tex2D(_MainTex, uv);
				uv = i.uv01.xy + offset;
				col += 0.1f * tex2D(_MainTex, uv);
				uv = i.uv01.zw + offset;
				col += 0.1f * tex2D(_MainTex, uv);
				uv = i.uv23.xy + offset;
				col += 0.10f * tex2D(_MainTex, uv);
				uv = i.uv23.zw + offset;
				col += 0.10f * tex2D(_MainTex, uv);
				uv = i.uv45.xy + offset;
				col += 0.1f * tex2D(_MainTex, uv);
				uv = i.uv45.zw + offset;
				col += 0.1f * tex2D(_MainTex, uv);
				return col;
			}
			ENDCG
		}
	}
}
