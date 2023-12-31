// Copyright 2023 murnana.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#nullable enable
using System;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace EmojiGenerator.Editor.Window
{
    /// <summary>
    /// レンダーテクスチャを取り扱います
    /// </summary>
    public sealed class RenderTextureController : IDisposable
    {
        /// <summary>
        /// <see cref="global::UnityEngine.RenderTexture" /> のインスペクター表示
        /// </summary>
        private readonly InspectorElement m_Inspector;

        /// <summary>
        /// <see cref="global::UnityEngine.RenderTexture" /> のプレビュー
        /// </summary>
        private readonly Image m_PreviewImage;

        /// <summary>
        /// 描画先レンダーテクスチャ
        /// </summary>
        private readonly RenderTexture? m_Texture;

        /// <summary>
        /// <see cref="m_Texture" /> を <see cref="global::UnityEditor.UIElements.InspectorElement" /> に渡すための中継オブジェクト
        /// </summary>
        private readonly SerializedObject? m_SerializedObject;


        /// <summary>
        /// レンダーテクスチャ
        /// </summary>
        public RenderTexture? Texture
        {
            [DebuggerStepThrough]
            get { return m_Texture; }
        }


        /// <summary>
        /// レンダーテクスチャを生成します
        /// </summary>
        /// <param name="inspector"></param>
        /// <param name="previewImage"></param>
        public RenderTextureController(InspectorElement inspector, Image previewImage)
        {
            m_Inspector    = inspector;
            m_PreviewImage = previewImage;

            m_Texture = new RenderTexture (
                            width: 512,
                            height: 512,
                            colorFormat: GraphicsFormat.R8G8B8A8_SRGB,
                            depthStencilFormat: GraphicsFormat.None,
                            mipCount: 0
                        )
                        {
                            hideFlags      = HideFlags.DontSaveInBuild,
                            antiAliasing   = 1,
                            memorylessMode = RenderTextureMemoryless.None,
                            useMipMap      = false,
                            wrapMode       = TextureWrapMode.Clamp,
                            filterMode     = FilterMode.Point
                        };

            // 初期テクスチャは透明 (RGBA:0,0,0,0) なので、単色で染める
            var commandBuffer = new CommandBuffer();
            commandBuffer.Clear();
            if (!m_Texture.IsCreated())
            {
                m_Texture.Create();
            }

            commandBuffer.SetRenderTarget (rt: m_Texture);
            commandBuffer.ClearRenderTarget (
                clearFlags: RTClearFlags.All,
                backgroundColor: Color.magenta,
                depth: -1,
                stencil: 0
            );
            Graphics.ExecuteCommandBuffer (buffer: commandBuffer);
            commandBuffer.Release();

            m_PreviewImage.image     = m_Texture;
            m_PreviewImage.scaleMode = ScaleMode.ScaleToFit;

            m_SerializedObject = new SerializedObject (obj: m_Texture);
            m_Inspector.Bind (obj: m_SerializedObject);
        }


        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            m_Inspector.Unbind();

            m_SerializedObject?.Dispose();

            if (m_Texture != null)
            {
                if (m_Texture.IsCreated())
                {
                    m_Texture.Release();
                }

                Object.DestroyImmediate (
                    obj: m_Texture,
                    allowDestroyingAssets: true
                );
            }
        }

        #endregion
    }
}
