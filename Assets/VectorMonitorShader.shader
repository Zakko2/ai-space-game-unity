Shader "Custom/VectorMonitorShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Glow ("Glow Intensity", Range(0, 10)) = 5
        _GlowSharpness ("Glow Sharpness", Range(0, 10)) = 8
        _GlowExponent ("Glow Exponent", Range(0, 10)) = 4
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _Glow;
            float _GlowSharpness;
            float _GlowExponent;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float dist = distance(i.uv, float2(0.5, 0.5));
                float glowFactor = (1.0 - smoothstep(0.0, 1.0 / _GlowSharpness, dist));
                col.rgb += _Glow * pow(glowFactor, _GlowExponent);
                return col;
            }
            ENDCG
        }
    }
}
