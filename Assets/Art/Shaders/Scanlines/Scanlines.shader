/*
	Overlays a moving, repeating texture on the screen, useful for scanlines!
	Use a texture that has a Wrap type of 'repeating' for the mask texture
*/
Shader "Hidden/Scanlines" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskTex ("Mask texture", 2D) = "white" {}
		_maskBlend ("Mask blending", Float) = 0.5
		_maskSize ("Mask Size", Float) = 1
		_speed ("Speed", float) = 1
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
		
			uniform sampler2D _MainTex, _MaskTex;
			fixed _maskBlend, _maskSize;
			float _speed;

			fixed4 frag (v2f_img i) : COLOR {
				
				// This provides the movement
				// --> Note: _Time.w is most performant of the _Time properties, so we'll use that
				half2 moved = i.uv + _Time.w * _speed;
				
				// This applies the mask
				fixed4 mask = tex2D(_MaskTex, moved * _maskSize);
				fixed4 base = tex2D(_MainTex, i.uv);
				return lerp(base, mask, _maskBlend);
			}
			ENDCG
		}
	}
}