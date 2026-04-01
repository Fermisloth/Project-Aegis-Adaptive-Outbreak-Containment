Shader "Custom/RiskHeatmap"
{
    Properties
    {
        _MainTex ("Density Map", 2D) = "white" {}
        _ColorEmpty ("Empty Color", Color) = (0.3, 0.3, 0.3, 1) // Neutral Grey
        _ColorHigh ("High Density Color", Color) = (1, 0, 0, 1) // Red
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry"}
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionHCS  : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _ColorEmpty;
                float4 _ColorHigh;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half3 finalColor = lerp(_ColorEmpty.rgb, _ColorHigh.rgb, texColor.r);
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
