using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

[InitializeOnLoad]
public class ForcePlayFromMainMenu
{
    private const string PreviousSceneKey = "ForcePlayFromMainMenu.PreviousScene";
    private static bool wasAppActive = false;

    static ForcePlayFromMainMenu()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.update += CheckAppFocus;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        string mainMenuPath = "Assets/Scenes/MainMenu.unity";

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Store the current scene before switching to MainMenu
            var currentScenePath = EditorSceneManager.GetActiveScene().path;
            if (currentScenePath != mainMenuPath)
            {
                EditorPrefs.SetString(PreviousSceneKey, currentScenePath);
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(mainMenuPath);
                }
                else
                {
                    EditorApplication.isPlaying = false;
                }
            }
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Restore the previous scene after exiting Play Mode
            if (EditorPrefs.HasKey(PreviousSceneKey))
            {
                string previousScenePath = EditorPrefs.GetString(PreviousSceneKey);
                if (!string.IsNullOrEmpty(previousScenePath) && previousScenePath != mainMenuPath)
                {
                    EditorSceneManager.OpenScene(previousScenePath);
                }
                EditorPrefs.DeleteKey(PreviousSceneKey);
            }
        }
    }

    private static void CheckAppFocus()
    {
        if (EditorApplication.isPlaying)
        {
            bool isActive = InternalEditorUtility.isApplicationActive;
            if (wasAppActive && !isActive)
            {
                EditorApplication.isPlaying = false;
            }
            wasAppActive = isActive;
        }
    }
}