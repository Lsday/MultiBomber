Shader "Custom/Boxes"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex("Noise (RGB)", 2D) = "white" {}
        _BurnTex("Burn (RGB)", 2D) = "white" {}
        _DiffuseMin("Diffuse Min", Range(0,1)) = 0
        _DiffuseMax("Diffuse Max", Range(0,2)) = 1

        [PerRendererData] _RandomValue("RandomValue",Float) = 1
        [PerRendererData] _NoiseValue("NoiseValue",Float) = 0
        //_NoiseValue("NoiseValue", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }
        LOD 200

        Cull off
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        AlphaTest Greater 0

        CGPROGRAM
        #pragma multi_compile_instancing
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf CelShadingForward fullforwardshadows alphatest:_Cutoff

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        sampler2D _BurnTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _DiffuseMin;
        half _DiffuseMax;

        //half _NoiseValue;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
            UNITY_DEFINE_INSTANCED_PROP(float, _RandomValue)
            UNITY_DEFINE_INSTANCED_PROP(float, _NoiseValue)
        UNITY_INSTANCING_BUFFER_END(Props)
        
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
            float4 randomValue = UNITY_ACCESS_INSTANCED_PROP(Props, _RandomValue);
            float noiseValue = UNITY_ACCESS_INSTANCED_PROP(Props, _NoiseValue);
            //float noiseValue = _NoiseValue;
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb * randomValue;

            o.Alpha = c.a;
            

            if (noiseValue > 0) {
                fixed4 noise = tex2D(_NoiseTex, IN.uv_MainTex);
                
                o.Alpha = noise.r > noiseValue ? 1 : 0;

                fixed4 burn = tex2D(_BurnTex, float2((noise.r - noiseValue) / noise.r, 0));
                
                o.Emission = noise.r > noiseValue + 0.5 ? 0 : burn * 4;

                if (o.Alpha == 0) discard;
            }

            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
