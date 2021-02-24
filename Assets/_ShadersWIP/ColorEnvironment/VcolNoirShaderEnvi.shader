Shader "UnlitVcolNoirShaderEnvi"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
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
                //float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                fixed4 vcol : COLOR0;
                fixed frensel : COLOR1;
                fixed nDotL : COLOR2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float _Frensel;
            fixed4 _Color;
            float _FrenselInt;
            //float _Contrast;
            float _LightContrast;
            //sampler2D _MainTex;
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
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vcol = v.vcol;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = i.vcol * _Color * i.nDotL + i.frensel;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
