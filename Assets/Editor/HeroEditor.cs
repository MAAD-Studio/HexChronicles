using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ActiveSkill;
using static Status;

public class HeroEditor : EditorWindow
{
    public List<HeroAttributesSO> heroSOs = new List<HeroAttributesSO>();
    public List<BasicAttributes> loadedHeroes = new List<BasicAttributes>();
    private string savePath = "Assets/Game/Scriptables/Heroes";
    private string newAssetName = "";
    private Vector2 scrollPosition;

    [MenuItem("CustomTools/Hero Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(HeroEditor));
        HeroDataManager.Instance.AddHeroSO();
    }

    private void OnEnable()
    {
        heroSOs = HeroDataManager.Instance.heroSOs;
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Heroes in DataManager:", EditorStyles.boldLabel);
        // maybe add input field for a list of HeroAttributesSO
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        #region Display Attributes

        // Scroll view
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Start display each hero horizontally
        EditorGUILayout.BeginHorizontal(); 

        // Display each attributes vertically
        for (int i = 0; i < heroSOs.Count; i++)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(400)); // Adjust width here
            GUIStyle centeredBoldLabel = new GUIStyle(EditorStyles.boldLabel);
            centeredBoldLabel.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField($"{heroSOs[i].name}", centeredBoldLabel);
            GUILayout.Space(5);

            GUILayout.Label("Attributes:", EditorStyles.boldLabel);
            foreach (var field in heroSOs[i].attributes.GetType().GetFields())
            {
                if (field.FieldType == typeof(string))
                {
                    field.SetValue(heroSOs[i].attributes, EditorGUILayout.TextField
                        (field.Name, (string)field.GetValue(heroSOs[i].attributes)));
                }
                else if (field.FieldType == typeof(float))
                {
                    field.SetValue(heroSOs[i].attributes, EditorGUILayout.FloatField
                        (field.Name, (float)field.GetValue(heroSOs[i].attributes)));
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(heroSOs[i].attributes, EditorGUILayout.IntField
                        (field.Name, (int)field.GetValue(heroSOs[i].attributes)));
                }
                else if (field.FieldType == typeof(Sprite))
                {
                    field.SetValue(heroSOs[i].attributes, (Sprite)EditorGUILayout.ObjectField
                        (field.Name, (Sprite)field.GetValue(heroSOs[i].attributes), typeof(Sprite), false));
                }
                else if (field.FieldType == typeof(ElementType))
                {
                    field.SetValue(heroSOs[i].attributes, (ElementType)EditorGUILayout.EnumPopup
                        (field.Name, (ElementType)field.GetValue(heroSOs[i].attributes)));
                }

                GUILayout.Space(5);
            }

            GUILayout.Space(10);
            GUILayout.Label("Attack Area:", EditorStyles.boldLabel);
            heroSOs[i].attackArea = (AttackArea)EditorGUILayout.ObjectField(heroSOs[i].attackArea, typeof(AttackArea), false);

            GUILayout.Space(10);
            GUILayout.Box("", GUILayout.Height(5), GUILayout.ExpandWidth(true));
            GUILayout.Label("Active Skill:", EditorStyles.boldLabel);
            foreach (var field in heroSOs[i].activeSkill.GetType().GetFields())
            {
                if (field.FieldType == typeof(string))
                {
                    if (field.Name == "description")
                    {
                        string currentText = (string)field.GetValue(heroSOs[i].activeSkill);
                        field.SetValue(heroSOs[i].activeSkill, EditorGUILayout.TextField
                            (field.Name, (string)field.GetValue(heroSOs[i].activeSkill), GUILayout.Height(60)));
                    }
                    else
                    {
                        field.SetValue(heroSOs[i].activeSkill, EditorGUILayout.TextField
                            (field.Name, (string)field.GetValue(heroSOs[i].activeSkill)));
                    }
                }
                else if (field.FieldType == typeof(float))
                {
                    field.SetValue(heroSOs[i].activeSkill, EditorGUILayout.FloatField
                        (field.Name, (float)field.GetValue(heroSOs[i].activeSkill)));
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(heroSOs[i].activeSkill, EditorGUILayout.IntField
                        (field.Name, (int)field.GetValue(heroSOs[i].activeSkill)));
                }
                else if (field.FieldType == typeof(bool))
                {
                    field.SetValue(heroSOs[i].activeSkill, EditorGUILayout.Toggle
                        (field.Name, (bool)field.GetValue(heroSOs[i].activeSkill)));
                }
                else if (field.FieldType == typeof(Sprite))
                {
                    field.SetValue(heroSOs[i].activeSkill, (Sprite)EditorGUILayout.ObjectField
                        (field.Name, (Sprite)field.GetValue(heroSOs[i].activeSkill), typeof(Sprite), false));
                }
                else if (field.FieldType == typeof(AttackArea))
                {
                    field.SetValue(heroSOs[i].activeSkill, (AttackArea)EditorGUILayout.ObjectField
                        (field.Name, (AttackArea)field.GetValue(heroSOs[i].activeSkill), typeof(AttackArea), false));
                }
                else if (field.FieldType == typeof(GameObject))
                {
                    field.SetValue(heroSOs[i].activeSkill, (GameObject)EditorGUILayout.ObjectField
                        (field.Name, (GameObject)field.GetValue(heroSOs[i].activeSkill), typeof(GameObject), false));
                }
                else if (field.FieldType == typeof(SkillEffect))
                {
                    field.SetValue(heroSOs[i].activeSkill, (SkillEffect)EditorGUILayout.EnumFlagsField
                        (field.Name, (SkillEffect)field.GetValue(heroSOs[i].activeSkill)));
                }
                else if (field.FieldType == typeof(Status))
                {
                    foreach (var statusField in field.GetValue(heroSOs[i].activeSkill).GetType().GetFields())
                    {
                        if (statusField.FieldType == typeof(Status.StatusTypes))
                        {
                            statusField.SetValue(heroSOs[i].activeSkill.status, (StatusTypes)EditorGUILayout.EnumPopup
                                (statusField.Name, (StatusTypes)statusField.GetValue(heroSOs[i].activeSkill.status)));
                        }
                        else if (statusField.FieldType == typeof(int))
                        {
                            statusField.SetValue(heroSOs[i].activeSkill.status, EditorGUILayout.IntField
                                (statusField.Name, (int)statusField.GetValue(heroSOs[i].activeSkill.status)));
                        }
                    }
                }
                else if (field.FieldType == typeof(SkillKeyword[]))
                {
                    GUILayout.Box("", GUILayout.Height(3), GUILayout.ExpandWidth(true));
                    EditorGUILayout.LabelField("Keywords:", EditorStyles.boldLabel);
                    foreach (var keyword in (SkillKeyword[])field.GetValue(heroSOs[i].activeSkill))
                    {
                        EditorGUILayout.LabelField("Keyword:");
                        EditorGUILayout.BeginHorizontal();
                        keyword.keyword = EditorGUILayout.TextField("Name", keyword.keyword);
                        keyword.color = EditorGUILayout.ColorField(keyword.color, GUILayout.Width(80));
                        EditorGUILayout.EndHorizontal();
                        keyword.bold = EditorGUILayout.Toggle("Bold", keyword.bold);
                        keyword.italic = EditorGUILayout.Toggle("Italic", keyword.italic);
                    }
                    GUILayout.Box("", GUILayout.Height(3), GUILayout.ExpandWidth(true));
                }
                /*else if (field.FieldType == typeof(ReleaseTypes))
                {
                    field.SetValue(heroSOs[i].activeSkill, (ReleaseTypes)EditorGUILayout.EnumPopup
                                               (field.Name, (ReleaseTypes)field.GetValue(heroSOs[i].activeSkill)));
                }*/
                GUILayout.Space(5);
            }

            EditorGUILayout.EndVertical();

            GUILayout.Box("", GUILayout.Width(5), GUILayout.ExpandHeight(true));
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        #endregion Display Attributes

        GUILayout.Space(10);
        GUILayout.Label("Remember to save:", EditorStyles.boldLabel);
        if (GUILayout.Button("------ Save the Changes ------", new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold }))
        {
            SaveEditorChanges();
        }

