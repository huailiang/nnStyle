Shader "nn/inout"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
		
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}


			void test(inout float x)
			{
				x = clamp(x * 2,0,1);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				test(i.uv.x);
				return fixed4(i.uv.x,0,0,1);
			}
			ENDCG
		}

	
	}
}
