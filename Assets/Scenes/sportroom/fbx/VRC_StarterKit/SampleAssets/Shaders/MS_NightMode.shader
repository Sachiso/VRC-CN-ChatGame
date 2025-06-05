// Copyright (c) 2020 momoma
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

// Copyright (c) 2022 xlwnya
// SPS-I & GPU-I support
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

Shader "MomomaShader/OverwriteScreen/NightMode"
{
	Properties
	{
		_Alpha ("Alpha", Range(0, 1)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Overlay+6000" "IgnoreProjector" = "True" "DisableBatching" = "True" }
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

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = float4(2.0 * v.texcoord.x - 1.0, 1.0 - 2.0 * v.texcoord.y, 1.0, 1.0);
				o.uv = v.texcoord;
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
