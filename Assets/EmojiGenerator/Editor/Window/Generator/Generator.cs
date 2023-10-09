#nullable enable
using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace EmojiGenerator.Editor.Window
{
    /// <inheritdoc />
    /// <summary>
    /// カメラに映ったものをテクスチャに変換します
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/EditorWindow.html" />
    internal sealed class Generator : EditorWindow
    {
        /// <summary>
        /// <see cref="global::UnityEngine.UIElements.VisualTreeAsset" /> のアセットパス
        /// </summary>
        private const string VisualTreeAssetPath = "Assets/EmojiGenerator/Editor/Window/Generator.uxml";

        /// <summary>
        /// このウィンドウをメニューから呼び出す際の、メニューのパス
        /// </summary>
        private const string MenuItemName = "Window/Emoji Generator";

        /// <summary>
        /// ウィンドウタイトル
        /// </summary>
        private const string WindowTitle = "Emoji Generator";


        /// <summary>
        /// 読み込んだ<see cref="global::UnityEngine.UIElements.VisualTreeAsset" />
        /// </summary>
        [SerializeField]
        private VisualTreeAsset? m_VisualTreeAsset;


        private SceneController? m_Scene;
        private RenderTextureController? m_Texture;
        private Button? m_PreviewButton;
        private Button? m_CreateButton;


        /// <summary>
        /// メニューから実行されるときに呼ばれます
        /// </summary>
        [MenuItem (itemName: MenuItemName)]
        private static void ExecuteFromMenu()
        {
            // ウィンドウを取得します
            var window = GetWindow<Generator>();

            // ユーティリティウィンドウとして表示します
            window.ShowUtility();
        }

        /// <summary>
        /// シーンがアンロードされたときに呼ばれます
        /// </summary>
        /// <seealso cref="global::EmojiGenerator.Editor.Window.SceneController"/>
        private void OnSceneUnloaded()
        {
            if (this != null)
            {
                Close();
            }
        }

        /// <summary>
        /// プレビューボタン (<see cref="m_PreviewButton" />) が押下されたときに呼ばれます
        /// </summary>
        /// <seealso cref="global::UnityEngine.UIElements.Button.clicked" />
        private void OnClickedPreviewButton()
        {
            var camera = m_Scene?.Camera;
            if (camera == null)
            {
                Debug.LogException (exception: new NullReferenceException (message: nameof(m_Scene.Camera)));
                return;
            }

            // レンダリング
            camera.Render();
        }

        /// <summary>
        /// 画像作成ボタン (<see cref="m_CreateButton" />) が押下されたときに呼ばれます
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void OnClickedCreateButton()
        {
            var camera = m_Scene?.Camera;
            if (camera == null)
            {
                Debug.LogException (exception: new NullReferenceException (message: nameof(m_Scene.Camera)));
                return;
            }

            var renderTexture = m_Texture?.Texture;
            if (renderTexture == null)
            {
                Debug.LogException (exception: new NullReferenceException (message: nameof(m_Texture.Texture)));
                return;
            }

            // 保存先パスを取得します
            var saveToPath = EditorUtility.SaveFilePanel (
                title: "Save to...",
                directory: null,
                defaultName: "New Stamp",
                extension: "png"
            );

            // ファイルパスがなければ保存しません
            if (string.IsNullOrEmpty (value: saveToPath))
            {
                var message = "Save to file cancel";
                EditorUtility.DisplayDialog (title: "Save to...", message: message, ok: "OK");
                Debug.LogFormat (
                    logType: LogType.Log,
                    logOptions: LogOption.NoStacktrace,
                    context: null,
                    format: message
                );
                return;
            }

            // ファイルパスを成形します
            var directoryPath = Path.GetDirectoryName (path: saveToPath);
            if (!string.IsNullOrEmpty (value: directoryPath)
             && !Directory.Exists (path: directoryPath))
            {
                Directory.CreateDirectory (path: directoryPath);
            }
            saveToPath = Path.ChangeExtension (path: saveToPath, extension: ".png");

            // レンダリング
            camera.Render();

            // ファイルに保存します
            // RenderTexture -> Texture2D へ変換します
            var oldActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            var texture2D = new Texture2D (
                width: renderTexture.width,
                height: renderTexture.height,
                format: renderTexture.graphicsFormat,
                flags: TextureCreationFlags.DontInitializePixels
            );
            texture2D.ReadPixels (
                source: new Rect (
                    x: 0,
                    y: 0,
                    width: texture2D.width,
                    height: texture2D.height
                ),
                destX: 0,
                destY: 0,
                recalculateMipMaps: false
            );
            texture2D.Apply();
            RenderTexture.active = oldActive;

            // 変換した Texture2D を PNG (byte配列) へ変換します
            var pngByte = texture2D.EncodeToPNG();

            // Texture2Dがこの時点で用なしになるので、破棄します
            DestroyImmediate (obj: texture2D, allowDestroyingAssets: true);

            // ファイルに保存します
            File.WriteAllBytes (
                path: saveToPath,
                bytes: pngByte
            );

            // 保存先を開きます
            Application.OpenURL (url: saveToPath);
        }

        /// <summary>
        /// Unity Editorアプリケーション終了時に呼ばれます
        /// </summary>
        private bool OnWantsToQuit()
        {
            EditorApplication.wantsToQuit -= OnWantsToQuit;

            // ウィンドウを閉じます
            Close();

            return true;
        }


        /// <summary>
        /// ウィンドウが読み込まれたときに呼ばれます
        /// </summary>
        /// <seealso href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/ScriptableObject.OnEnable.html" />
        private void OnEnable()
        {
            // ウィンドウのタイトルを設定します
            titleContent = new GUIContent (text: WindowTitle);

            // シーンを読み込みます
            m_Scene = new SceneController (
                onSceneUnloaded: OnSceneUnloaded
            );

            EditorApplication.wantsToQuit += OnWantsToQuit;
        }

        /// <summary>
        /// このウィンドウが破棄される直前に呼ばれます
        /// </summary>
        /// <seealso href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/EditorWindow.OnDestroy.html" />
        private void OnDestroy()
        {
            EditorApplication.wantsToQuit -= OnWantsToQuit;

            if (m_PreviewButton != null)
            {
                // コールバックの解除
                m_PreviewButton.clicked -= OnClickedPreviewButton;
                m_PreviewButton         =  null;
            }

            if (m_CreateButton != null)
            {
                m_CreateButton.clicked -= OnClickedCreateButton;
                m_CreateButton         =  null;
            }

            // シーンを読み込んでいる場合、破棄します
            m_Scene?.Dispose();
            m_Scene = null;

            // レンダーテクスチャの破棄をします
            m_Texture?.Dispose();
            m_Texture = null;
        }

        /// <summary>
        /// このウィンドウの <see cref="global::UnityEditor.EditorWindow.rootVisualElement" /> の準備が完了し、GUIの作成をするときに呼ばれます
        /// </summary>
        /// <seealso href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/EditorWindow.CreateGUI.html" />
        private void CreateGUI()
        {
            // m_VisualTreeAsset が設定されていなかった時のための保険としての処理
            if (m_VisualTreeAsset == null)
            {
                // 指定されたパス (VisualTreeAssetPath) から読み込みます
                m_VisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset> (assetPath: VisualTreeAssetPath);
            }

            // 念のためのnullチェック
            Assert.IsNotNull (
                value: m_VisualTreeAsset,
                message: $"'{VisualTreeAssetPath}' is exist"
            );

            // rootVisualElement に m_VisualTreeAsset の内容を生成します
            var root = rootVisualElement;
            m_VisualTreeAsset.CloneTree (target: root);

            // m_Textureの初期化
            var inspector    = root.Q<InspectorElement> (name: "m_RenderTextureInspector");
            var previewImage = root.Q<Image> (name: "m_PreviewImage");
            m_Texture = new RenderTextureController (
                inspector: inspector,
                previewImage: previewImage
            );

            // レンダーテクスチャをカメラにセット
            Assert.IsNotNull (value: m_Scene, message: "m_Scene != null");
            m_Scene!.Camera.targetTexture = m_Texture.Texture;

            // プレビューボタンの設定
            m_PreviewButton         =  root.Q<Button> (name: nameof(m_PreviewButton));
            m_PreviewButton.clicked += OnClickedPreviewButton;

            // ファイル保存ボタンの設定
            m_CreateButton         =  root.Q<Button> (name: nameof(m_CreateButton));
            m_CreateButton.clicked += OnClickedCreateButton;
        }
    }
}