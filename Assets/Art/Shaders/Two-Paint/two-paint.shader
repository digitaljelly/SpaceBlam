// This shader allows you to set two custom colours to your Albedo map using the Albedo map's alpha channel
// The two Threshold sliders determine what alpha values (0-255) get mapped to a color:
//		So a threshold for color 2 of 128 will make all alpha values greather than 128 
//		blend with color 2 (increasing the blend)
// You can also change the blend modes and blend colors over one-another
// As this uses an alpha channel, it should be no additonal draw calls! Bonus!
// Extras map is optional, provides you with 4 channels for Smoothness, Metalness, Emmisive & Alpha transparency


Shader "MattMcDonald/Two-Paint" {
	Properties {
		_Color ("Albedo Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB), Two-Tone Mask (A)", 2D) = "white" {}
		[Normal] _Normal ("Normal Map (RGB) ", 2D) = "bump" {}
		_Extras ("Smoothness (R), Metallic (G), Emission (B)", 2D) = "black" {}
		_EmmisiveColor("Emmisive Color", Color) = (1,0,0,1)
		_CustomColor1 ("Paint Color 1", Color) = (1,0,0,1)
		_CustomColor1Threshold ("Paint Color 1 Range", Range(0,256)) = 127
		[KeywordEnum(Replace, Add, Multiply, Divide, Subtract)] _CustomColor1Overlay("Color 1 blend mode", Float) = 0
		_CustomColor2 ("Paint Color 2", Color) = (0,1,0,1)
		_CustomColor2Threshold ("Paint Color 2 Range", Range(0,256)) = 129
		[KeywordEnum(Replace, Add, Multiply, Divide,  Subtract)] _CustomColor2Overlay("Color 2 blend mode", Float) = 0
		_Occlusion ("Occlusion", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Normal;
		sampler2D _Extras;
		sampler2D _Occlusion;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Normal;
			float2 uv_Extras;
			float2 uv_Occlusion;
		};

		fixed4 _Color;
		fixed4 _CustomColor1;
		fixed4 _CustomColor2;
		fixed4 _EmmisiveColor;
		float _CustomColor1Threshold;
		float _CustomColor1Overlay;
		float _CustomColor2Threshold;
		float _CustomColor2Overlay;

		float4 NoBlend ( float4 _originalColor, float4 _newColor ){
			float4 _outColor;
			float alpha = _originalColor.a * _newColor.a;
			float3 colorToAdd = alpha * _newColor.rgb; // alpha is, say 0.33, color is 255 red, new color should be 85
			float3 colorToRemove = (1 - alpha) * _originalColor.rgb; // reverse alpha is 0.67, colour is blue, new colour should be 171
			_outColor.rgb = colorToAdd + colorToRemove  ; // blend of the two
			//_outColor.rgb = float3(_originalColor.a,_originalColor.a,_originalColor.a);
			return _outColor;
		}

		float4 AddColors ( float4 _originalColor, float4 _newColor ){
			float4 _outColor;
			 //blend amount
			float3 colorToAdd = _newColor.a * _newColor.rgb;
			_outColor.rgb = _originalColor.rgb + (colorToAdd * _originalColor.a);
			return _outColor;
		}

		float4 SubtractColors ( float4 _originalColor, float4 _newColor  ){
			float4 _outColor;
			float3 colorToSubtract = _newColor.a * _newColor.rgb;
			_outColor.rgb = _originalColor.rgb - (colorToSubtract * _originalColor.a);
			return _outColor;
		}

		float4 MultiplyColors ( float4 _originalColor, float4 _newColor){
			float4 _outColor;
			float alpha = _originalColor.a * _newColor.a;
			float4 colorToMultiply = (alpha * _newColor);
			_outColor.rgb = ((1 - alpha) * _originalColor.rgb) + (_originalColor * colorToMultiply).rgb;
			return _outColor;
		}

		float4 DivideColors ( float4 _originalColor, float4 _newColor ){
			float4 _outColor;
			float alpha = _originalColor.a * _newColor.a;
			float3 denom = _newColor.rgb;
			float3 nom =  _originalColor.rgb;

			float3 divValue = nom / denom   ;
			
			_outColor.rgb = (divValue.rgb * alpha) + ((1 - alpha) * _originalColor.rgb );
			if(_newColor.r == 0) _outColor.r = 1;
			if(_newColor.g == 0) _outColor.g = 1;
			if(_newColor.b == 0) _outColor.b = 1;
			return _outColor;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			// get the alpha, get values < 128
			float alpha = c.a;
			float _range1 = _CustomColor1Threshold / 256.0;
			float _range2 = _CustomColor2Threshold / 256.0;
			float4 newColor;
			if(alpha < _range1){
			
				// 90.8... 0.3546875
				// 1 / ^ = 2.8193832599...
				// value = 0.3... * ^ = 0.8458149...
				// value = 1 - value
				float multiplier = (1.0 / 256.0) * _CustomColor1Threshold;
				float sum = 1.0 / multiplier;
				float alphaVal = sum * alpha;
				c.a = 1.0 - alphaVal;
				switch(_CustomColor1Overlay){
					case 0 : newColor.rgb = NoBlend(c, _CustomColor1).rgb; break; // black to color
					case 1 : newColor.rgb = AddColors( c, _CustomColor1).rgb; break; // Add to albedo
					case 2 : newColor.rgb = MultiplyColors( c, _CustomColor1).rgb; break;
					case 3 : newColor.rgb = DivideColors( c, _CustomColor1).rgb; break;
					case 4 : newColor.rgb = SubtractColors( c, _CustomColor1).rgb; break;
				}
				o.Albedo = newColor.rgb;
			}
			if(alpha > _range2){
				// Don't invert 
				c.a = c.a - 0.5;
				c.a = c.a * 2;
				switch(_CustomColor2Overlay){
					case 0 : newColor.rgb = NoBlend(c, _CustomColor2).rgb; break; // black to color
					case 1 : newColor.rgb = AddColors( c, _CustomColor2).rgb; break; // Add to albedo
					case 2 : newColor.rgb = MultiplyColors( c, _CustomColor2).rgb; break;
					case 3 : newColor.rgb = DivideColors( c, _CustomColor2).rgb; break;
					case 4 : newColor.rgb = SubtractColors( c, _CustomColor2).rgb; break;
				}
				o.Albedo = newColor.rgb;
			}
			o.Normal = UnpackNormal (tex2D (_Normal, IN.uv_Normal));
			// Metallic and smoothness come from average RGB values of the provided textures (these should be greyscale)
			fixed4 extasColor = tex2D (_Extras, IN.uv_Extras);
			o.Smoothness = extasColor.r;
			o.Metallic = extasColor.g;
			o.Emission = extasColor.b * _EmmisiveColor.rgb * _EmmisiveColor.a;

			fixed4 occlusionColor = tex2D (_Occlusion, IN.uv_Occlusion);
			o.Occlusion = occlusionColor.rgb;
			o.Alpha = 1.0;//extasColor.a;
		}



		ENDCG
	}
	FallBack "Diffuse"
}
