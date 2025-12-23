Shader "Custom/FlowerWaveBendy"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        // How much the top can bend sideways
        _BendStrength ("Bend Strength", float) = 0.15
        
        // How fast the wind changes over time
        _WindSpeed ("Wind Speed", float) = 1.5
        
        // How "tight" or "broad" the noise is in world space
        _NoiseScale ("Noise Scale", float) = 0.5
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

            float _BendStrength;
            float _WindSpeed;
            float _NoiseScale;

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

            // simple smooth-ish noise using sin/cos on world position
            float Noise2D(float2 p)
            {
                // basic smooth noise in [-1, 1]
                return sin(p.x) * cos(p.y);
            }

            v2f vert (appdata v)
            {
                v2f o;

                // Height along the sprite:
                // uv.y = 0 → bottom (stem base)
                // uv.y = 1 → top (flower head)
                float height = saturate(v.uv.y);

                // Make bending stronger at the top, weaker near bottom
                // squaring exaggerates the top motion
                float bendFactor = height * height;

                // Get world position so nearby flowers can have slightly different wind
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Animate noise in time & world space
                float2 noiseUV = worldPos.xz * _NoiseScale + _Time.y * _WindSpeed;

                // Noise in [-1, 1]
                float windNoise = Noise2D(noiseUV);

                // Final bend amount for this vertex
                float bend = windNoise * _BendStrength * bendFactor;

                // Bend sideways (along local X). 
                // If your sprite is oriented differently, change which axis you use.
                v.vertex.x += bend;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }

            ENDCG
        }
    }
}
