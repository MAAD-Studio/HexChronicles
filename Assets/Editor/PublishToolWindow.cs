using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class PublishToolWindow : EditorWindow
{
    static string toolPath = "D:\\_CourseContents\\PROG56693_Games Tools and Data-Driven Design\\PublishTool\\bin\\Debug\\net6.0";
    static string innoFile = "D:\\Program\\Inno Setup 6\\ISCC.exe";
    static string issFile = "D:\\_Assignments\\PrototypeBuilds\\Setup\\HexCombatBuild.iss";
    static string butlerFile = "D:\\Program\\butler-windows-amd64\\butler.exe";
    static string outputPath = "D:\\_Assignments\\PrototypeBuilds\\Setup\\Output";
    static string fileName = "HexCombat_setup.exe";

    [MenuItem("Tools/Publish Game")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PublishToolWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label("Publish Tool", EditorStyles.boldLabel);
        GUILayout.Label("Please fill the fields below properly to publish the game.", EditorStyles.boldLabel);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

        GUILayout.Label("Path Input Example:", EditorStyles.boldLabel);
        GUILayout.Label("D:\\_CourseContents\\PROG56693_Games Tools and Data-Driven Design\\PublishTool\\bin\\Debug\\net6.0", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Label("File Input Example:", EditorStyles.boldLabel);
        GUILayout.Label("HexCombat_setup.exe", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        GUILayout.Space(5);

        toolPath = EditorGUILayout.TextField("Publish Tool's Path: ", toolPath);
        innoFile = EditorGUILayout.TextField("Inno File: ", innoFile);
        issFile = EditorGUILayout.TextField("ISS File: ", issFile);
        butlerFile = EditorGUILayout.TextField("Butler File: ", butlerFile);
        outputPath = EditorGUILayout.TextField("Output Path: ", outputPath);

        GUILayout.Space(10);

        if (toolPath == "" || innoFile == "" || issFile == "" || butlerFile == "" || outputPath == "")
        {
            EditorGUILayout.LabelField("Please fill all the fields!", EditorStyles.boldLabel);
        }
        else if(!System.IO.Directory.Exists(toolPath))
        {
            EditorGUILayout.LabelField("Tool's path is not valid!", EditorStyles.boldLabel);
        }
        else if (!System.IO.File.Exists(innoFile))
        {
            EditorGUILayout.LabelField("Inno's path is not valid!", EditorStyles.boldLabel);
        }
        else if (!System.IO.File.Exists(issFile))
        {
            EditorGUILayout.LabelField("ISS's path is not valid!", EditorStyles.boldLabel);
        }
        else if (!System.IO.File.Exists(butlerFile))
        {
            EditorGUILayout.LabelField("Butler's path is not valid!", EditorStyles.boldLabel);
        }
        else if (!System.IO.Directory.Exists(outputPath))
        {
            EditorGUILayout.LabelField("Output's path is not valid!", EditorStyles.boldLabel);
        }
        else
        {
            EditorGUILayout.LabelField("All paths are valid, ready to publish!", EditorStyles.boldLabel);

            if (GUILayout.Button("Publish!"))
            {
                UnityEngine.Debug.Log("Launching Publish Tool...");
                LaunchPublishTool();
            }
        }
    }

    static void LaunchPublishTool()
    {
        string setupCommand = "& \'" + innoFile + "\' \'" + issFile + "\'";
        string itchCommand = "& \'" + butlerFile + "\' push \'" + outputPath + "\\" + fileName + "\' " + "Aria-L/HexCombat:windows-standalone";

        Process process = new Process();
        process.StartInfo.FileName = toolPath + "\\PublishTool.exe";
        process.StartInfo.Arguments = $"\"{setupCommand}\" \"{itchCommand}\"";
        process.Start();

        process.WaitForExit();
        process.Close();
        UnityEngine.Debug.Log("Publish finished!");
    }
}
