Shader "VRStudy/VRVignette"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _Radius ("Open Radius", Range(0.0, 1.0)) = 0.95
        _Softness ("Softness", Range(0.01, 0.5)) = 0.15
        _Intensity ("Intensity", Range(0.0, 1.0)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)

            float4 _Color;
            float _Radius;
            float _Softness;
            float _Intensity;

            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.uv = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 center = float2(0.5, 0.5);

                float distanceFromCenter =
                    distance(input.uv, center);

                float alpha =
                    smoothstep(
                        _Radius,
                        _Radius + _Softness,
                        distanceFromCenter
                    );

                alpha *= _Intensity;

                return half4(
                    _Color.rgb,
                    alpha
                );
            }

            ENDHLSL
        }
    }
}