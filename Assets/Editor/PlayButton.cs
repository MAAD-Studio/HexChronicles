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

        if (GUI.Button(new Rect(10, 45, 120, 30), "Open UI"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Core/UI.unity");
        }

        #region Levels

        GUI.Label(new Rect(10, 80, 120, 20), "Lvl");

        if (GUI.Button(new Rect(10, 105, 20, 20), "1"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Levels/Small Level01.unity");
        }

        if (GUI.Button(new Rect(10, 130, 20, 20), "2"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Levels/Small Level03.unity");
        }

        if (GUI.Button(new Rect(10, 155, 20, 20), "3"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Levels/PlayTestLevel.unity");
        }

        if (GUI.Button(new Rect(10, 180, 20, 20), "4"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Levels/Puzzle 1.unity");
        }

        if (GUI.Button(new Rect(80, 80, 50, 20), "VFX"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Test Scenes/VFX_Test.unity");
        }

        #endregion


        #region Tutorials

        GUI.Label(new Rect(40, 80, 120, 20), "Tut");

        if (GUI.Button(new Rect(40, 105, 20, 20), "1"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Levels/Tutorials/Tutorial_BasicAttack.unity");
        }

        if (GUI.Button(new Rect(40, 130, 20, 20), "2"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Levels/Tutorials/Tutorial_ActiveSkill.unity");
        }

        if (GUI.Button(new Rect(40, 155, 20, 20), "3"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Levels/Tutorials/Tutorial_Tile.unity");
        }

        if (GUI.Button(new Rect(40, 180, 20, 20), "4"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            EditorSceneManager.OpenScene("Assets/Game/Scenes/Levels/Tutorials/Tutorial_Weather.unity");
        }
        #endregion*/

        Handles.EndGUI();
    }
}