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
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace EmojiGenerator.Editor.Window
{
    /// <summary>
    /// シーンのコントローラー
    /// </summary>
    public sealed class SceneController : IDisposable
    {
        /// <summary>
        /// シーンアセットのパス
        /// </summary>
        private const string SceneAssetPath = "Assets/EmojiGenerator/Scenes/StudioScene.unity";

        /// <summary>
        /// シーン
        /// </summary>
        private readonly Scene m_Scene;

        /// <summary>
        /// 撮影用カメラ
        /// </summary>
        private readonly Camera m_Camera;


        /// <summary>
        /// シーンを閉じる前のセットアップ
        /// </summary>
        private SceneSetup[]? m_SceneSetups;

        /// <summary>
        /// シーンが先にアンロードされたら呼ばれます
        /// </summary>
        private UnityAction? m_OnSceneUnloaded;


        /// <summary>
        /// シーン内のカメラ
        /// </summary>
        public Camera Camera
        {
            [DebuggerStepThrough] get { return m_Camera; }
        }


        /// <summary>
        /// プレビュー用シーンを初期化します
        /// </summary>
        /// <param name="onSceneUnloaded">ウィンドウの破棄よりも、先にシーンが破棄されたときに呼ばれます</param>
        public SceneController(UnityAction onSceneUnloaded)
        {
            // シーンが先に破棄されたときのためのコールバックを入れます
            m_OnSceneUnloaded              =  onSceneUnloaded;
            EditorSceneManager.sceneClosed += OnSceneClosed;

            // シーンを強制的に切り替えるので、ユーザーにシーン保存を促します
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            // 現在開いているシーンを、復元するための情報を取得します
            m_SceneSetups = EditorSceneManager.GetSceneManagerSetup();

            // 絵文字を撮影するためのシーンを読み込みます
            m_Scene = EditorSceneManager.OpenScene (
                scenePath: SceneAssetPath,
                mode: OpenSceneMode.Single
            );
            Assert.IsTrue (
                condition: m_Scene.IsValid(),
                message: "m_Scene.IsValid()"
            );

            // 撮影用カメラを取得します
            m_Camera = Object.FindObjectOfType<Camera>();
            Assert.IsNotNull (value: m_Camera, message: "m_Camera != null");
        }


        /// <summary>
        /// シーンが閉じたときに呼ばれます
        /// </summary>
        /// <param name="scene"></param>
        /// <seealso cref="global::UnityEditor.SceneManagement.EditorSceneManager.sceneClosed" />
        private void OnSceneClosed(Scene scene)
        {
            // 別のシーンが破棄された時も呼ばれるため
            // 先に、対象のシーンが破棄されたのかを確認します
            if (scene != m_Scene)
            {
                return;
            }

            // コールバックは一度だけ呼ばれたいので
            // この時点で、コールバックを解除します
            EditorSceneManager.sceneClosed -= OnSceneClosed;

            // シーンが破棄されたことを知らせます
            m_OnSceneUnloaded?.Invoke();
            m_OnSceneUnloaded = null;

            // シーンの破棄が先に走ったので
            // 復元処理はしません
            m_SceneSetups = null;
        }


        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            // OnDestroyで呼ばれる関数なので
            // コールバックは必要ありません
            m_OnSceneUnloaded              =  null;
            EditorSceneManager.sceneClosed -= OnSceneClosed;

            // シーンを復元します
            if (m_SceneSetups != null)
            {
                // 0個だった場合は、復元できません
                if (m_SceneSetups.Length > 0)
                {
                    // シーンを復元します
                    EditorSceneManager.RestoreSceneManagerSetup (
                        value: m_SceneSetups
                    );
                }
                else
                {
                    // 復元するシーンが一つもなければ、空っぽの新規シーンを作ります
                    EditorSceneManager.NewScene (
                        setup: NewSceneSetup.EmptyScene,
                        mode: NewSceneMode.Single
                    );
                }

                m_SceneSetups = null;
            }
        }

        #endregion
    }
}
