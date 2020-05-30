Shader "kodiakwhale/Stability Build Debug"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaxColor("Max Stability Color", Color) = (0,1,0,1)
		_MinColor("Max Stability Color", Color) = (1,0,0,1)
		_Stability("Stability", Range(0, 100)) = 100
		[Toggle(SHOW_STABILITY)]_ShowStability("Show Stability", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows
		#pragma shader_feature SHOW_STABILITY

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };
		
        fixed4 _Color;
		fixed4 _MaxColor;
		fixed4 _MinColor;
		half _Stability;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Albedo comes from a texture tinted by color
			#ifdef SHOW_STABILITY
				fixed4 lerpedColor = lerp(_MinColor, _MaxColor, clamp(_Stability, 0, 100) / (float)100);
				o.Albedo = lerpedColor.rgb;
			#else
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			#endif
        }
        ENDCG
    }
    FallBack "Diffuse"
}
