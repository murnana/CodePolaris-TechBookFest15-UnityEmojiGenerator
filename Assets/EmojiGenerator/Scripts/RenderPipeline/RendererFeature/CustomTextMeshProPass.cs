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