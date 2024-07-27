Shader "Custom/Unlit Grayscale Cutout"
{
    Properties
    {
        _MainTex ("Base", 2D) = "" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        [Range(0, 1)]
        _Cutoff ("Cutoff", Float) = 0.5
        _FallOff ("FallOff", Float) = 0.1
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct v2f
    {
        float4 position : SV_POSITION;
        float2 texcoord : TEXCOORD0;
    };

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _Color;
    float _Cutoff;
    float _FallOff;

    v2f vert(appdata_base v)
    {
        v2f o;
        o.position = UnityObjectToClipPos(v.vertex);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        return o;
    }

    float remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    float4 frag(v2f i) : SV_Target
    {
        float4 c = tex2D(_MainTex, i.texcoord);
        c.a = step(_Cutoff, (c.r + c.g + c.b) / 3);
        c.a = remap((c.r + c.g + c.b) / 3, _Cutoff, _FallOff, 0, 1);
        c.a = saturate(c.a);
        return c;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    FallBack "Diffuse"
}
