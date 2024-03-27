Shader "Custom/GlassShader"
{
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0,1)) = 1.0
        _Threshold ("Cutout threshold", Range(0,1)) = 0.1
        _Softness ("Cutout softness", Range(0,0.5)) = 0.0
        _EmissionColor ("Emission Color", Color) = (0,0,0,0) 
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Lambert alpha fullforwardshadows
 
        sampler2D _MainTex;
        float4 _Color;
        float _Alpha;
        float _Threshold;
        float _Softness;
        fixed4 _EmissionColor; 
 
        struct Input {
            float2 uv_MainTex;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = smoothstep(_Threshold, _Threshold + _Softness,
               0.333 * (c.r + c.g + c.b)) * _Alpha;
            o.Emission = _EmissionColor.rgb; 
        }
        ENDCG
    }
    FallBack "Diffuse"
}
