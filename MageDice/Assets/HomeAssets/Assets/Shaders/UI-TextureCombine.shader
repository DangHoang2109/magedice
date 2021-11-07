// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Cosina/TextureCombine" 
  {
      Properties
      {
          _MainTex("Albedo (RGB)", 2D) = "white" {}
          _Color ("Tint", Color) = (1,1,1,1)
          _OpacityTex("Opacity (A)", 2D) = "white" {}
      }
  
      SubShader
      {
          Tags{ "Queue" = "transparent" "RenderType" = "transparent" }
          Blend SrcAlpha OneMinusSrcAlpha
          Lighting Off
  
          Pass
          {
              CGPROGRAM
              #pragma vertex vert
              #pragma fragment frag
              #include "UnityCG.cginc"
  
              sampler2D _MainTex;
              fixed4 _Color;
              sampler2D _OpacityTex;
  
              uniform float4 _MainTex_ST;
             // Needed by TRANSFORM_TEX
              uniform float4 _OpacityTex_ST;
 
              struct v2f 
              {
                  float4  pos : SV_POSITION;
                  float2  uv : TEXCOORD0;
                // "declare" a new set of uvs will be passed from the vertex to the fragment shader
                  float2  uvOpacityTex : TEXCOORD1;
              };
  
              v2f vert(appdata_base v)
              {
                  v2f o;
                  o.pos = UnityObjectToClipPos(v.vertex);
  
                  // Texture offset - GOOD
                  o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                  // "bind" your new uvs with the OpacityTex tile and offset properties you can find in the material
                  o.uvOpacityTex = TRANSFORM_TEX(v.texcoord, _OpacityTex);
                  return o;
              }
  
              half4 frag(v2f i) : COLOR
              {
                  fixed4 c = tex2D(_MainTex, i.uv);
                  // Use your OpacityTex uvs to map the texture
                  //half opacity = tex2D(_OpacityTex, i.uvOpacityTex).a;
                  fixed4 c2 = tex2D(_OpacityTex, i.uvOpacityTex);
  
                 // sorry but this seems to be garbage
                 // if (opacity == 0)
                 // {
                 //     c.a = c.a;
                 // }
                 // else
                 // {
                 //     c.a = 0;
                 // }
 
                 // multiply _MainTex and _OpacityTex alpha 
                  //c.a *= opacity;
                  c *= _Color;
                  c *= c2;
                  return c;
              }        
              ENDCG
          }
      }
  }