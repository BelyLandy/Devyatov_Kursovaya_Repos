Shader "Custom/StencilMask"
{
    Properties
    {
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
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
            float _Cutoff;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mc = tex2D(_MaskTex, i.uv);
                if (mc.a < _Cutoff)
                    discard;
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
