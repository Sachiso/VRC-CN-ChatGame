// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

// Copyright (c) 2022 xlwnya
// SPS-I & GPU-I support
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/RayTracing/FakeFloor"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Outer Texture", 2D) = "white" {}
		_Plane ("Plane", Vector) = (0, 1, 0, 0.5)
		[Header(Fog Settings)]
		_FogDensity ("Fog Density", Range(0, 1)) = 0.4
		_FogColor ("Fog Color", Color) = (0, 0, 0, 1)
	}
	SubShader
	{
		Tags { "DisableBatching" = "True" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 objPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _Plane;
			fixed _FogDensity;
			fixed4 _FogColor;

			v2f vert(appdata_base v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.objPos = v.vertex;
				o.uv = v.texcoord;
				return o;
			}

			inline float intersect_Plane(float3 r0, float3 rd, float4 p)
			{
				return -(dot(r0, p.xyz) + p.w) / dot(rd, p.xyz);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float4 c = 1.0;
				float3 r0 = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
				float3 rd = normalize(i.objPos - r0);

				float d = intersect_Plane(r0, rd, _Plane);
				float3 p = r0 + rd * d;
				bool isPlane = (d > 0);
				c.rgb = isPlane * tex2Dgrad(_MainTex, TRANSFORM_TEX(p.xz, _MainTex), ddx(p.xz), ddy(p.xz)) * _Color;
				float fogFactor = (isPlane ? max(d - distance(i.objPos, r0), 0.0) : 9999) * _FogDensity;
				c.rgb = lerp(_FogColor.rgb, c.rgb, saturate(exp2(-fogFactor * fogFactor)));

				return c;
			}
			ENDCG
		}
	}
}
