Shader "CustomRenderTexture/Test1"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("InputTex", 2D) = "white" {}
        _ScrollSpeed("Scroll speed", Vector) = (0,0,0,0)
     }

     SubShader
     {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag 

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;
                float2 _ScrollSpeed;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct appdata{
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata a){
                v2f o = (v2f)0;

                o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
                o.uv = TRANSFORM_TEX(a.uv, _MainTex);
                return o;
            }

            float4 frag(v2f v) : SV_TARGET{
                float2 uv = v.uv + _ScrollSpeed * _Time.y;
                float4 textureColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return _Color * textureColor;
            }
            
            ENDHLSL
        }
    }
}
