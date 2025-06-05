// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

// Copyright (c) 2022 xlwnya
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/OverwriteScreen/NightModeQuest"
{
	Properties
	{
		_Alpha ("Alpha", Range(0, 1)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Overlay+6000" "IgnoreProjector" = "True" }
		Pass
		{
			ZTest Always
			ZWrite Off
			Blend Zero OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_instancing

			#include "UnityCG.cginc"

			fixed _Alpha;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 xy = float2(2.0 * v.uv.x - 1.0, 2.0 * v.uv.y - 1.0);
				o.pos = mul(UNITY_MATRIX_P, float4(xy, - (_ProjectionParams.y + _ProjectionParams.z)/2, 1));
				o.pos.xy = xy * o.pos.w;
				o.pos.y *= _ProjectionParams.x;
				
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0, 0, _Alpha);
			}
			ENDCG
		}
	} 
}
