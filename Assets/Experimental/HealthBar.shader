Shader "Sprites/HealthBar"
{
    Properties
    {
        _Transparency("Transparency", Range(0, 1)) = 1

        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [Header(Life)] _Color("Main Color", Color) = (0,1,0,1)
        _Steps("Steps", Float) = 1
        _Percent("Percent", Float) = 1

        [Header(Damages)]/*_DamagesColor("Damages color", Color) = (1,0,0,1)
        _DamagesPercent("Damages Percent", Float) = 0*/
        _DamagedColor("Damaged color", Color) = (0.5,0,0,1)


        [Header(Border)]_BorderColor("Border color", Color) = (0.1,0.1,0.1,1)
        _BorderWidth("Border width", Float) = 1
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0


        _ImageSize("Image Size", Vector) = (100, 100, 0, 0)
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Cull Off
            Lighting Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            Pass
            {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ PIXELSNAP_ON
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    half2 texcoord  : TEXCOORD0;
                };

                half _Transparency;

                fixed4 _Color;
                half _Steps;
                half _Percent;

                fixed4 _DamagesColor;
                half _DamagesPercent;
                fixed4 _DamagedColor;

                fixed4 _BorderColor;
                half _BorderWidth;

                v2f vert(appdata_t IN)
                {
                    v2f OUT;
                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.texcoord = IN.texcoord;
                    #ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
                    #endif

                    return OUT;
                }

                sampler2D _MainTex;
                float4 _ImageSize;

                fixed4 frag(v2f IN) : SV_Target
                {
                    fixed4 c = tex2D(_MainTex, IN.texcoord);

                    float sizedX = IN.texcoord.x * _ImageSize.x;
                    float sizedY = IN.texcoord.y * _ImageSize.y;
                    if (sizedX % (_ImageSize.x / _Steps) < _BorderWidth)
                        c *= _BorderColor;
                    else if (sizedY < _BorderWidth)
                        c *= _BorderColor;
                    else if (sizedX > _ImageSize.x - _BorderWidth)
                        c *= _BorderColor;
                    else if (sizedY > _ImageSize.y - _BorderWidth)
                        c *= _BorderColor;
                    else
                    {
                        if (IN.texcoord.x > _Percent/* + _DamagesPercent*/)
                            c *= _DamagedColor;
                        /*else if (IN.texcoord.x > _Percent)
                            c *= _DamagesColor;*/
                        else
                            c *= _Color;
                    }

                    c.a *= _Transparency;
                    c.rgb *= c.a;
                    return c;
                }
            ENDCG
            }
        }
}