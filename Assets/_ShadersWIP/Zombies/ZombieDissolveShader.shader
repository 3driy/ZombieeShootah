Shader "Unlit/ZombieDissolveShader"
{
    Properties
    {
        [HideInInspector]
         _Color ("Main color", Color) = (1,1,1,1)
         _EdgeCol ("Edge color", Color) = (1,1,1,1)
        _NoiseTex ("Texture", 2D) = "white" {}
        _Scale ("Texture scale", Range(0,5)) = 1
        //_Clip ("Clip", Range(0,1)) = 1
        _Edge ("Edge", Range(0,1)) = 1
        _Frensel ("Frensel mul", Range(0,5)) = 1
        _FrenselInt ("Frensel intensity", Range(0,5)) = 1
        //_Contrast ("Frensel contrast", Range(0,3)) = 1
        _LightContrast("Light contrast", Range(0,2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {

        Tags { "RenderMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL0;
                fixed4 vcol : COLOR0;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 vcol : COLOR0;
                fixed frensel : COLOR1;
                fixed nDotL : COLOR2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            //float _Frensel, _Scale, _Edge, _Clip;
            float _Frensel, _Scale, _Edge;
            //fixed4 _Color;
            float4 _Color, _EdgeCol;
            float _FrenselInt;
            //float _Contrast;
            float _LightContrast;
            sampler2D _NoiseTex;
            //float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
               float3 wPos = mul(unity_ObjectToWorld,v.vertex);
               float3 normal = UnityObjectToWorldNormal(v.normal);
               float3 view = normalize(_WorldSpaceCameraPos.xyz - wPos);
               o.frensel =  (1.2 - dot(view, normal) * _FrenselInt) * _Frensel;
               //float3 lightDir = 
               o.nDotL = saturate( dot(_WorldSpaceLightPos0.xyz, normal) * _LightContrast);
                o.uv = wPos.xz * _Scale;
                o.vcol = v.vcol;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed noise = tex2D(_NoiseTex,i.uv).r;
                fixed4 col = i.vcol * i.nDotL + i.frensel;
                // apply fog
                fixed4 albedo = lerp(col,fixed4(1,0,0,1),i.vcol.g > 0.5);
                fixed4 fin = lerp(albedo,_EdgeCol,(noise  - _Color.r + _Edge) < 0.5);
                clip(noise - _Color.r);
                UNITY_APPLY_FOG(i.fogCoord, fin);
                return fin;
            }
            ENDCG
        }
    }
}
