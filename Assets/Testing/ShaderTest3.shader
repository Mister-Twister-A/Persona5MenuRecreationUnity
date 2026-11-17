Shader "CustomRenderTexture/XRayEffect"
{
    Properties
    {
        _MainColor("Foreground color", Color) = (0,0,0,0)
        _BackgroundColor("Background color", Color) = (1,1,1,1)
    }

     SubShader
     {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag 

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _MainColor;
                float4 _BackgroundColor;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct appdata{
                float4 positionOS : POSITION;
            };

            struct v2f {
                float4 positionCS : SV_POSITION;
                float4 positionSS : TEXCOORD0;
            };

            v2f vert(appdata a){
                v2f o = (v2f)0;

                o.positionCS = TransformObjectToHClip(a.positionOS.xyz);
                o.positionSS = ComputeScreenPos(o.positionCS);

                return o;
            }

            float4 frag(v2f v) : SV_TARGET{
                float2 screenUV = v.positionSS.xy / v.positionSS.w;
                float rawDepth = SampleSceneDepth(screenUV);

                float linearDepth = Linear01Depth(rawDepth, _ZBufferParams);

                return lerp(_MainColor, _BackgroundColor, linearDepth);
            }
            
            ENDHLSL
        }
    }
}
