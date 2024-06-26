Shader "Custom/PortalMask"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _MaskID("Mask ID", Float) = 1
        _RenderTime("Render Time", Float) = 1999
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        Pass
        {
            Stencil
            {
                Ref 1           // 스텐실 값 1
                Comp Equal      // 스텐실 값이 1인 경우에만 패스
                Pass Keep       // 패스 시 스텐실 값 유지
                Fail Keep       // 실패 시 스텐실 값 유지
                ZFail Keep      // ZFail 시 스텐실 값 유지
            }   

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float4 screenPos : TEXCOORD0;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.screenPos = ComputeScreenPos(o.vertex);
                    return o;
                }

                uniform sampler2D _MainTex;

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.screenPos.xy / i.screenPos.w;
                    fixed4 col = tex2D(_MainTex, uv);
                    return col;
                }
            ENDCG
        }
    }
}
