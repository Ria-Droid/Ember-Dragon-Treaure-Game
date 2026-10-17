Shader "Ember/KaelFresnel"
{

    Properties
    {
        _BaseColor ("Base Color", Color) = (0.2, 0.5, 0.3, 1)
        _RimColor ("Rim Color", Color) = (0.55, 0.9, 0.7, 1)
        _RimPower ("Rim Power", Range(0.5, 8)) = 3.0
        _RimIntensity ("Rim Intensity", Range(0, 5)) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _RimColor;
                float _RimPower;
                float _RimIntensity;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = positionInputs.positionCS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);

                //So that Kael still reads as lit.
                Light mainLight = GetMainLight();
                float ndotl = saturate(dot(normalWS, mainLight.direction));
                float3 diffuse = _BaseColor.rgb * (ndotl * 0.7 + 0.3) * mainLight.color;

                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _RimPower);
                float3 rim = _RimColor.rgb * fresnel * _RimIntensity;

                float3 finalColor = diffuse + rim;
                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
