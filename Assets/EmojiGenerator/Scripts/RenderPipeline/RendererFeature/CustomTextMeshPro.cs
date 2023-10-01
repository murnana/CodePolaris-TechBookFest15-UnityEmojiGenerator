#nullable enable
using EmojiGenerator.Scripts.RenderPipeline.RendererFeature;
using UnityEngine.Assertions;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

namespace EmojiGenerator.RenderPipeline.RendererFeature
{
    /// <summary>
    /// カスタムレンダーパイプライン : カスタマイズした TextMeshProシェーダー
    /// </summary>
    public sealed class CustomTextMeshPro : ScriptableRendererFeature
    {
        private CustomTextMeshProPass? m_RenderOpaque;
        private CustomTextMeshProPass? m_RenderTransparent;

        #region Overrides of ScriptableRendererFeature

        /// <inheritdoc />
        public override void Create()
        {
            m_RenderOpaque      = new CustomTextMeshProPass (renderQueueType: RenderQueueType.Opaque);
            m_RenderTransparent = new CustomTextMeshProPass (renderQueueType: RenderQueueType.Transparent);
        }

        /// <inheritdoc />
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Assert.IsNotNull (value: m_RenderOpaque, message: "m_RenderOpaque != null");
            Assert.IsNotNull (value: m_RenderTransparent, message: "m_RenderTransparent != null");

            renderer.EnqueuePass (pass: m_RenderOpaque);
            renderer.EnqueuePass (pass: m_RenderTransparent);
        }

        #endregion
    }
}