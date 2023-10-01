#nullable enable
using TMPro.EditorUtilities;
using UnityEditor;

namespace EmojiGenerator.Editor.ShaderGUI
{
    /// <summary>
    /// TextMeshPro/Custom/Additional シェーダーのGUI表示
    /// </summary>
    public sealed class Additional : TMP_SDFShaderGUI
    {
        /// <summary>
        /// Effect パネルを表示するのか
        /// </summary>
        private static bool s_EffectPanelExpanded;


        /// <summary>
        /// エフェクトパネルを表示します
        /// </summary>
        private void DoEffectPanel()
        {
            using (new EditorGUI.IndentLevelScope())
            {
                DoFloat (
                    name: "_EffectAlpha",
                    label: "Effect Alpha"
                );
                DoFloat (
                    name: "_EffectOffset",
                    label: "Effect Offset"
                );
                DoFloat (
                    name: "_EffectRotateZ",
                    label: "Effect Rotate"
                );
            }

            EditorGUILayout.Space();
        }


        #region Overrides of TMP_SDFShaderGUI

        /// <inheritdoc />
        protected override void DoGUI()
        {
            base.DoGUI();

            s_EffectPanelExpanded = BeginPanel (
                panel: "Effect",
                expanded: s_EffectPanelExpanded
            );
            if (s_EffectPanelExpanded)
            {
                DoEffectPanel();
            }

            EndPanel();
        }

        #endregion
    }
}