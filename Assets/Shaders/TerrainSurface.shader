Shader "Custom/TerrainSurface" {
	Properties {
		_Noise ("Noise", Vector) = (1,1,1)
		_Noise0 ("Noise0", Vector) = (1,1,1)
		_Noise1 ("Noise1", Vector) = (1,1,1)
		_Noise2 ("Noise2", Vector) = (1,1,1)
		_Color ("Color", Color) = (1,1,1,1)
		_MaxAltitude ("MaxAltitude", float) = 14365
		_MinAltitude ("MinAltitude", float) = 1
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#include "SimplexNoise.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};

		float4 _Noise;
		float4 _Noise0;
		float4 _Noise1;
		float4 _Noise2;
		float _MaxAltitude;
		float _MinAltitude;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float3 steepnessShading(float steepness, Input IN) {
			float3 color;
			if(steepness < 0.85)
				color = float3(0.2, 0.2, 0.2) + (pow(steepness, _Noise.z)) * snoise(IN.worldPos.xyz * _Noise.y) * _Noise;	// Steep
			else if(steepness < 0.98)
				color = float3(0.1, 0.8, 0.4) + (pow(steepness, _Noise1.z)) * snoise(IN.worldPos.xyz * _Noise1.y) * _Noise1.x;	// Grass
			else
				color = float3(0.7, 0.7, 0.3) + (pow(steepness, _Noise2.z)) * snoise(IN.worldPos.xyz * _Noise2.y) * _Noise2.x;	// Mountainside

			return color;
		}

		float3 steepnessShading2(float steepness, Input IN) {
			float3 color;
			if(steepness < 0.90)
				color = float3(0.4, 0.5, 0.7) + (pow(steepness, _Noise.z)) * snoise(IN.worldPos.xyz * _Noise.y) * _Noise;	// brantast
			else if(steepness < 0.96)
				color = float3(0.5, 0.65, 0.3) + (pow(steepness, _Noise1.z)) * snoise(IN.worldPos.xyz * _Noise1.y) * _Noise1.x;	// brant
			else
				color = float3(1, 1, 0.3) + (pow(steepness, _Noise2.z)) * snoise(IN.worldPos.xyz * _Noise2.y) * _Noise2.x;	// platt

			return color;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float3 dir = normalize(IN.worldPos);
			float steepness = dot(dir, IN.worldNormal);
			float altitude = length(IN.worldPos);

			float3 steepnessColor = steepnessShading(steepness, IN);
			float3 steepnessColor2 = steepnessShading2(steepness, IN);

			float angleToSun = dot(_WorldSpaceLightPos0, float4(IN.worldPos, 0));

			o.Albedo = lerp(steepnessColor, steepnessColor2, (altitude - _MinAltitude) / (_MaxAltitude - _MinAltitude));
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 0.0f;
		}



		ENDCG
	}
	FallBack "Diffuse"
}
