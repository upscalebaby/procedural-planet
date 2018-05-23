// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Terrain" {
	Properties {
		_UseCircularSplat("Use Circular Splat", Range(0,1)) = 0
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
  		_Shininess ("Shininess", Float) = 10
		_Grassland("Grassland", Vector) = (0, 0, 0, 0)
		_Tundra("Tundra", Vector) = (0, 0, 0, 0)
		_Arctic("Arctic", Vector) = (0, 0, 0, 0)
		_Test("Test", float) = 1
		_Test2("Test2", float) = 1
		_Test3("Test3", float) = 1
		_ArcticHeight("Arctic Height", float) = 1
		_DispTex("Disp Texture", 2D) = "gray" {}
		_Color("Color", Color) = (0, 0, 0, 0)
		_Tess ("Tesselation", Range(1,64)) = 3
		_minDist ("minDist", float) = 25
		_maxDist ("maxDist", float) = 100
	}
	SubShader {

		Tags { "RenderType" = "Opaque" "Queue" = "Background" "LightMode"="ForwardBase" }

		Pass {

			ZWrite on

			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma hull hull
			#pragma domain domain
			//#pragma geometry geom
			//#pragma addshadow

			#include "UnityCG.cginc"
			#include "Tessellation.cginc"
			#include "UnityLightingCommon.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"
			#include "Assets/Shaders/SimplexNoise.cginc"

			uniform sampler2D _CameraDepthTexture;

			float _UseCircularSplat;
	     	float _Shininess;
	     	sampler2D _DispTex;
	     	float4 _Color;
	     	float4 _Arctic;
	     	float4 _Tundra;
	     	float4 _Grassland;
	     	float4 _CameraPos;
			float _Tess;
			float _minDist;
			float _maxDist;
			float _Test;
			float _Test2;
			float _Test3;
			float _ArcticHeight;

			struct vert_hull_Input
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			struct hull_ConstantOutput
			{
				float TessFactor[3]    : SV_TessFactor;
				float InsideTessFactor : SV_InsideTessFactor;
			};

			struct domain_ControlPointInput
			{
				float4 pos : POS;
				float3 normal : NORMAL;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct geom_Input
			{
				float4 pos : POS;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			struct frag_Input
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
				fixed4 diff : COLOR1;
				fixed3 ambient : COLOR2;

				float3 vert : POS;
				float4 scrPos : TEXCOORD0;

				SHADOW_COORDS(1) // put shadows data into TEXCOORD1
				//LIGHTING_COORDS(0,1)
			};

			struct frag_Output
			{		
				float4 color : SV_Target0;
				//float depth : SV_Depth;
			};

			vert_hull_Input vert( vert_hull_Input i )
			{
				vert_hull_Input o = (vert_hull_Input)0;

				o.pos = i.pos;
				o.normal = i.normal;
				o.color = i.color;
				//o.diff = ;

				return o;
			}

			hull_ConstantOutput hullConstant( InputPatch<vert_hull_Input, 3> i, uint id : SV_PrimitiveID )
			{
				hull_ConstantOutput o = (hull_ConstantOutput)0;

				// Tess factors for edges of triangle
		        float4 tess = UnityEdgeLengthBasedTess(i[0].pos, i[1].pos, i[2].pos, _Tess);

		        o.TessFactor[0] = tess.x;
		        o.TessFactor[1] = tess.y;
		        o.TessFactor[2] = tess.z;

		        // Tess factors for inside of triangle
				o.InsideTessFactor = tess.w;

				return o;
			}

			[domain("tri")]
			[partitioning("integer")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("hullConstant")]
			[outputcontrolpoints(3)]
			domain_ControlPointInput hull( InputPatch<vert_hull_Input, 3> i, uint uCPID : SV_OutputControlPointID )
			{
				domain_ControlPointInput o = (domain_ControlPointInput)0;

				o.pos = i[uCPID].pos;
				o.normal = i[uCPID].normal;
				o.color = i[uCPID].color;

				return o;
			}

			[domain("tri")]
			frag_Input domain( hull_ConstantOutput hullConstantData, const OutputPatch<domain_ControlPointInput, 3> i, float3 BarycentricCoordomain : SV_DomainLocation )
			{
				float fU = BarycentricCoordomain.x;
				float fV = BarycentricCoordomain.y;
				float fW = BarycentricCoordomain.z;

				float3 pos = i[0].pos * fU + i[1].pos * fV + i[2].pos * fW;

				// Displace vertex
				float3 pos0 = i[0].pos;
				float3 pos1 = i[1].pos;
				float3 pos2 = i[2].pos;

				float3 directionT = normalize(float3(0, 1, 0));
				//pos.xyz += directionT * _Grassland.x * snoise(pos0.xyz * _Grassland.y + _Time * _Grassland.z);

				// Normal calculation
				float3 normal0 = normalize(cross(pos1 - pos0, pos2 - pos0));
				//float3 normal1 = normalize(cross(pos2 - pos1, pos0 - pos1));
				//float3 normal2 = normalize(cross(pos0 - pos2, pos1 - pos2));

				float3 worldNormal = UnityObjectToWorldNormal(normal0);

				// Prepare vertex output
				frag_Input o = (frag_Input)0;

				// Lightning and shadows
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
			 	o.diff = nl * _LightColor0;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                TRANSFER_SHADOW(o)
                o.color = i[0].color;

                // Position data
				o.pos = UnityObjectToClipPos(float4(pos, 1));
				o.normal = normal0.xyz;
				o.vert = pos;
				o.scrPos = ComputeScreenPos(o.pos);

				return o;
			}

			[maxvertexcount(12)]
			geom_Input geom()
			{
				
			}

			frag_Output frag( frag_Input i )
			{
				frag_Output o = (frag_Output)0;

				// Get world normal
				float3 worldPos = mul(unity_ObjectToWorld, i.vert);

				// Choose reference direction
				float3 dir;
				if(_UseCircularSplat > 0) {
					dir = normalize(worldPos - float3(0, 0, 0));
				} else {
					dir = float3(0, 1, 0);
				}

				// Get angle between normal and reference direction
				float angle = dot(dir, UnityObjectToWorldNormal(i.normal));

				// Define region colors
				float3 mountain = float3(0.3, 0.4, 0.4);
				float3 darkgrass = float3(0, 0.2, 0);
				float3 snow = float3(1, 1, 1);
				float3 rural = float3(0.4, 0.4, 0.15);

				// Arctic
				if(worldPos.y > _Arctic.w) {
					float step = (worldPos.y - _Tundra.w) / (_Arctic.w + 1000 - _Tundra.w);
					float3 color = lerp(mountain, snow, float3(step, step, step));
					o.color.rgb = color;
					if(angle > _Arctic.x)
						o.color.rgb *= float3(1.5, 1.5, 1.5);	// add snow on flat areas
					else if(angle < _Arctic.y)
						o.color.rgb *= float3(0.8, 0.8, 0.9);	// Steep snow / ice
				} else
				// Tundra
				if(worldPos.y > _Tundra.w) {
					float step = (worldPos.y - 4000) / (_Tundra.w + 2000 - 4000);
					float3 color = lerp(mountain, snow, float3(step, step, step));
					o.color.rgb = color;
					if(angle > _Tundra.x)
						o.color.rgb *= float3(1.5, 1.5, 1.5) * angle;	// Spread snow on flat Tundra
					else if(angle < _Tundra.y)
						o.color.rgb *= float3(1.1, 1.1, 1.1) * angle;	// Darken the tundra
					else if(angle > _Tundra.z)
						o.color.rgb *= float3(0.6, 0.6, 0.8) * angle;	// Steep snow / ice
					//else
					//	o.color.rgb = float3(0.3, 0.4, 0.4);	// Grassland Mountain
				} else
				// Grasslands
				if(angle < _Grassland.x)
					o.color.rgb = float3(0.3, 0.4, 0.4);	// Mountain
				else if(angle < _Grassland.y)
					o.color.rgb = float3(0, 0.2, 0);	// Dark green
				else if(angle < _Grassland.z)
					o.color.rgb = float3(0.01, 0.4, 0);	// Bright green
				else if(angle < _Grassland.w)
					o.color.rgb = float3(0.4, 0.4, 0.15);	// Rural area

				// External Billboard Grass shader
				float dist = distance(_WorldSpaceCameraPos, worldPos.xyz);
				if(dist < 750) {
					//o.color.r = 0.3;
				}

				// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
                fixed shadow = SHADOW_ATTENUATION(i);

                // darken light's illumination with shadow, keep ambient intact
                fixed3 lighting = i.diff * shadow + i.ambient;
                o.color.rgb *= lighting;

				return o;
			}

			ENDCG

		}
		// shadow casting support
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}

}