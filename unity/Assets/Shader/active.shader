Shader "nn/active"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			CGPROGRAM

			#pragma shader_feature ACTIVE_SHADER_RELU 
			#pragma shader_feature ACTIVE_SHADER_LRELU
			#pragma shader_feature ACTIVE_SHADER_TANH
			#pragma shader_feature ACTIVE_SHADER_SIGMOD

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "libActive.cginc"

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
			
			#define NUM 32
			int arr[NUM][NUM];
			int active_func = 0;

			#define DO_ACTIVE_FUNC(scope,func,eplison) \
				[unroll(NUM)] \
				for (int i = 0; i < NUM; ++i) { \
					for (int j = 0; j < NUM; ++j) { \
						float tx = scope*(i*scope-NUM)/(float)NUM;	\
						float ty = scope*(j*scope-NUM)/(float)NUM;	\
						arr[i][j] = abs(func(tx) - ty) < eplison; \
					} \
				}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float x = i.uv.x;
				float y = i.uv.y;
				float coord = (abs(x-0.5)<1e-3 || abs(y-0.5)<1e-3) ? 1 : 0.1; //coord

				#if ACTIVE_SHADER_RELU
				DO_ACTIVE_FUNC(2,relu,1e-4)
				#elif ACTIVE_SHADER_LRELU
				DO_ACTIVE_FUNC(2,lrelu,1e-2)
				#elif ACTIVE_SHADER_TANH
				DO_ACTIVE_FUNC(2,tanh,1e-1)
				#else
				DO_ACTIVE_FUNC(2,sigmod,1e-2)
				#endif

				int xx = (int)(x*NUM);
				int yy = (int)(y*NUM);
				x = arr[xx][yy];
				return fixed4(x,0.1,coord,1);
			}
			ENDCG
		}

	}
	CustomEditor "ActiveShaderGUI"
}
