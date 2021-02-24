Shader "Custom/VCOLSurfAnimFace"
{
    Properties
    {
        //_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("animation", 2D) = "white" {}
        _EyesTex ("Eyes", 2D) = "white" {}
        _EyesOffset("Eyes offset", Color) = (0,0,0,0)
          _Divider("Rows and lines", int) = 1
        _Speed("Speed", Range(0,300)) = 1
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex, _EyesTex;

        struct Input
        {
            //float2 uv_MainTex;
            //float2 uv_MainTex;
            fixed4 color : COLOR0;
            float2 eyesuv: TEXCOORD1;
            float2 faceuv: TEXCOORD2;
        };


      fixed4 _EyesOffset;
          int _Divider;
            float _Speed;
        //half _Glossiness;
        //half _Metallic;
        //fixed4 _Color;

          void vert(inout appdata_full v, out Input o)
         {
             UNITY_INITIALIZE_OUTPUT(Input,o);
            o.eyesuv = v.texcoord + _EyesOffset;
             float timer = floor(_Time * _Speed) / _Divider; 
               float timerY = floor(_Time * (_Speed / _Divider))/ _Divider; 
                float2 uves = v.texcoord / _Divider;
               //o.uv = v.uv / _Divider;

               //o.uv = v.uv;
               uves.x += timer;
               uves.y -= timerY;
               o.faceuv = uves;

      }
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.faceuv);
            fixed4 eyes = tex2D(_EyesTex, IN.eyesuv);
            //fixed4 c = ;
            //o.Albedo = c;


            o.Albedo = lerp(lerp(IN.color, IN.color * c.r, c.a),eyes * c.r,c.g);
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            //o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
