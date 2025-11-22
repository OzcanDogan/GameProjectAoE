Shader "Outlined/Silhouetted Diffuse"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _Outline ("Outline width", Range (.002, 0.03)) = .005
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }

            Cull Front
            ZWrite Off
            ZTest Always
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Outline;
            float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal) * _Outline;
                o.pos = UnityObjectToClipPos(v.vertex + float4(norm, 0));
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                return _OutlineColor;
            }
            ENDCG
        }

        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            Material { Diffuse (1,1,1,1) }
            Lighting On

            SetTexture [_MainTex] { combine texture * primary }
        }
    }
}
