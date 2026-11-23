Shader "UI/MaskedImage"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Mask)]
        [Toggle] _MaskInvert ("Invert Mask", Float) = 0
        [Toggle] _OutsideDiscard ("Discard Outside", Float) = 0
        _OutsideColor ("Outside Tint", Color) = (1, 0.35, 0.35, 1)
        _OutsideAmount ("Outside Tint Amount", Range(0, 1)) = 0.5

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
           HLSLPROGRAM
           #pragma vertex vert
           #pragma fragment frag

           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

           TEXTURE2D(_MainTex);
           SAMPLER(sampler_MainTex);

           CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float  _MaskInvert;
                float  _OutsideDiscard;
                float4 _OutsideColor;
                float  _OutsideAmount;

                float4x4 _MaskCanvasToLocal;   
                float2   _MaskCenter;
                float2   _MaskHalfSize;
            CBUFFER_END

            struct appdata
            {
                float4 positionOS : POSITION;   
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 canvasPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o = (v2f)0;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.canvasPos  = v.positionOS.xy;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 m = mul(_MaskCanvasToLocal, float4(i.canvasPos, 0.0, 1.0)).xy - _MaskCenter;
                float2 q = m / max(_MaskHalfSize, 1e-5);

                bool inside = max(abs(q.x), abs(q.y)) <= 1.0;

                inside = lerp(inside, 1.0 - inside, _MaskInvert);
                float outside = 1.0 - inside;
                clip(1.0 - outside * _OutsideDiscard * 2.0);

                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * i.color;

                float tintAmount = _OutsideAmount * outside * (1.0 - _OutsideDiscard);
                col.rgb = lerp(col.rgb, col.rgb * _OutsideColor.rgb, tintAmount);

                return col;
            }
            ENDHLSL
        }
    }
}