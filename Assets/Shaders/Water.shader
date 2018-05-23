// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Water" {
	Properties {
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
      	_Shininess ("Shininess", Float) = 10
		_Distortion("Distortion", float) = 1
		_Test("Test", float) = 1
		_Test2("Test2", float) = 1
		_Test3("Test2", float) = 1
		_Noise("Noise", Vector) = (1, 1, 1, 0)
		_Noise2("Noise2", Vector) = (1, 1, 1, 0)
		_Noise3("Noise3", Vector) = (1, 1, 1, 0)
		_NormalMap("Normal map", 2D) = "bump" {}
		_Color("Color", Color) = (0, 0, 0, 0)
		_Tess ("Tesselation", Range(1,32)) = 3
		_minDist ("minDist", float) = 25
		_maxDist ("maxDist", float) = 100
	}
	SubShader {

		GrabPass
        {
            "_GrabTexture"
        }

		Tags { "RenderType" = "Opaque" "Queue" = "Transparent" "LightMode"="ForwardBase" }

		Cull off
		ZWrite off

		Pass {

		CGPROGRAM
		#pragma target 5.0
		#pragma vertex vert
		#pragma fragment frag
		#pragma hull hull
		#pragma domain domain

		#include "Tessellation.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "UnityLightingCommon.cginc"
		#include "Assets/Shaders/SimplexNoise.cginc"

		float _Tess;
		float _minDist;
		float _maxDist;
		float _Test;
		float _Test2;
		float _Test3;
     	float _Shininess;
     	float _Distortion;
     	float4 _Color;
     	float4 _Noise;
		float4 _Noise2;
		float4 _Noise3;
     	sampler2D _NormalMap;
     	sampler2D _GrabTexture;
     	uniform sampler2D _CameraDepthTexture;

		struct vert_Input
		{
			float4 vertex : POSITION;
		};

		struct hull_Input
		{
			float4 pos : POSITION;
		};

		struct hull_ConstantOutput
		{
			float TessFactor[3]    : SV_TessFactor;
			float InsideTessFactor : SV_InsideTessFactor;
		};

		struct hull_ControlPointOutput
		{
			float4 pos : POS;
		};

		struct domain_Output
		{
			float4 pos : POSITION;
			float4 scrPos : TEXCOORD1;
		};

		struct frag_Input
		{
			float3 vert : POS;
			float4 pos : POSITION;
			float4 scrPos : TEXCOORD1; // Screen pos
			float3 normal : NORMAL;
			float4 grabPos : TEXCOORD0;

		};

		struct frag_Output
		{
			float4 color : SV_Target0;
		};

		hull_Input vert( vert_Input input )
		{
			hull_Input output;
			output.pos = input.vertex;
			return output;
		}

		hull_ConstantOutput hullConstant( InputPatch<hull_Input, 3> input, uint id : SV_PrimitiveID )
		{
			hull_ConstantOutput Output = (hull_ConstantOutput)0;

			// Set tessellation factors for the edges of the triangle
	        float4 tess = UnityDistanceBasedTess(input[0].pos, input[1].pos, input[2].pos, _minDist, _maxDist, _Tess);

	        Output.TessFactor[0] = tess.x;
	        Output.TessFactor[1] = tess.y;
	        Output.TessFactor[2] = tess.z;

	        // Set tessellation factor inside the triangle
			Output.InsideTessFactor = tess.w;

			return Output;
		}

		[domain("tri")]
		[partitioning("integer")]
		[outputtopology("triangle_cw")]
		[patchconstantfunc("hullConstant")]
		[outputcontrolpoints(3)]
		hull_ControlPointOutput hull( InputPatch<hull_Input, 3> input, uint uCPID : SV_OutputControlPointID )
		{
			hull_ControlPointOutput output = (hull_ControlPointOutput)0;
			output.pos = input[uCPID].pos;
			return output;
		}

		[domain("tri")]
		frag_Input domain( hull_ConstantOutput hullConstantData, const OutputPatch<hull_ControlPointOutput, 3> Input, float3 BarycentricCoordomain : SV_DomainLocation )
		{
			float fU = BarycentricCoordomain.x;
			float fV = BarycentricCoordomain.y;
			float fW = BarycentricCoordomain.z;

			frag_Input output = (frag_Input)0;

			// Vertex manipulation
			float3 pos0 = Input[0].pos * fU + Input[1].pos * fV + Input[2].pos * fW;
			float3 pos1 = pos0 + float3(0.1, 0, 0);
			float3 pos2 = pos0 + float3(0, 0, 0.1);

			pos0.y += _Noise.x * snoise(pos0.xyz * _Noise.y + _Time * _Noise.z);
			pos1.y += _Noise.x * snoise(pos1.xyz * _Noise.y + _Time * _Noise.z);
			pos2.y += _Noise.x * snoise(pos2.xyz * _Noise.y + _Time * _Noise.z);

			//pos0.y += _Noise2.x * snoise(pos0.xyz * _Noise2.y + _Time * _Noise2.z);
			//pos1.y += _Noise2.x * snoise(pos1.xyz * _Noise2.y + _Time * _Noise2.z);
			//pos2.y += _Noise2.x * snoise(pos2.xyz * _Noise2.y + _Time * _Noise2.z);

			//pos0.y += _Noise3.x * snoise(pos0.xyz * _Noise3.y + _Time * _Noise3.z);
			//pos1.y += _Noise3.x * snoise(pos1.xyz * _Noise3.y + _Time * _Noise3.z);
			//pos2.y += _Noise3.x * snoise(pos2.xyz * _Noise3.y + _Time * _Noise3.z);

			// Normal calculation
			float3 normal0 = normalize(cross(pos1 - pos0, pos2 - pos0));
			float3 normal1 = normalize(cross(pos2 - pos1, pos0 - pos1));
			float3 normal2 = normalize(cross(pos0 - pos2, pos1 - pos2));

			// Sharper wave crests (pull vertices)
			pos0 += normal0 * _Noise.w;
			pos1 += normal1 * _Noise.w;
			pos2 += normal2 * _Noise.w;

			// Normalize again
			normal0 = normalize(cross(pos1 - pos0, pos2 - pos0));

			// Prepare vertex output
			output.pos = UnityObjectToClipPos(float4(pos0, 1));
			output.vert = pos0;
			normal0 = UnityObjectToWorldNormal(normal0);
			output.normal = normal0;
			output.scrPos = ComputeScreenPos(output.pos);

            // get the grabpass coordinates
            output.grabPos = ComputeGrabScreenPos(UnityObjectToClipPos(pos0));

			return output;
		}

		frag_Output frag( frag_Input input )
		{
			frag_Output output = (frag_Output)0;

			// Calculate diffuse lighting
			float3 normalDirection = input.normal;
			float3 vert = mul(unity_ObjectToWorld, input.vert);
			float3 viewDirection = normalize(_WorldSpaceCameraPos - vert);
    		float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
    		float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

    		float3 diffuseReflection = _LightColor0.rgb * _Color.rgb *
    			max(0.0, dot(normalDirection, -lightDirection));

    		float3 specularReflection;

        	if (dot(normalDirection, lightDirection) > 0.0)  // Angle between normal and light
        	{
           		specularReflection = float3(0.0, 0.0, 0.0);
        	}
        	else
        	{
           		specularReflection = _Test2 * _LightColor0.rgb * _SpecColor.rgb *
           			pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
        	}

        	// Assign pixel color
        	output.color = float4(ambientLighting + diffuseReflection + specularReflection, 1.0);

			half4 grabPassColor = tex2Dproj(_GrabTexture, input.grabPos + _Distortion * input.normal.x * input.normal.y * input.normal.z);
			output.color = grabPassColor * output.color * output.color.a;

			// Check depthbuffer
			float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, input.scrPos));

			float distToCamera = input.pos.w;

			float diff = (abs(sceneZ - distToCamera));

			if(diff <= _Test)
			{
				output.color.rgb = float3(1, 1, 1);
			}

			return output;
		}

		ENDCG

		}
	}



}
