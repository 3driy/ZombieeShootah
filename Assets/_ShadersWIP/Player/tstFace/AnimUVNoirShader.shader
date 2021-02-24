Shader "Unlit/AnimUVNoirShader"
{
   Properties
    {
        [NoScaleOffset]
        _MainTex ("Texture", 2D) = "white" {}
        _EyesTex ("eyes Texture", 2D) = "white" {}
        _Frensel ("Frensel mul", Range(0,5)) = 1
        _FrenselInt ("Frensel intensity", Range(0,5)) = 1
        _LightContrast("Light contrast", Range(0,2)) = 1
        _Divider("Rows and lines", int) = 1
        _Speed("Speed", Range(0,300)) = 1
        _EyesOffset("eyes offset", Vector) = (0,0,0,1)
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
                float2 uvEyes : TEXCOORD1;
                fixed frensel : COLOR1;
                fixed4 vcol : COLOR0;
                fixed nDotL : COLOR2;
                UNITY_FOG_COORDS(3)
                float4 vertex : SV_POSITION;
            };

            float _Frensel;
            float _FrenselInt;
            //float _Contrast;
            float _LightContrast;
            float2 _EyesOffset;
            sampler2D _MainTex, _EyesTex;
            int _Divider;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vcol = v.vcol;
                o.vertex = UnityObjectToClipPos(v.vertex);
               float3 wPos = mul(unity_ObjectToWorld,v.vertex);
               float3 normal = UnityObjectToWorldNormal(v.normal);
               float3 view = normalize(_WorldSpaceCameraPos.xyz - wPos);
               o.frensel =  (1.2 - dot(view, normal) * _FrenselInt) * _Frensel;
               o.nDotL = saturate( dot(_WorldSpaceLightPos0.xyz, normal) * _LightContrast);
               float timer = floor(_Time * _Speed) / _Divider; 
               float timerY = floor(_Time * (_Speed / _Divider))/ _Divider; 
               o.uv = v.uv / _Divider;
               //o.uv = v.uv;
               o.uv.x += timer;
               o.uv.y -= timerY;
               o.uvEyes = v.uv + _EyesOffset;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 eyes = tex2D(_EyesTex,i.uvEyes);
                fixed2 txtr = tex2D(_MainTex,i.uv).rg;
                fixed4 col = i.vcol * i.nDotL + i.frensel + fixed4(txtr.r,txtr.r,txtr.r,0);
                fixed4 fin = fixed4(lerp(col.rgb,eyes.rgb,eyes.a * txtr.g),1);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, fin);
                return fin;
            }
            ENDCG
        }
    }
}
