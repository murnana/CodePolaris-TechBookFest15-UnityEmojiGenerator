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
