// Copyright 2023 murnana.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
