Shader "Custom/CircleShadow" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Radius ("Radius", Range(0, 0.5)) = 0.25
        _ShadowDistance ("Shadow Distance", Range(0, 1)) = 0.1
        _ShadowColor ("Shadow Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float _Radius;
            float _ShadowDistance;
            float4 _ShadowColor;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float remap(float value, float from1, float to1, float from2, float to2) {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }

            float inverseLerp(float a, float b, float value) {
                return (value - a) / (b - a);
            }

            float lerp(float a, float b, float t) {
                return a + (b - a) * t;
            }

            float easing(float x) {
                // easeOutCubic
                return 1 - pow(1 - x, 3);
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uvCenter = i.uv - float2(0.5, 0.5);
                float dist = length(uvCenter);
                if(dist > _Radius + _ShadowDistance) {
                    discard;
                }

                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                if(dist > _Radius) {
                    col.rgb = _ShadowColor.rgb;
                    float t = inverseLerp(_Radius, _Radius + _ShadowDistance, dist);
                    col.a = lerp(_ShadowColor.a, 0, easing(t));
                    
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
