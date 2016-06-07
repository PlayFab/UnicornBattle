Shader "Cartoon FX/Demo/Water Wiggle" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_WiggleTex ("Base (RGB)", 2D) = "white" {}
	_WiggleStrength ("Wiggle Strength", Range (0.01, 0.1)) = 0.01
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
sampler2D _WiggleTex;
fixed4 _Color;
float _WiggleStrength;

struct Input
{
	float2 uv_MainTex;
	float2 uv_WiggleTex;
};

void surf (Input IN, inout SurfaceOutput o)
{
	float2 tc2 = IN.uv_WiggleTex;
	tc2.x -= _SinTime;
	tc2.y += _CosTime;
	float4 wiggle = tex2D(_WiggleTex, tc2);
	
	IN.uv_MainTex.x -= wiggle.r * _WiggleStrength;
	IN.uv_MainTex.y += wiggle.b * _WiggleStrength*1.5;
	
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "VertexLit"
}
