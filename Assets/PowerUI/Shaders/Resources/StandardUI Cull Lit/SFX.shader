Shader "PowerUI/StandardUI Cull Lit/SFX" {
	Properties {
		_Font ("Font Texture", 2D) = "white" {}
		_Atlas  ("Graphical Atlas", 2D) = "white" {}
	}
	
	SubShader {
		
		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		Cull Back
		
		CGPROGRAM
		
		#pragma surface surf Lambert alpha:blend vertex:vert
		
		struct Input {
			float2 uv_Atlas;
			float2 uv2_Font;
			fixed4 color : COLOR;
			half2 tangent;
		};
		
		sampler2D _Font;
		sampler2D _Atlas;
		
		void vert(inout appdata_full i, out Input o){
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.tangent=i.tangent.xy;
			
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			
			fixed4 col = IN.color;
			
			if(IN.uv_Atlas.y<=1){
				col *= tex2D(_Atlas, IN.uv_Atlas);
			}
			
			if(IN.uv2_Font.y<=1){
				col.a *= smoothstep(IN.tangent.x,IN.tangent.y,tex2D(_Font,IN.uv2_Font).a);
			}
			
			o.Albedo = col.rgb;
			o.Alpha=col.a;
			
		}
		
		ENDCG
	}
	
	Fallback "Diffuse"
}