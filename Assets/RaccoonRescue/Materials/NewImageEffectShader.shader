// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ui_font_default_glow_single"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
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
            CGPROGRAM
            #pragma vertex ui_vert
            #pragma fragment ui_frag
            #pragma target 3.0
 
            // prevent auto-normalize of normals & tangents on GLSL
            // This would break the shader code because we use normals
            // & tangents for a different purpose
            #pragma glsl_no_auto_normalization
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata_t
            {
                float4 vertex           : POSITION;
                fixed4 color            : COLOR;
                half2 texcoord          : TEXCOORD0;
                half4 uvRect            : TANGENT;
                fixed3 glowColor        : NORMAL;
                half2 glowSize          : TEXCOORD1;
            };
            struct v2f
            {
                float4 vertex           : SV_POSITION;
                fixed4 color            : COLOR;
                half2  texcoord         : TEXCOORD0;
                half4 uvRect            : TEXCOORD2;
                fixed4 glowColor        : TEXCOORD3;
                half2 glowSize          : TEXCOORD4;
            };
            sampler2D _MainTex;
            fixed4 _Color;
            float _GlowSize;
            #if USE_TEXTURESAMPLEADD
            fixed4 _TextureSampleAdd;
            #endif
            v2f ui_vert(appdata_t IN)
            {
                v2f OUT;
            #if USE_CLIPRECT
                OUT.worldPosition = IN.vertex;
            #endif
                float4 pos = IN.vertex;
                OUT.glowSize = IN.glowSize;
                OUT.glowColor.rgb = IN.glowColor;
                OUT.glowColor.a = saturate(IN.vertex.z);
                // reset Z pos because we abused it to store the glow strength
                pos.z = 0.0f;
                OUT.vertex = UnityObjectToClipPos(pos);
                OUT.texcoord = IN.texcoord;
                OUT.uvRect = IN.uvRect;
            #ifdef UNITY_HALF_TEXEL_OFFSET
                OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
            #endif
                OUT.color = IN.color * _Color;
                return OUT;
            }
            fixed4 ui_frag(v2f IN) : SV_Target
            {
                fixed4 color;
                // defines UV offsets (scaled by glow size) where to sample from the texture
                float2 g_kernelOffsets[8] = {float2(-0.7f,-0.7f), float2(0.f,-1.f), float2(0.7f,-0.7f),
                                             float2(-1.f , 0.0f)                  , float2(1.f , 0.f ),
                                             float2(-0.7f, 0.7f), float2(0.f, 1.f), float2(0.7f, 0.7f)};
                // font texture is alpha only so set color directly
                color.rgb = IN.color.rgb;
                color.a = tex2D(_MainTex, IN.texcoord).a * IN.color.a;
                // clip base texture. Our rect is extruded so we need to prevent
                // bleeding of neighbouring characters into the output also for
                // the base texture
                half2 tc = IN.texcoord;
                float uvClip = UnityGet2DClipping(tc, IN.uvRect);
                color.a *= uvClip;
                // create average color out of every kernel stage
                float4 glow = float4(0,0,0,0);
                for(int i=0; i<8; ++i)
                {
                    // build new UV coords with offset by the kernel
                    half2 glowTC = tc + g_kernelOffsets[i] * IN.glowSize;
        
                    // calculate clipping
                    uvClip = UnityGet2DClipping(glowTC, IN.uvRect);
                    float4 sample = tex2D(_MainTex, glowTC);
                    // fonts are alpha only so use white
                    sample.rgb = float3(1,1,1);
                    glow.rgb += sample.rgb;
                    glow.a += sample.a * uvClip;
                }
                // divide by 8 to have an average color
                glow.rgb *= 0.125f;
                // mix with main texture output
                fixed3 glowColor = IN.glowColor.rgb * glow.rgb;
                fixed glowAlpha = IN.glowColor.a;
                glow.a *= IN.color.a;
                color.rgb = lerp(glowColor, color.rgb, color.a);
                color.a += glow.a;
            #if USE_TEXTURESAMPLEADD
                color += _TextureSampleAdd;
            #endif
                return color;
            }
            ENDCG
        }
    }
}