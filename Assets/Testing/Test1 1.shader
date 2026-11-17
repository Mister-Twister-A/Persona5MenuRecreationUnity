Shader "CustomRenderTexture/Test1Transperent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("InputTex", 2D) = "white" {}
        _ScrollSpeed("Scroll speed", Vector) = (0,0,0,0)
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source blend mode", Integer) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("destination blend mode", Integer) = 10
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
            Tags{
                "LightMode" = "SRPDefaultUnLit"
            }
            Blend [_SrcBlend] [_DstBlend]
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
        
        Pass{
            Tags{
                "LightMode" = "DepthOnly"
            }

            ZWrite On 
            ColorMask R 

            HLSLPROGRAM
            #pragma vertex depthOnlyVert
            #pragma fragment depthOnlyFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata{
                float4 positionOS : POSITION;
            };

            struct v2f {
                float4 positionCS : SV_POSITION;
            };

            v2f depthOnlyVert(appdata v){
                v2f o = (v2f)0;

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);

                return o;
            }

            float depthOnlyFrag(v2f v): SV_TARGET{
                return v.positionCS.z;
            }
            ENDHLSL

        }

        Pass{
            Tags{
                "LightMode" = "DepthNormals"
            }
            ZWrite On
            HLSLPROGRAM
            #pragma vertex depthNormalVertex
            #pragma fragment depthNormalFragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            

            struct appdata{
                float4 positionOS : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };

            v2f depthNormalVertex(appdata v){
                v2f o = (v2f)0;

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normal);
                o.normalWS = NormalizeNormalPerVertex(normalWS);
                return o;
            }

            float4 depthNormalFragment(v2f v) : SV_TARGET{
                float3 normalWS = NormalizeNormalPerPixel(v.normalWS);

                return float4 (normalWS, 0.0f);
            }
            ENDHLSL
        }
    }

}
