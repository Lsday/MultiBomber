Shader "Custom/Walls"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex("Noise (RGB)", 2D) = "white" {}
        _BurnTex("Burn (RGB)", 2D) = "white" {}
        _DiffuseMin("Diffuse Min", Range(0,1)) = 0
        _DiffuseMax("Diffuse Max", Range(0,2)) = 1

    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf CelShadingForward fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _DiffuseMin;
        half _DiffuseMax;


        fixed4 _Color;

  
        half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten) {
            half NdotL = dot(s.Normal, lightDir);
            if (NdotL <= 0.0) NdotL = 0;
            else NdotL = 1;
            NdotL = lerp(_DiffuseMin, _DiffuseMax, NdotL);
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 0.5);
            c.a = s.Alpha;
            return c;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {

            //float noiseValue = _NoiseValue;
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
