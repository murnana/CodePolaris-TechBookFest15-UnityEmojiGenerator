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