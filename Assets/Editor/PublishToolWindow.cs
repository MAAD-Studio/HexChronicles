using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class PublishToolWindow : EditorWindow
{
    static string toolPath = "C:\\Users\\lianti\\OneDrive - Sheridan College\\Hex Chronicles\\Source\\Code\\PublishTool";
    static string innoSetupFolder = "C:\\Program Files (x86)\\Inno Setup 6";
    static string issFile = "C:\\Users\\lianti\\OneDrive - Sheridan College\\Hex Chronicles\\Build\\Hex Chronicles Athera Build.iss";
    //static string butlerFile = "D:\\Program\\butler-windows-amd64\\butler.exe";
    static string butlerFile = "Assets/Game/Scripts/Systems/Build Commands/Editor/Butler/butler.exe";

    static string outputPath = "C:\\Users\\lianti\\OneDrive - Sheridan College\\Hex Chronicles\\Build\\Output";
    static string fileName = "Hex Chronicles Athera_setup.exe";

    [MenuItem("CustomTools/Publish Game")]
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
        GUILayout.Label("D:\\PublishTool\\bin\\Debug\\net6.0", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Label("File Input Example:", EditorStyles.boldLabel);
        GUILayout.Label("Hex Chronicles Athera_setup.exe", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        GUILayout.Space(5);

        toolPath = EditorGUILayout.TextField("Publish Tool's Path: ", toolPath);
        innoSetupFolder = EditorGUILayout.TextField("Inno File: ", innoSetupFolder);
        issFile = EditorGUILayout.TextField("ISS File: ", issFile);
        //butlerFile = EditorGUILayout.TextField("Butler File: ", butlerFile);
        outputPath = EditorGUILayout.TextField("Output Path: ", outputPath);
        fileName = EditorGUILayout.TextField("File Name: ", fileName);

        GUILayout.Space(10);

        if (toolPath == "" || innoSetupFolder == "" || issFile == "" || butlerFile == "" || outputPath == "" || fileName == "")
        {
            EditorGUILayout.LabelField("Please fill all the fields!", EditorStyles.boldLabel);
        }
        else if(!System.IO.Directory.Exists(toolPath))
        {
            EditorGUILayout.LabelField("Tool's path is not valid!", EditorStyles.boldLabel);
        }
        else if (!System.IO.Directory.Exists(innoSetupFolder))
        {
            EditorGUILayout.LabelField("Inno's path is not valid!", EditorStyles.boldLabel);
        }
        else if (!System.IO.File.Exists(issFile))
        {
            EditorGUILayout.LabelField("ISS's path is not valid!", EditorStyles.boldLabel);
        }
        /*else if (!System.IO.File.Exists(butlerFile))
        {
            EditorGUILayout.LabelField("Butler's path is not valid!", EditorStyles.boldLabel);
        }*/
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
        string setupCommand = "& \'" + innoSetupFolder + "\\ISCC.exe" + "\' \'" + issFile + "\'";
        string itchCommand = "& \'" + butlerFile + "\' push \'" + outputPath + "\\" + fileName + "\' " + "maad-studio/hex-chronicles-athera:windows-standalone";

        Process process = new Process();
        process.StartInfo.FileName = toolPath + "\\PublishTool.exe";
        process.StartInfo.Arguments = $"\"{setupCommand}\" \"{itchCommand}\"";
        process.Start();

        process.WaitForExit();
        process.Close();
        UnityEngine.Debug.Log("Publish finished!");
    }
}
