Shader "Custom/SoftStencilMask"
{
    Properties
    {
        _MaskTex   ("Mask Texture", 2D) = "white" {}
        _Cutoff    ("Alpha Cutoff", Range(0,1)) = 0.5
        _Softness  ("Softness",    Range(0,0.5)) = 0.05
    }
    SubShader
    {
        // Рендерится перед основным изображением
        Tags { "Queue"="Geometry-10" }

        Pass
        {
            ColorMask 0
            ZWrite Off
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MaskTex;
            float    _Cutoff;
            float    _Softness;

            struct appdata {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Центральная и 4 соседних выборки
                float2 uv = i.uv;
                float sum = tex2D(_MaskTex, uv).a;                                             // центр
                sum += tex2D(_MaskTex, uv + float2(_Softness,  0)).a;   // право
                sum += tex2D(_MaskTex, uv + float2(-_Softness,  0)).a;   // лево
                sum += tex2D(_MaskTex, uv + float2(0,  _Softness)).a;   // вверх
                sum += tex2D(_MaskTex, uv + float2(0, -_Softness)).a;   // вниз

                float avg = sum / 5;                        // усредняем :contentReference[oaicite:5]{index=5}
                // Плавная граница: интерполяция через smoothstep :contentReference[oaicite:6]{index=6}
                float mask = smoothstep(_Cutoff - _Softness, _Cutoff + _Softness, avg);
                if (mask < 0.5)
                    discard;
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
