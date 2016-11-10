Shader "PowerUI/StandardUI Lit/Isolated" {
	Properties {
		_Sprite  ("Graphical Sprite", 2D) = "white" {}
	}
	
	SubShader {

		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		Cull Off
		
		CGPROGRAM
		
		#pragma surface surf Lambert alpha:blend
		
		struct Input {
			float2 uv_Sprite;
			fixed4 color : COLOR;
		};
		
		sampler2D _Sprite;
		
		void surf (Input IN, inout SurfaceOutput o) {
			
			fixed4 col = IN.color * tex2D(_Sprite, IN.uv_Sprite);
			
			o.Albedo = col.rgb;
			o.Alpha=col.a;
			
		}
		
		ENDCG
	}
	
	Fallback "Diffuse"
}
