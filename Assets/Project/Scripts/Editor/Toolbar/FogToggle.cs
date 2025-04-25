using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Toolbar
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command") {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }

    [InitializeOnLoad]
    public class FogToggleLeftButton
    {
        static FogToggleLeftButton() => UnityToolbarExtender.ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Ⓕ", "Toggle Fog"), ToolbarStyles.commandButtonStyle)) {
                Scene scene = EditorSceneManager.GetActiveScene();
                if (!SaveScene(scene)) return;
                RenderSettings.fog = !RenderSettings.fog;
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                GUIUtility.ExitGUI();
            }
        }

        private static bool SaveScene(Scene scene)
        {
            // Check if need to save scene & if user wants it
            if (!scene.isDirty) return true;
            if (!EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new Scene[] { scene })) return false;

            // Manage Single opened scene case
            if (EditorSceneManager.sceneCount <= 1) {
                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                return true;
            }

            // Try find multiple scene setup index
            SceneSetup[] setups = EditorSceneManager.GetSceneManagerSetup().Where((SceneSetup setup) => !setup.isSubScene).ToArray();
            int sceneIndex = System.Array.FindIndex(setups, (SceneSetup setup) => setup.path == scene.path);
            if (sceneIndex < 0) return false;

            // Manage Multiple opened scene case
            SceneSetup sceneSetup = setups[sceneIndex];
            EditorSceneManager.CloseScene(scene, removeScene: false);
            EditorSceneManager.OpenScene(scene.path, sceneSetup.isLoaded ? OpenSceneMode.Additive : OpenSceneMode.AdditiveWithoutLoading);
            if (sceneSetup.isActive) EditorSceneManager.SetActiveScene(scene);
            return true;
        }
    }
}