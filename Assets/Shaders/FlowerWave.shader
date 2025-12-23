Shader "Custom/FlowerWave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude ("Wave Amplitude", float) = 0.1
        _Speed ("Wave Speed", float) = 2
        _PhaseOffset ("Phase Offset", float) = 0   // NEW
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Amplitude;
            float _Speed;
            float _PhaseOffset;   // NEW

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                
                // Only sway the **top two vertices** (uv.y == 1)
                float swayMask = saturate((v.uv.y - 0.8) * 5.0);

                // Add per-instance phase offset
                float t = _Time.y * _Speed + _PhaseOffset;

                float sway = sin(t) * _Amplitude;

                // Move horizontally only
                v.vertex.x += sway * swayMask;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }

            ENDCG
        }
    }
}
