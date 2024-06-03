using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class PlayButton : Editor
{
    static PlayButton()
    {
        CustomEditorTools.Enable();
    }
}

public class CustomEditorTools : EditorWindow
{
    public static void Enable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static void Disable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        if (GUI.Button(new Rect(10, 10, 120, 30), "Start from Core"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) // check if the scene is saved
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Core/Core.unity");
            EditorApplication.isPlaying = true;
        }

        Handles.EndGUI();
    }
}