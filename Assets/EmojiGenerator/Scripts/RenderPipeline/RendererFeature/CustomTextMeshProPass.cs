// Copyright 2023 murnana.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#nullable enable
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace EmojiGenerator.Scripts.RenderPipeline.RendererFeature
{
    /// <summary>
    /// カスタムレンダーパイプライン : カスタマイズした TextMeshProシェーダー
    /// </summary>
    public sealed class CustomTextMeshProPass : ScriptableRenderPass
    {
        /// <summary>
        /// レンダリング対象のシェーダータグ一覧
        /// </summary>
        private static readonly List<ShaderTagId> m_ShaderTagIds
            = new()
              {
                  new(name: "TextMeshProCustomDistanceField"),
                  new(name: "TextMeshProCustomDistanceFieldRed"),
                  new(name: "TextMeshProCustomDistanceFieldGreen"),
                  new(name: "TextMeshProCustomDistanceFieldBlue")
              };

        /// <summary>
        /// レンダーキュー
        /// </summary>
        private readonly RenderQueueType m_RenderQueueType;

        /// <summary>
        /// レンダーフィルタ
        /// </summary>
        private FilteringSettings m_FilteringSettings;


        /// <inheritdoc />
        public CustomTextMeshProPass(RenderQueueType renderQueueType)
        {
            m_RenderQueueType = renderQueueType;
            m_FilteringSettings = new FilteringSettings (
                renderQueueRange: renderQueueType == RenderQueueType.Transparent
                                      ? RenderQueueRange.transparent
                                      : RenderQueueRange.opaque
            );
        }


        #region Overrides of ScriptableRenderPass

        /// <inheritdoc />
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var sortingCriteria = m_RenderQueueType == RenderQueueType.Transparent
                                      ? SortingCriteria.CommonTransparent
                                      : renderingData.cameraData.defaultOpaqueSortFlags;

            var drawingSettings = CreateDrawingSettings (
                shaderTagIdList: m_ShaderTagIds,
                renderingData: ref renderingData,
                sortingCriteria: sortingCriteria
            );

            context.DrawRenderers (
                cullingResults: renderingData.cullResults,
                drawingSettings: ref drawingSettings,
                filteringSettings: ref m_FilteringSettings
            );
        }

        #endregion
    }
}
