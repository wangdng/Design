Shader "Unlit/miniMap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_miniMapMask("miniMapMask",2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			blend srcalpha oneminussrcalpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _miniMapMask;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;

				col.rgb = tex2D(_MainTex, i.uv).rgb;

				fixed4 mask = tex2D(_miniMapMask,i.uv);

				col.a = mask.a;

				return col;
			}
			ENDCG
		}
	}
}
