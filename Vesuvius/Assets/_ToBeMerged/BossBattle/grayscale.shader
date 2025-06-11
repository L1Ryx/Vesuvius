Shader "Custom/URP2DGrayscale"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GrayscaleAmount ("Grayscale Amount", Range(0,1)) = 0
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Color;
            float _GrayscaleAmount;

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color      : COLOR;
                float2 uv        : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // Flip sprite UVs correctly (handles sprite flipping, atlas, etc)
                v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteProps.xy);

                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                o.color = v.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Sample the texture with URP macros (handles mipmaps properly)
                half4 texColor = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                // Convert to grayscale
                float gray = dot(texColor.rgb, float3(0.3, 0.59, 0.11));
                float3 grayscale = float3(gray, gray, gray);

                // Blend original color and grayscale by _GrayscaleAmount
                texColor.rgb = lerp(texColor.rgb, grayscale, _GrayscaleAmount);

                return texColor;
            }

            ENDHLSL
        }
    }

    Fallback "Universal Render Pipeline/2D/Sprite-Lit-Default"
}
