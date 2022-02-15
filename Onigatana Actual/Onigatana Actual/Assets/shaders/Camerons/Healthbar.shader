Shader "Unlit/Healthbar"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _StartColor("Low Health", Color) = (1,1,1,1)
        _EndColor("Max Health", Color) = (1,1,1,1)
        _Health ("Health", Range(0,1)) = 1

        _Threshold("Flash Threshold", Range(0,1)) = 0.3
        _Frequency ("Flash Frequency", Range(0,10)) = 5
        _Magnitude ("Flash Magnitude", Range(0,1)) = 0.3

        _BorderSize("BorderSize", Range(0.0,0.5)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
     
            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct FragInput
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Health;
            float4 _StartColor;
            float4 _EndColor;
            float _Frequency;
            float _Magnitude;
            float _Threshold;
            float _BorderSize;

            FragInput vert (MeshData v)
            {
                FragInput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float InverseLerp(float a, float b, float v)
            {
                return (v - a) / (b - a);
            }

            float4 frag(FragInput i) : SV_Target
            {
                //rounded corner cliping
                float2 coords = i.uv;
                coords.x *= 8;
                float2 pointOnLineSeg = float2(clamp(coords.x,0.5,7.5), 0.5);
                float sdf = distance(coords, pointOnLineSeg) * 2 - 1;
                clip(-sdf);

                //border
                float borderSdf = sdf + _BorderSize;
                float borderMask = step(0, -borderSdf);

                //return float4(borderMask.xxx, 1);


                float tHealthColor = InverseLerp(0.2,0.8, _Health);
                float3 healthbarColor = lerp(_StartColor, _EndColor, tHealthColor);
                float3 bgColor = float3(0, 0, 0);

                float healthbarMask = i.uv.x < _Health;
                float flash = 1;
                if (_Health < _Threshold)
                {
                    flash = cos(_Time.y * _Frequency) * _Magnitude + 1;
                }

                float3 outColor = lerp(bgColor, healthbarColor * flash * borderMask, healthbarMask);

                

                return float4 (outColor,0);
            }
            ENDCG
        }
    }
}
