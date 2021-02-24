Shader "Unlit/AnimUVuiShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Divider("Rows and lines", int) = 1
        _Speed("Speed", Range(0,300)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

             sampler2D _MainTex;
            float4 _MainTex_ST;
            uint _Divider;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float mmm = (1 / _Divider);
                //o.uv = v.uv * multiplier;
                float timer = floor(_Time * _Speed) / _Divider; 
                //float timer = floor(_Time * _Speed) * mmm; 
                float timerY = floor(_Time * (_Speed / _Divider))/ _Divider; 
                o.uv = v.uv / _Divider;
                o.uv.x += timer;
                o.uv.y -= timerY;
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
