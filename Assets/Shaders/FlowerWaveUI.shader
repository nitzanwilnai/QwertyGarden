Shader "UI/FlowerWaveUI_Instanced"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // Movement
        _Amplitude ("Amplitude (UI units)", Float) = 6
        _Speed ("Speed", Float) = 2
        _SwayStart ("Sway Start (UV Y)", Range(0,1)) = 0.8

        // Per-instance controls (set by script)
        _PhaseOffset ("Phase Offset (radians)", Float) = 0
        _SpeedMul ("Speed Multiplier", Float) = 1

        // --- Standard UI masking/stencil props ---
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            float _Amplitude;
            float _Speed;
            float _SwayStart;

            float _PhaseOffset;
            float _SpeedMul;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                float2 texcoord      : TEXCOORD0;
                fixed4 color         : COLOR;
                float4 worldPosition : TEXCOORD1; // for clipping
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                // Only sway top portion of the sprite (based on UV.y)
                float swayMask = saturate((v.texcoord.y - _SwayStart) / max(1e-5, (1.0 - _SwayStart)));

                float t = _Time.y * (_Speed * _SpeedMul) + _PhaseOffset;
                float sway = sin(t) * _Amplitude;

                float4 local = v.vertex;
                local.x += sway * swayMask;

                o.worldPosition = local;
                o.vertex = UnityObjectToClipPos(local);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = (tex2D(_MainTex, i.texcoord) + _TextureSampleAdd) * i.color;

                #ifdef UNITY_UI_CLIP_RECT
                c.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(c.a - 0.001);
                #endif

                return c;
            }
            ENDCG
        }
    }
}
