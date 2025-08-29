using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public static class HelperMenu
    {
        private const string CoreSceneName = "CoreScene";
        private const string StartSceneName = "StartScene";
        
        [MenuItem("Helper/Scene Navigation/Go To Core Scene")]
        public static void GoToCoreScene()
        {
            if (Application.isPlaying)
            {
                SceneManager.LoadScene(CoreSceneName);
                Debug.Log($"[Helper] Switched to {CoreSceneName} (Play Mode)");
            }
            else
            {
                string scenePath = FindScenePath(CoreSceneName);
                if (!string.IsNullOrEmpty(scenePath))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePath);
                        Debug.Log($"[Helper] Opened {CoreSceneName} (Edit Mode)");
                    }
                }
                else
                {
                    Debug.LogError($"[Helper] Scene '{CoreSceneName}' not found in Build Settings!");
                }
            }
        }

        [MenuItem("Helper/Scene Navigation/Go To Start Scene")]
        public static void GoToStartScene()
        {
            if (Application.isPlaying)
            {
                SceneManager.LoadScene(StartSceneName);
                Debug.Log($"[Helper] Switched to {StartSceneName} (Play Mode)");
            }
            else
            {
                string scenePath = FindScenePath(StartSceneName);
                if (!string.IsNullOrEmpty(scenePath))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePath);
                        Debug.Log($"[Helper] Opened {StartSceneName} (Edit Mode)");
                    }
                }
                else
                {
                    Debug.LogError($"[Helper] Scene '{StartSceneName}' not found in Build Settings!");
                }
            }
        }

        [MenuItem("Helper/Development/Reload Current Scene")]
        public static void ReloadCurrentScene()
        {
            if (Application.isPlaying)
            {
                UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.name);
                Debug.Log($"[Helper] Reloaded scene: {currentScene.name}");
            }
            else
            {
                UnityEngine.SceneManagement.Scene currentScene = EditorSceneManager.GetActiveScene();
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(currentScene.path);
                    Debug.Log($"[Helper] Reloaded scene: {currentScene.name}");
                }
            }
        }
        
        private static string FindScenePath(string sceneName)
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    string name = Path.GetFileNameWithoutExtension(scene.path);
                    if (name == sceneName)
                    {
                        return scene.path;
                    }
                }
            }
            
            string[] sceneGuids = AssetDatabase.FindAssets($"{sceneName} t:Scene");
            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string name = Path.GetFileNameWithoutExtension(path);
                if (name == sceneName)
                {
                    Debug.LogWarning($"[Helper] Scene '{sceneName}' found in project but not in Build Settings. Consider adding it to Build Settings.");
                    return path;
                }
            }
            
            return null;
        }
        
        [MenuItem("Helper/Scene Navigation/Go To Core Scene", true)]
        public static bool ValidateGoToCoreScene()
        {
            return !string.IsNullOrEmpty(FindScenePath(CoreSceneName));
        }

        [MenuItem("Helper/Scene Navigation/Go To Start Scene", true)]
        public static bool ValidateGoToStartScene()
        {
            return !string.IsNullOrEmpty(FindScenePath(StartSceneName));
        }
    }
}