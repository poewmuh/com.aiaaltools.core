/*using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AiaalTools.UnityExtensions
{
    [InitializeOnLoad]
    public static class StartSceneToolbar
    {
        private const string globalScenesPath = "Assets/Game/Scenes/";
        private const string menuScenePath = globalScenesPath + "MainMenu.unity";
        private const string bootScenePath = globalScenesPath + "Boot.unity";

        private static SceneAsset startScene
        {
            get
            {
                const string path = globalScenesPath + "Boot.unity";
                return AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            }
        }

        static StartSceneToolbar()
        {
            ToolbarExtender.LeftToolbarGUI.Remove(OnToolbarGUI);
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);

            EditorApplication.playModeStateChanged += ResetStartScene;
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent($"Play {startScene.name}", $"Start {startScene.name} Scene"),
                    EditorStyles.toolbarButton))
            {
                EditorSceneManager.playModeStartScene = startScene;
                EditorApplication.isPlaying = true;
            }
            
            if (GUILayout.Button(new GUIContent($"Menu"),
                    EditorStyles.toolbarButton))
            {
                EditorSceneManager.OpenScene(menuScenePath);
            }
            
            if (GUILayout.Button(new GUIContent($"BootScene"),
                    EditorStyles.toolbarButton))
            {
                EditorSceneManager.OpenScene(bootScenePath);
            }
        }

        private static void ResetStartScene(PlayModeStateChange state)
        {
            if (EditorSceneManager.playModeStartScene != startScene ||
                state != PlayModeStateChange.ExitingPlayMode) return;

            EditorSceneManager.playModeStartScene = null;
        }
    }
}*/