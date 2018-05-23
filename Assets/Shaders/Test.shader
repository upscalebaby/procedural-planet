Shader "Custom/Test"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 depth : TEXCOORD1;
			};

			struct v2f
			{
				float2 depth : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o = (v2f)0;
				o.vertex = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_DEPTH(o.depth);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				UNITY_OUTPUT_DEPTH(i.depth);
			}
			ENDCG
		}
	}
}
