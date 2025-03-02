Shader "Mobile/Transparent/Vertex Color" {
	Properties{
		_Color("Main Color", Vector) = (1,1,1,1)
		_SpecColor("Spec Color", Vector) = (1,1,1,0)
		_Emission("Emmisive Color", Vector) = (0,0,0,0)
		_Shininess("Shininess", Range(0.1, 1)) = 0.7
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}
		//DummyShaderTextExporter
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200
			CGPROGRAM
	#pragma surface surf Standard
	#pragma target 3.0

			sampler2D _MainTex;
			fixed4 _Color;
			struct Input
			{
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG
	}
}