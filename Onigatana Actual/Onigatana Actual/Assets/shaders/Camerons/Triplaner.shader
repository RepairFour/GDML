Shader "Unlit/Triplaner"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Tint Colour", Color) = (1,1,1,1)
        _Scale("Scale", Range(0.01, 1)) = 1
        _Sharpness("Blend Sharpness", Range(1,64)) = 1
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
                float3 normal : NORMAL;
            };

            struct FragInput
            {
                float4 position : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Scale;
            float _Sharpness;

            FragInput vert (MeshData v)
            {
                FragInput o;
                o.position = UnityObjectToClipPos(v.vertex);
                //cal world pos of vertex
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex) ;
                o.worldPos = worldPos.xyz * _Scale;
      
                //calculate world normal
                float3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.normal = normalize(worldNormal);
                return o;
            }

            fixed4 frag(FragInput i) : SV_Target
            {
              

                float2 uv_front = TRANSFORM_TEX(i.worldPos.xy, _MainTex);
                float2 uv_side = TRANSFORM_TEX(i.worldPos.zy, _MainTex);
                float2 uv_top = TRANSFORM_TEX(i.worldPos.xz, _MainTex);


                //read texture at uv position of the three projections
                fixed4 col_front = tex2D(_MainTex, uv_front);
                fixed4 col_side = tex2D(_MainTex, uv_side);
                fixed4 col_top = tex2D(_MainTex, uv_top);


                float3 weights = i.normal;
                weights = abs(weights);
                weights = pow(weights, _Sharpness);
                weights = weights / (weights.x + weights.y + weights.z);

                col_front *= weights.z;
                col_side *= weights.x;
                col_top *= weights.y;

                fixed4 col = col_front + col_side + col_top;

                col *= _Color;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Standard"
}
