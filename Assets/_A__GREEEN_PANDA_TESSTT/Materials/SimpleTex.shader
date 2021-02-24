Shader "Unlit/SimpleTex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
         _LightContrast ("Light contrast", Range (0,2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                //fixed4 vcol : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //fixed4 vcol : COLOR0;
                fixed nDotL : COLOR1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            //float4 _MainTex_ST;
            half  _LightContrast;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                //o.vcol = v.vcol;
                float3 normal = UnityObjectToWorldNormal(v.normal);
                o.nDotL = clamp((dot(_WorldSpaceLightPos0.xyz, normal) * _LightContrast + 1) * 0.5,0,1) ;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.nDotL;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
