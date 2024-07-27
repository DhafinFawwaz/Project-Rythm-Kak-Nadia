Shader "Custom/Overlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            float4 OverlayBlend(float4 baseColor, float4 blendColor)
            {
                float4 result;
                for (int i = 0; i < 3; i++)
                {
                    result[i] = baseColor[i] < 0.5 ?
                        (2 * baseColor[i] * blendColor[i]) :
                        (1 - 2 * (1 - baseColor[i]) * (1 - blendColor[i]));
                }
                result.a = blendColor.a;
                return result;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 mainTex = tex2D(_MainTex, i.texcoord);
                float4 overlayTex = float4(1, 1, 1, 1); // Assuming a white overlay color
                return OverlayBlend(mainTex, overlayTex);
            }
            ENDCG
        }
    }
}
