Shader "PowerUI/StandardUI Cull Lit/Normal" {
	Properties {
		_Font ("Font Texture", 2D) = "white" {}
		_Atlas  ("Graphical Atlas", 2D) = "white" {}
		BottomFontAlias ("Lower Alias",Float)=0.24
		TopFontAlias ("Upper Alias",Float)=1.24
	}
	
	SubShader {
		
		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		Cull Back
		
		CGPROGRAM
		
		#pragma surface surf Lambert alpha:blend
		
		struct Input {
			float2 uv_Atlas;
			float2 uv2_Font;
			fixed4 color : COLOR;
		};
		
		sampler2D _Font;
		sampler2D _Atlas;
		uniform float TopFontAlias;
		uniform float BottomFontAlias;
		
		void surf (Input IN, inout SurfaceOutput o) {
			
			fixed4 col = IN.color;
			
			if(IN.uv_Atlas.y<=1){
				col *= tex2D(_Atlas, IN.uv_Atlas);
			}
			
			if(IN.uv2_Font.y<=1){
				col.a *= smoothstep(BottomFontAlias,TopFontAlias,tex2D(_Font,IN.uv2_Font).a);
			}
			
			o.Albedo = col.rgb;
			o.Alpha=col.a;
			
		}
		
		ENDCG
	}
	
	Fallback "Diffuse"
}