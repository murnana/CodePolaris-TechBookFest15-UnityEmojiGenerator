// Copyright 2023 murnana.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#ifndef __TEXT_MESH_PRO__CUSTOM__ADDITIONAL_BLUE__INCLUDED__
#define __TEXT_MESH_PRO__CUSTOM__ADDITIONAL_BLUE__INCLUDED__
#include "UnityCG.cginc"
#include "Assets/TextMesh Pro/Shaders/TMPro_Properties.cginc"
#include "Assets/TextMesh Pro/Shaders/TMPro.cginc"
#include "Comon.cginc"

struct vertex_t
{
    UNITY_VERTEX_INPUT_INSTANCE_ID
    float4 position : POSITION;
    float3 normal : NORMAL;
    fixed4 color : COLOR;
    float2 texcoord0 : TEXCOORD0;
    float2 texcoord1 : TEXCOORD1;
};


struct pixel_t
{
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
    float4 position : SV_POSITION;
    fixed4 color : COLOR;
    float2 atlas : TEXCOORD0; // Atlas
    float4 param : TEXCOORD1; // alphaClip, scale, bias, weight
    float4 mask : TEXCOORD2; // Position in object space(xy), pixel Size(zw)
    float3 viewDir : TEXCOORD3;

    #if (UNDERLAY_ON || UNDERLAY_INNER)
			float4	texcoord2		: TEXCOORD4;		// u,v, scale, bias
			fixed4	underlayColor	: COLOR1;
    #endif
    float4 textures : TEXCOORD5;
};

// Used by Unity internally to handle Texture Tiling and Offset.
float4 _FaceTex_ST;
float4 _OutlineTex_ST;

float _EffectAlpha;
float _EffectOffset;
float _EffectRotateZ;

pixel_t VertShader(vertex_t input)
{
    pixel_t output;

    UNITY_INITIALIZE_OUTPUT(pixel_t, output);
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    const float bold = step(input.texcoord1.y, 0);

    float4 vert = input.position;
    vert.x += _VertexOffsetX;
    vert.y += _VertexOffsetY;

    float4 vPosition = UnityObjectToClipPos(
        ShiftMoveTo(_EffectOffset, UNITY_TWO_PI/3 + _EffectRotateZ, input.position)
    );

    float2 pixelSize = vPosition.w;
    pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
    float scale = rsqrt(dot(pixelSize, pixelSize));
    scale *= abs(input.texcoord1.y) * _GradientScale * (_Sharpness + 1);
    if (UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale,
                                                abs(dot(UnityObjectToWorldNormal(input.normal.xyz),
                                                        normalize(WorldSpaceViewDir(vert)))));

    float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
    weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

    float bias = .5 - weight + .5 / scale;

    float alphaClip = 1.0 - _OutlineWidth * _ScaleRatioA - _OutlineSoftness * _ScaleRatioA;

    #if GLOW_ON
			alphaClip = min(alphaClip, 1.0 - _GlowOffset * _ScaleRatioB - _GlowOuter * _ScaleRatioB);
    #endif

    alphaClip = alphaClip / 2.0 - .5 / scale - weight;

    #if (UNDERLAY_ON || UNDERLAY_INNER)
			float4 underlayColor = _UnderlayColor;
			underlayColor.rgb *= underlayColor.a;

			float bScale = scale;
			bScale /= 1 + ((_UnderlaySoftness*_ScaleRatioC) * bScale);
			float bBias = (0.5 - weight) * bScale - 0.5 - ((_UnderlayDilate * _ScaleRatioC) * 0.5 * bScale);

			float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
			float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
			float2 bOffset = float2(x, y);
    #endif

    // Generate UV for the Masking Texture
    float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
    float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

    // Support for texture tiling and offset
    float2 textureUV = UnpackUV(input.texcoord1.x);
    float2 faceUV = TRANSFORM_TEX(textureUV, _FaceTex);
    float2 outlineUV = TRANSFORM_TEX(textureUV, _OutlineTex);


    output.position = vPosition;
    output.color = input.color;
    output.atlas = input.texcoord0;
    output.param = float4(alphaClip, scale, bias, weight);
    output.mask = half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw,
                        0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy));
    output.viewDir = mul((float3x3)_EnvMatrix, _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, vert).xyz);
    #if (UNDERLAY_ON || UNDERLAY_INNER)
			output.texcoord2 = float4(input.texcoord0 + bOffset, bScale, bBias);
			output.underlayColor =	underlayColor;
    #endif
    output.textures = float4(faceUV, outlineUV);

    return output;
}


fixed4 PixShader(pixel_t input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);

    const float c = tex2D(_MainTex, input.atlas).a;

    #ifndef UNDERLAY_ON
    clip(c - input.param.x);
    #endif

    float scale = input.param.y;
    const float bias = input.param.z;
    float weight = input.param.w;
    float sd = (bias - c) * scale;

    float outline = _OutlineWidth * _ScaleRatioA * scale;
    const float softness = _OutlineSoftness * _ScaleRatioA * scale;

    half4 faceColor = _FaceColor;
    half4 outlineColor = _OutlineColor;

    faceColor.rgb *= input.color.rgb;

    faceColor *= tex2D(_FaceTex, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y);
    outlineColor *= tex2D(_OutlineTex, input.textures.zw + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y);

    faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);

    #if BEVEL_ON
			float3 dxy = float3(0.5 / _TextureWidth, 0.5 / _TextureHeight, 0);
			float3 n = GetSurfaceNormal(input.atlas, weight, dxy);

			float3 bump = UnpackNormal(tex2D(_BumpMap, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y)).xyz;
			bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
			n = normalize(n- bump);

			float3 light = normalize(float3(sin(_LightAngle), cos(_LightAngle), -1.0));

			float3 col = GetSpecular(n, light);
			faceColor.rgb += col*faceColor.a;
			faceColor.rgb *= 1-(dot(n, light)*_Diffuse);
			faceColor.rgb *= lerp(_Ambient, 1, n.z*n.z);

			fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDir, -n));
			faceColor.rgb += reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
    #endif

    #if UNDERLAY_ON
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * saturate(d - input.texcoord2.w) * (1 - faceColor.a);
    #endif

    #if UNDERLAY_INNER
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * (1 - saturate(d - input.texcoord2.w)) * saturate(1 - sd) * (1 - faceColor.a);
    #endif

    #if GLOW_ON
			float4 glowColor = GetGlowColor(sd, scale);
			faceColor.rgb += glowColor.rgb * glowColor.a;
    #endif

    // Alternative implementation to UnityGet2DClipping with support for softness.
    #if UNITY_UI_CLIP_RECT
			half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
			faceColor *= m.x * m.y;
    #endif

    #if UNITY_UI_ALPHACLIP
			clip(faceColor.a - 0.001);
    #endif

    return fixed4(0, 0, faceColor.b, faceColor.a) * input.color.a * _EffectAlpha;
}
#endif
