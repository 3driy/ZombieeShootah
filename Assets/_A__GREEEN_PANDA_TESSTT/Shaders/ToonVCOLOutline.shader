Shader "Unlit/ToonVCOL"
{
    Properties
    {
         _LightContrast ("Light contrast", Range (0,2)) = 1
        _ShadowTex ("Texture", 2D) = "white" {}
         //_OutColor ("Outline color", Color) = (0,0,0,1)
         _OutWidth ("Outline Width", Range(0,0.2)) = 1 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                //float2 uv : TEXCOORD0;
                fixed4 vcol : COLOR0;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                fixed4 vcol : COLOR0;
                //fixed nDotL : COLOR1;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            //sampler2D _ShadowTex;
            ////float4 _MainTex_ST;
            //half  _LightContrast;
            //fixed4 _OutColor;
            half _OutWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vcol = v.vcol;
                float3 normal = UnityObjectToWorldNormal(v.normal);
               float4 wPos = mul(unity_ObjectToWorld, v.vertex);
			//float3 wNormal = UnityObjectToWorldNormal(v.normal);
			//o.camDist = clamp(length(wPos.xz - _WorldSpaceCameraPos.xz) * _FogRange - _FogOffset, 0, 1);
			    wPos.xyz += normal * _OutWidth;
			    o.vertex = mul(UNITY_MATRIX_VP, wPos);
                //o.nDotL = clamp((dot(_WorldSpaceLightPos0.xyz, normal) * _LightContrast + 1) * 0.5,0,1) ;
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = i.vcol *  tex2D(_ShadowTex,fixed2(i.nDotL,0));
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return i.vcol + 0.1;
            }
            ENDCG
        }


             Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                //float2 uv : TEXCOORD0;
                fixed4 vcol : COLOR0;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                fixed4 vcol : COLOR0;
                fixed nDotL : COLOR1;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _ShadowTex;
            //float4 _MainTex_ST;
            half  _LightContrast;
            //fixed4 _OutColor;
            //half _OutWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vcol = v.vcol;
                float3 normal = UnityObjectToWorldNormal(v.normal);
                o.nDotL = clamp((dot(_WorldSpaceLightPos0.xyz, normal) * _LightContrast + 1) * 0.5,0,1) ;
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = i.vcol *  tex2D(_ShadowTex,fixed2(i.nDotL,0));
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
