Shader "Custom/PaletteSwapSpriteShader"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorSwapCount ("Color Swap Count", Range(0,5)) = 2
        
        // Пара 0
        _OldColor0 ("Old Color 0", Color) = (1,1,1,1)
        _NewColor0 ("New Color 0", Color) = (1,1,1,1)
        
        // Пара 1
        _OldColor1 ("Old Color 1", Color) = (0,0,0,1)
        _NewColor1 ("New Color 1", Color) = (0,0,0,1)
        
        // Пара 2
        _OldColor2 ("Old Color 2", Color) = (1,0,0,1)
        _NewColor2 ("New Color 2", Color) = (1,0,0,1)
        
        // Пара 3
        _OldColor3 ("Old Color 3", Color) = (0,1,0,1)
        _NewColor3 ("New Color 3", Color) = (0,1,0,1)
        
        // Пара 4
        _OldColor4 ("Old Color 4", Color) = (0,0,1,1)
        _NewColor4 ("New Color 4", Color) = (0,0,1,1)
    }
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ColorSwapCount;
            fixed4 _OldColor0;
            fixed4 _NewColor0;
            fixed4 _OldColor1;
            fixed4 _NewColor1;
            fixed4 _OldColor2;
            fixed4 _NewColor2;
            fixed4 _OldColor3;
            fixed4 _NewColor3;
            fixed4 _OldColor4;
            fixed4 _NewColor4;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                half2 texcoord : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }
            
            // Функция для замены цвета. Использует сравнение с порогом.
            fixed4 paletteSwap(fixed4 col)
            {
                // Порог для сравнения (можно настроить, если требуется более «жёсткое» или «мягкое» сравнение)
                float threshold = 0.01;
                
                if(_ColorSwapCount > 0.0 && distance(col, _OldColor0) < threshold)
                {
                    return _NewColor0;
                }
                if(_ColorSwapCount > 1.0 && distance(col, _OldColor1) < threshold)
                {
                    return _NewColor1;
                }
                if(_ColorSwapCount > 2.0 && distance(col, _OldColor2) < threshold)
                {
                    return _NewColor2;
                }
                if(_ColorSwapCount > 3.0 && distance(col, _OldColor3) < threshold)
                {
                    return _NewColor3;
                }
                if(_ColorSwapCount > 4.0 && distance(col, _OldColor4) < threshold)
                {
                    return _NewColor4;
                }
                
                return col;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Получаем цвет пикселя и умножаем его на vertex color
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                // Применяем палитровую замену
                col = paletteSwap(col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
