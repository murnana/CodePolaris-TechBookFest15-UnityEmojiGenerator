// Copyright 2023 murnana.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Shader "TextMeshPro/Custom/Additional"
{
    Properties
    {
        _FaceTex            ("Face Texture", 2D) = "white" {}
        _FaceUVSpeedX       ("Face UV Speed X", Range(-5, 5)) = 0.0
        _FaceUVSpeedY       ("Face UV Speed Y", Range(-5, 5)) = 0.0
        [HDR]_FaceColor     ("Face Color", Color) = (1,1,1,1)
        _FaceDilate         ("Face Dilate", Range(-1,1)) = 0

        [HDR]_OutlineColor  ("Outline Color", Color) = (0,0,0,1)
        _OutlineTex         ("Outline Texture", 2D) = "white" {}
        _OutlineUVSpeedX    ("Outline UV Speed X", Range(-5, 5)) = 0.0
        _OutlineUVSpeedY    ("Outline UV Speed Y", Range(-5, 5)) = 0.0
        _OutlineWidth       ("Outline Thickness", Range(0, 1)) = 0
        _OutlineSoftness    ("Outline Softness", Range(0,1)) = 0

        _Bevel              ("Bevel", Range(0,1)) = 0.5
        _BevelOffset        ("Bevel Offset", Range(-0.5,0.5)) = 0
        _BevelWidth         ("Bevel Width", Range(-.5,0.5)) = 0
        _BevelClamp         ("Bevel Clamp", Range(0,1)) = 0
        _BevelRoundness     ("Bevel Roundness", Range(0,1)) = 0

        _LightAngle         ("Light Angle", Range(0.0, 6.2831853)) = 3.1416
        [HDR]_SpecularColor ("Specular", Color) = (1,1,1,1)
        _SpecularPower      ("Specular", Range(0,4)) = 2.0
        _Reflectivity       ("Reflectivity", Range(5.0,15.0)) = 10
        _Diffuse            ("Diffuse", Range(0,1)) = 0.5
        _Ambient            ("Ambient", Range(1,0)) = 0.5

        _BumpMap            ("Normal map", 2D) = "bump" {}
        _BumpOutline        ("Bump Outline", Range(0,1)) = 0
        _BumpFace           ("Bump Face", Range(0,1)) = 0

        _ReflectFaceColor       ("Reflection Color", Color) = (0,0,0,1)
        _ReflectOutlineColor    ("Reflection Color", Color) = (0,0,0,1)
        _Cube                   ("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
        _EnvMatrixRotation      ("Texture Rotation", vector) = (0, 0, 0, 0)


        [HDR]_UnderlayColor ("Border Color", Color) = (0,0,0, 0.5)
        _UnderlayOffsetX    ("Border OffsetX", Range(-1,1)) = 0
        _UnderlayOffsetY    ("Border OffsetY", Range(-1,1)) = 0
        _UnderlayDilate     ("Border Dilate", Range(-1,1)) = 0
        _UnderlaySoftness   ("Border Softness", Range(0,1)) = 0

        [HDR]_GlowColor     ("Color", Color) = (0, 1, 0, 0.5)
        _GlowOffset         ("Offset", Range(-1,1)) = 0
        _GlowInner          ("Inner", Range(0,1)) = 0.05
        _GlowOuter          ("Outer", Range(0,1)) = 0.05
        _GlowPower          ("Falloff", Range(1, 0)) = 0.75

        _WeightNormal       ("Weight Normal", float) = 0
        _WeightBold         ("Weight Bold", float) = 0.5

        _ShaderFlags        ("Flags", float) = 0
        _ScaleRatioA        ("Scale RatioA", float) = 1
        _ScaleRatioB        ("Scale RatioB", float) = 1
        _ScaleRatioC        ("Scale RatioC", float) = 1

        _MainTex            ("Font Atlas", 2D) = "white" {}
        _TextureWidth       ("Texture Width", float) = 512
        _TextureHeight      ("Texture Height", float) = 512
        _GradientScale      ("Gradient Scale", float) = 5.0
        _ScaleX             ("Scale X", float) = 1.0
        _ScaleY             ("Scale Y", float) = 1.0
        _PerspectiveFilter  ("Perspective Correction", Range(0, 1)) = 0.875
        _Sharpness          ("Sharpness", Range(-1,1)) = 0

        _EffectAlpha("Effect Alpha", float) = 0.5
        _EffectOffset("Effect Offset", float) = 0.1
        _EffectRotateZ("Effect Rotate", float) = 0.1

        _VertexOffsetX      ("Vertex OffsetX", float) = 0
        _VertexOffsetY      ("Vertex OffsetY", float) = 0

        _MaskCoord          ("Mask Coordinates", vector) = (0, 0, 32767, 32767)
        _ClipRect           ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
        _MaskSoftnessX      ("Mask SoftnessX", float) = 0
        _MaskSoftnessY      ("Mask SoftnessY", float) = 0

        _StencilComp        ("Stencil Comparison", Float) = 8
        _Stencil            ("Stencil ID", Float) = 0
        _StencilOp          ("Stencil Operation", Float) = 0
        _StencilWriteMask   ("Stencil Write Mask", Float) = 255
        _StencilReadMask    ("Stencil Read Mask", Float) = 255

        _CullMode           ("Cull Mode", Float) = 0
        _ColorMask          ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull [_CullMode]
        ZWrite Off
        Lighting Off
        Fog { Mode Off }
        ZTest [unity_GUIZTestMode]
        ColorMask [_ColorMask]

//        Pass
//        {
//            Name "DistanceField"
//            Tags
//            {
//                "LightMode" = "TextMeshProCustomDistanceField"
//            }
//
//            Blend SrcAlpha OneMinusSrcAlpha
//
//            CGPROGRAM
//            #pragma target 3.0
//            #pragma vertex VertShader
//            #pragma fragment PixShader
//            #pragma shader_feature __ BEVEL_ON
//            #pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
//            #pragma shader_feature __ GLOW_ON
//
//            #pragma multi_compile __ UNITY_UI_CLIP_RECT
//            #pragma multi_compile __ UNITY_UI_ALPHACLIP
//
//            #include "DistanceField.cginc"
//
//            ENDCG
//        }

        Pass
        {
            Name "DistanceFieldRed"
            Tags
            {
                "LightMode" = "TextMeshProCustomDistanceFieldRed"
            }

            Blend SrcAlpha One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex VertShader
            #pragma fragment PixShader
            #pragma shader_feature __ BEVEL_ON
            #pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
            #pragma shader_feature __ GLOW_ON

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "DistanceFieldRed.cginc"

            ENDCG
        }

        Pass
        {
            Name "DistanceFieldGreen"
            Tags
            {
                "LightMode" = "TextMeshProCustomDistanceFieldGreen"
            }

           Blend SrcAlpha One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex VertShader
            #pragma fragment PixShader
            #pragma shader_feature __ BEVEL_ON
            #pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
            #pragma shader_feature __ GLOW_ON

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "DistanceFieldGreen.cginc"

            ENDCG
        }

        Pass
        {
            Name "DistanceFieldBlue"
            Tags
            {
                "LightMode" = "TextMeshProCustomDistanceFieldBlue"
            }

           Blend SrcAlpha One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex VertShader
            #pragma fragment PixShader
            #pragma shader_feature __ BEVEL_ON
            #pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
            #pragma shader_feature __ GLOW_ON

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "DistanceFieldBlue.cginc"

            ENDCG
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "EmojiGenerator.Editor.ShaderGUI.Additional"
}
