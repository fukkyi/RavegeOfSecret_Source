Shader "Custom/Laser"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _RimColor("RimColor", Color) = (1,1,1,1)
        _RimPower("RimPower", float) = 0.0
    }
        SubShader
        {
            Tags {"RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 viewDir : TEXCOORD1;
                    float3 normalDir : TEXCOORD2;
                };

                fixed4 _Color;
                fixed4 _RimColor;
                half _RimPower;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    float4x4 modelMatrix = unity_ObjectToWorld;
                    o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
                    o.viewDir = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = _Color;

                    half rim = 1.0 - saturate(dot(i.viewDir, i.normalDir));
                    fixed3 emission = _RimColor.rgb * pow(rim, _RimPower);
                    col.rgb -= emission;
                    col = fixed4(col.rgb, 1.0);
                    return col;
                }
                ENDCG
            }
        }
}