        // Create a new heroSO asset
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Add New Hero", EditorStyles.boldLabel);

        newAssetName = EditorGUILayout.TextField("Asset Name", newAssetName);

        if (GUILayout.Button("Add New Hero:"))
        {
            if (newAssetName == "")
            {
                Debug.LogError("Please enter a name for the asset");
            }
            else
            {
                AddNewHero();
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(10);
        if (GUILayout.Button("Load Hero Data from JSON"))
        {
            LoadHeroDataFromJSON();
        }

        if (GUILayout.Button("Save Hero Data to JSON"))
        {
            HeroDataManager.Instance.WriteJSON();
        }

        GUILayout.Space(10);
    }

    private void SaveEditorChanges()
    {
        foreach (HeroAttributesSO hero in heroSOs)
        {
            EditorUtility.SetDirty(hero);
        }
        AssetDatabase.SaveAssets();
    }

    private void AddNewHero()
    {
        if (!AssetDatabase.IsValidFolder(savePath))
        {
            Debug.LogError("Folder path does not exist");
        }
        if (newAssetName == "")
        {
            Debug.LogError("Please enter a name for the asset");
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{savePath}/{newAssetName}.asset");

        HeroAttributesSO heroAttributes = ScriptableObject.CreateInstance<HeroAttributesSO>();
        AssetDatabase.CreateAsset(heroAttributes, assetPath);
        AssetDatabase.SaveAssets();

        // Focus the project window and select the new asset
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = heroAttributes;

        //heroes.Add(heroAttributes.attributes);
    }

    public void LoadHeroDataFromJSON()
    {
        HeroDataManager.Instance.LoadJSON();
        loadedHeroes = HeroDataManager.Instance.heroes;
    }
}
