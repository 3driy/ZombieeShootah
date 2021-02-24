Shader "Unlit/AlphaFiolage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
		  _Frensel ("Frensel mul", Range(0,5)) = 1
        _FrenselInt ("Frensel intensity", Range(0,5)) = 1
        //_Contrast ("Frensel contrast", Range(0,3)) = 1
        _LightContrast("Light contrast", Range(0,2)) = 1
		_Clip ("Clip Alpha", Range(0,1)) = 0
		_Speed("Speed",Range(0,3)) = 1
		_Amp("Amplitude",Range(0,3)) = 1
		_SinAmp("Scale",Range(0,3)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {

            Cull Off
			//ZWrite On
			//ZTest Less
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
                fixed4 vcol : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 lightCol : COLOR1;
                //fixed4 vcol : COLOR0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			//sampler2D _Gradient;
			half _Frensel;
             float _FrenselInt;
            //float _Contrast;
            float _LightContrast;
			fixed4 _Color;
			//fixed4 _FogColor;
			//half _FogOffset;
			//half _FogRange;
			fixed _Clip;
			fixed _Amp;
			fixed _SinAmp;
			half _Speed;


            v2f vert (appdata v)
            {
                v2f o;
                half4 wpos = mul(unity_ObjectToWorld, v.vertex);
				//half wobbler = (tex2Dlod(_MainTex,float4((wpos.x  + _Time.y * _Speed)* _SinAmp,wpos.z,0,0)).g * 2) - 1;
				//half wobbler = _SinTime.y;
				wpos.xz += (sin(_Time.y * _Speed + (wpos.y + wpos.x) * _SinAmp )) * v.vcol.r * _Amp ;
				o.vertex = mul(UNITY_MATRIX_VP,wpos);
                 float3 normal = UnityObjectToWorldNormal(v.normal);
               float3 view = normalize(_WorldSpaceCameraPos.xyz - wpos);
               o.lightCol.x =  (1.2 - dot(view, normal) * _FrenselInt) * _Frensel;
               //float3 lightDir = 
               o.lightCol.y = saturate( dot(_WorldSpaceLightPos0.xyz, normal) * _LightContrast);
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed txtr = tex2D(_MainTex, i.uv).a;
                fixed4 col = txtr * _Color * i.lightCol.y + i.lightCol.x;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                clip(txtr - _Clip);
                return col;
            }
            ENDCG
        }
    }
}
