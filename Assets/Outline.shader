Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline", Color) = (0,0,0,0)
        _OutlineWidth ("Outline Width", Float) = 0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
        #pragma multi_compile_local _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        #include "UnitySprites.cginc"

        fixed4 _OutlineColor;
        float _OutlineWidth;

        struct Input
        {
            float2 uv_MainTex;
            fixed4 color;
        };

        void vert (inout appdata_full v, out Input o)
        {
            v.vertex = UnityFlipSprite(v.vertex, _Flip);

            #if defined(PIXELSNAP_ON)
            v.vertex = UnityPixelSnap (v.vertex);
            #endif

            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color * _RendererColor;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 mainTextUV = IN.uv_MainTex;
            
            // On créer 4 copies des textures avec un décalage haut/bas/gauche/droite
            float2 upTextUV = IN.uv_MainTex - float2 (0.0f, _OutlineWidth);
            float2 downTextUV = IN.uv_MainTex + float2 (0.0f, _OutlineWidth);
            
            float2 leftTextUV = IN.uv_MainTex - float2 (_OutlineWidth, 0.0f);
            float2 rightTextUV = IN.uv_MainTex + float2 (_OutlineWidth, 0.0f);
            

            // Ici on gère les couleurs des textures (enfin je crois)
            fixed4 c_main = SampleSpriteTexture (mainTextUV) * IN.color;

            fixed4 c_upTextUV = SampleSpriteTexture (upTextUV) * IN.color;
            fixed4 c_downTextUV = SampleSpriteTexture (downTextUV) * IN.color;
            
            fixed4 c_leftTextUV = SampleSpriteTexture (leftTextUV) * IN.color;
            fixed4 c_rightTextUV = SampleSpriteTexture (rightTextUV) * IN.color;
           
            
            // Pour finir on s'occuppe de l'affichage des textures
            if(c_main.a == 0) {
                o.Emission = 0;
                o.Emission += _OutlineColor.rgb * c_upTextUV.a;
                o.Emission += _OutlineColor.rgb * c_downTextUV.a;
                
                o.Emission += _OutlineColor.rgb * c_leftTextUV.a;
                o.Emission += _OutlineColor.rgb * c_rightTextUV.a;

                
                o.Alpha = c_main.a;
            }
            else {
                o.Emission = c_main.rgb;
                o.Alpha = 1;
            }            
        }
        ENDCG
    }

Fallback "Transparent/VertexLit"
}
