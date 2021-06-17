Shader "Custom/Items_V2"
{
    //https://www.ronja-tutorials.com/2020/02/11/material-property-blocks.html
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex("Noise (RGB)", 2D) = "white" {}
        _BurnTex("Burn (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [PerRendererData] _SpriteUV("SpriteUV",Vector) = (0, 0, 0.5, 0.5)
        [PerRendererData] _NoiseValue("NoiseValue",Float) = 0
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
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        sampler2D _BurnTex;
        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
            UNITY_DEFINE_INSTANCED_PROP(float4, _SpriteUV)
            UNITY_DEFINE_INSTANCED_PROP(float, _NoiseValue)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 spriteUV = UNITY_ACCESS_INSTANCED_PROP(Props, _SpriteUV);
            float noiseValue = UNITY_ACCESS_INSTANCED_PROP(Props, _NoiseValue);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex * spriteUV.zw + spriteUV.xy) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = c.rgb * c.a * 2;
            o.Alpha = 1;

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
