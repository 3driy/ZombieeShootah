﻿Shader "Unlit/VcolComicShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Frensel ("Frensel mul", Range(0,1)) = 1
        _FrenselInt ("Frensel intensity", Range(0,5)) = 1
        _LightContrast("Light contrast", Range(0,1)) = 1
        _RimColor("rim light color", Color) = (1,1,1,1)

    }
    SubShader
    {
        Tags {"RenderType"="Opaque"}
        LOD 100

        Pass
        {

        Tags {"LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL0;
                fixed4 vcol : COLOR0;
                float2 uv : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD1;
                fixed4 vcol : COLOR0;
                fixed frensel : COLOR1;
                fixed nDotL : COLOR2;
                UNITY_FOG_COORDS(5)
                float4 vertex : SV_POSITION;
            };

            float _Frensel;
            fixed4 _Color;
            fixed4 _RimColor;
            float _FrenselInt;
            float _LightContrast;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
               float3 wPos = mul(unity_ObjectToWorld,v.vertex);
               float3 normal = UnityObjectToWorldNormal(v.normal);
               float3 view = normalize(_WorldSpaceCameraPos.xyz - wPos);
               o.frensel = clamp((1.2 - dot(view, normal) * _FrenselInt) * _Frensel,0,1);
               o.nDotL = clamp((dot(_WorldSpaceLightPos0.xyz, normal) * _LightContrast + 1) * 0.5,0,1) ;
                o.uv = v.uv * unity_LightmapST.xy + unity_LightmapST.zw;
                o.vcol = v.vcol;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 light = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv));
                fixed4 fin = i.vcol  * i.nDotL;
                fin.rgb *= lerp(1,light,_Color.r);
                fin.rgb += light * _Color.g + i.frensel * _RimColor;
                UNITY_APPLY_FOG(i.fogCoord, fin);
                return fin;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
