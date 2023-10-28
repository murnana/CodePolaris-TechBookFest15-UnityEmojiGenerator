// Copyright 2023 murnana.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#ifndef __TEXT_MESH_PRO__CUSTOM__COMMON__INCLUDED__
#define __TEXT_MESH_PRO__CUSTOM__COMMON__INCLUDED__

/**
 * \brief offset, rotateZへ移動します
 * \param offset どれくらいずらすのか
 * \param rotateZ オフセットの角度
 * \param originPosition オリジナルの頂点座標
 * \return 変換後の座標
 */
float4 ShiftMoveTo(float offset, float rotateZ, float4 originPosition)
{
    const float2x2 rotateMatrix = float2x2(
        cos(rotateZ), -sin(rotateZ),
        sin(rotateZ), cos(rotateZ)
    );

    float2 offset2d = mul(rotateMatrix, float2(0, offset));
    const float4x4 offsetMatrix = float4x4(
        1, 0, 0, offset2d.x,
        0, 1, 0, offset2d.y,
        0, 0, 1, 0,
        0, 0, 0, 1
    );
    return mul(offsetMatrix, originPosition);
}

#endif
