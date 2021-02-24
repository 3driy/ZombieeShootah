Shader "Custom/VCOLSurfTransp"
{
    Properties
    {
        //_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Clip("Clip", Range(0,1)) = 0

        _Speed("Speed",Range(0,3)) = 1
		_Amp("Amplitude",Range(0,0.1)) = 1
		_SinAmp("Scale",Range(0,3)) = 1
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        //Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off
        //ZWrite Off
        //Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        //#pragma surface surf BlinnPhong vertex:vert fullforwardshadows 
        #pragma surface surf Lambert vertex:vert fullforwardshadows 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            fixed4 color : COLOR0;
        };

        half _Glossiness;
        half _Metallic;
        fixed _Clip;
          	fixed _Amp;
			fixed _SinAmp;
			half _Speed;
        //fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


         void vert(inout appdata_full v, out Input o)
         {
             UNITY_INITIALIZE_OUTPUT(Input,o);
            //v.vertex.xyz += v.normal * _Amount;
            float4 wPos = mul (unity_ObjectToWorld, v.vertex);
            wPos.xz += sin(_Time.y * _Speed + (wPos.y + wPos.x) * _SinAmp ) * _Amp ;
            //v.vertex.xy += (sin(_Time.y * _Speed + (wPos.y + wPos.x) * _SinAmp )) * _Amp ;
            v.vertex.xyz = mul(unity_WorldToObject,wPos);
      }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed c = tex2D (_MainTex, IN.uv_MainTex).a;
            //fixed4 c = ;
            o.Albedo = IN.color;
            clip(c - _Clip);
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            //o.Alpha = c;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
