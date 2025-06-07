Shader "Custom/ToggleGrayscaleSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GrayscaleAmount ("Grayscale Amount", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        // ✅ Correct transparency settings
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _GrayscaleAmount;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv) * i.color;

                float gray = dot(texColor.rgb, float3(0.3, 0.59, 0.11));
                float3 grayscale = float3(gray, gray, gray);
                texColor.rgb = lerp(texColor.rgb, grayscale, _GrayscaleAmount);

                // ✅ Preserve original alpha
                return texColor;
            }
            ENDCG
        }
    }
}
