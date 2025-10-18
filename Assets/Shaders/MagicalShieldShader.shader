Shader "Custom/MagicalShield"
{
    Properties
    {
        _Color ("Main Color", Color) = (0.2, 0.8, 1.0, 0.5)
        _RimColor ("Rim Glow Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.5, 8)) = 2.0
        _EdgeIntensity ("Edge Intensity", Range(0, 5)) = 1.0
        _MainTex ("Main Texture (optional)", 2D) = "white" {}
        _NoiseTex ("Noise Texture (optional)", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.2
        _NoiseSpeed ("Noise Speed", Range(0,5)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        ZWrite Off

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
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;  // normal in world or view space
                float3 viewDir : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            fixed4 _Color;
            fixed4 _RimColor;
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _RimPower;
            float _EdgeIntensity;
            float _NoiseStrength;
            float _NoiseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Compute normal and view direction in world space
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.normalDir = normalize(worldNormal);

                float3 viewPos = _WorldSpaceCameraPos - worldPos;
                o.viewDir = normalize(viewPos);

                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base color
                fixed4 col = _Color;

                // Sample main texture optionally
                fixed4 tex = tex2D(_MainTex, i.uv);
                col.rgb *= tex.rgb;

                // Rim / edge glow factor via Fresnel (view angle)
                float ndotv = saturate(dot(i.normalDir, i.viewDir));
                float rim = pow(1.0 - ndotv, _RimPower);

                // You can amplify rim
                rim *= _EdgeIntensity;

                // Rim color mixing
                col.rgb += _RimColor.rgb * rim;

                // Optionally add noise (animated) to color or alpha
                #if defined(_NoiseTex)
                float2 noiseUV = i.uv * 2.0 + _Time.y * _NoiseSpeed;
                float n = tex2D(_NoiseTex, noiseUV).r;  // one channel noise
                float noiseFactor = lerp(1.0 - _NoiseStrength, 1.0 + _NoiseStrength, n);
                col.rgb *= noiseFactor;
                #endif

                // Transparency / alpha
                col.a = _Color.a;

                return col;
            }
            ENDCG
        }
    }
}
