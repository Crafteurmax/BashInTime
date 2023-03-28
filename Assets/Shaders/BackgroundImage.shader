Shader "Unlit/BackgroundImage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                float xdist = -10.0f*(0.5 - i.uv.x) * (0.5 - i.uv.x);

                float sinTime = 1.5 + 1.0*sin(20.0*_Time);
                float cosTime = 1.0 + 0.05 * cos(13 * _Time);

                fixed4 col = fixed4(0.5 -i.uv.y - xdist * sinTime, 0.4-i.uv.y, (1.1 -i.uv.y)*cosTime, 1);

                return col;
            }
            ENDCG
        }
    }
}
