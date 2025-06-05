// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

// Copyright (c) 2022 xlwnya
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/UI/CubeMockGeom"
{
	Properties
	{
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

		_RimStrength ("Rim Strength", Range(0, 1)) = 1.0
		_MaxRadius ("Max Radius", Float) = -1.0
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "TransparentCutout"
			"Queue" = "AlphaTest"
			"IgnoreProjector"="True"
			//"PreviewType"="Plane"
			//"CanUseSpriteAtlas"="True"
			"DisableBatching"="True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Lighting Off
		ColorMask [_ColorMask]

		Pass
		{
			//Name "Default"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
				float3 uv3 : TEXCOORD2;
				float3 uv4 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float3 center : TEXCOORD2;
				float3 size : TEXCOORD3;
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			struct fragOut
			{
				fixed4 color : SV_Target;
				float depth : SV_Depth;
			};

			fixed4 _Color;
			float4 _ClipRect;
			
			fixed _RimStrength;
			fixed _MaxRadius;

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = v.vertex;
				o.color = v.color * _Color;
				o.center = v.uv3;
				o.size = v.uv4;
				return o;
			}

			inline float4 intersect_RoundedCube(float3 r0, float3 rd, float3 c, float3 s)
			{
				float3 v = c - r0;
				float3 v1 = v / rd - s / abs(rd);
				float t = max(v1.x, max(v1.y, v1.z));
				float r = min(s.x, s.y);
				r = 0 < _MaxRadius ? min(_MaxRadius, r) : r;
				float2 h = max(0.0, s.xy - r);
				v.xy = clamp(t * rd.xy, v.xy - h, v.xy + h);
				float b = dot(v.xy, normalize(rd.xy));
				float d = b * b + r * r - dot(v.xy, v.xy);
				float l = (b - sqrt(max(d, 0.0))) / length(rd.xy);
				float2 tv = (t * rd - v).xy;
				return 0 <= d && abs(l * rd.z - v.z) < s.z ? float4(l, normalize(float3(v.xy - rd.xy * l, 0.0))) : (dot(tv, tv) < r * r ? float4(t, float3(0.0, 0.0, sign(v.z))) : -1.0);
			}

			inline float compute_depth(float4 pos)
			{
				#if UNITY_UV_STARTS_AT_TOP
					return pos.z / pos.w;
				#else
					return (pos.z / pos.w) * 0.5 + 0.5;
				#endif
			}

			fragOut frag(v2f i)
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float4 color = i.color;
				
				#ifdef UNITY_UI_CLIP_RECT
				color.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				float3 r0 = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
				float3 rd = normalize(i.worldPos - r0);
				float4 d = intersect_RoundedCube(r0, rd, i.center, i.size * 0.5);
				clip(d.x);
				float rim = 1 - abs(dot(d.yzw, rd));
				color.rgb = lerp(color.rgb, 1.0, rim * rim * rim * _RimStrength);

				fragOut o;
				o.color = color;
				o.depth = compute_depth(UnityObjectToClipPos(r0 + rd * d.x));
				return o;
			}
			ENDCG
		}
	}
}
