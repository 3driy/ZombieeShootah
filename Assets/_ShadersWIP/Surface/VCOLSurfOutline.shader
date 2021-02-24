Shader "Custom/VCOLSurfOutline"
{
    Properties
    {
        //_Color ("Color", Color) = (1,1,1,1)
          _ShadeTex ("Texture", 2D) = "white" {}
       _OutlineColor ("Outline Color", Color) = (1,1,1,1)
       _Outline ("Outline width", Range(0,0.1)) = 0.1
         _Scale ("Scale Offset", Vector) = (1,1,0,0)
           _Contrast("ShadowContrast", Range(0,5)) = 1
        //_MainTex ("Albedo (RGB)", 2D) = "white" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {

          Tags { "RenderType"="Opaque" }
        LOD 200
          Pass
        {

            Cull Front
            //Blend DstColor Zero

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
             
             };

            struct v2f
            {
               float4 vertex : SV_POSITION;
            };

          
            fixed4 _OutlineColor;
            fixed _Outline;
            
            v2f vert (appdata v)
            {
                v2f o;
                float3 normal = mul(unity_ObjectToWorld, v.normal);
                float4 wPos =  mul(unity_ObjectToWorld, v.vertex);
                wPos.xyz += normal * _Outline;
                o.vertex = mul(UNITY_MATRIX_VP, wPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
               
                return _OutlineColor;
            }
            ENDCG
            }




        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        //#if defined(LIGHTMAP_ON)
         //#pragma surface surf Comic fullforwardshadows
	    //#else
        #pragma surface surfN Lambert fullforwardshadows
        //#endif
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            fixed4 screenPos;
            fixed4 color : COLOR0;
            fixed4 lightUV : TEXCOORD1;
        };

        //half _Glossiness;
        //half _Metallic;
        //fixed4 _Color;
           fixed4 _OutlineColor;
         fixed4 _Scale;
             float _Contrast;
          sampler2D _ShadeTex;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {

            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            fixed c = tex2D (_ShadeTex,screenUV * _Scale.xy).r;
            o.Albedo = IN.color;

            o.Alpha = c;
        }
        
        void surfN (Input IN, inout SurfaceOutput o)
        {

            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            fixed c = tex2D (_ShadeTex,screenUV * _Scale.xy).r;
            half2 uvs = IN.lightUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            fixed3 lightMap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, uvs));
             float shadow = smoothstep(0,(1 - c )* _Contrast,c * lightMap.r * _Contrast);

            //o.Albedo =lerp(_OutlineColor * IN.color, IN.color + lightMap, shadow);
            //o.Albedo = IN.color + lightMap * shadow;
            o.Albedo = IN.color + lightMap * lightMap * shadow;
            o.Alpha = c;
        }

        float4 LightingComic(SurfaceOutput s, float3 lightDir, half3 viewDir, float shadowAttenuation){

            float towardsLight = dot(s.Normal, lightDir);

             float shadow = smoothstep(0, (1 - s.Alpha) * _Contrast , towardsLight * shadowAttenuation);

            float lightIntensity = towardsLight * shadow ;
     
            float4 color;

            color.rgb = lerp(_OutlineColor, s.Albedo, lightIntensity) * _LightColor0.rgb;
            color.a = 1;
            return color;
        }



        ENDCG
    }
    FallBack "Diffuse"
}
