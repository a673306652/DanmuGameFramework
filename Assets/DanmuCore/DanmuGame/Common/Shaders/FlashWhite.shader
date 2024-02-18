Shader "Custom/FlashWhite"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Add("Add", Color) = (1,1,1,1)

	}
		SubShader
		{
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent"}
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 100
			Cull Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					half4 vertex:POSITION;
					float2 texcoord:TEXCOORD0;
					fixed4 color : COLOR;
				};

				struct v2f
				{
					half4 pos:SV_POSITION;
					float2 uv:TEXCOORD0;
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;
				fixed4 _Add;


				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color * _Color;
					return o;
				}

				fixed4 frag(v2f i) : SV_TARGET
				{
					fixed4 tex = tex2D(_MainTex, i.uv) * i.color;
					tex.rgb += _Add;
					return tex;
				}
				ENDCG
			}
		}
}