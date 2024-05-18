using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectEditor : EditorWindow
{
    #region Variables

    GameObject objectsPrefabList;
    GameObject actingParent;
    GameObject selectedPrefab;
    int prefabIndex;
    GameObject baseTile;

    #endregion

    #region EditorMethods

    [MenuItem("CustomTools/Object Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ObjectEditor));
    }

    private void OnGUI()
    {
        /*
         * OBJECT MODIFICATION SECTION
         */

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //Header
        GUILayout.Label("Object Modification Information:", EditorStyles.boldLabel);
        GUILayout.Space(5);

        //Gets the list of prefabs the user wants to use while modifying
        objectsPrefabList = (GameObject)EditorGUILayout.ObjectField("Prefab Objects List: ", objectsPrefabList, typeof(GameObject), true);
        GUILayout.Space(5);

        //The Parent the controls will interact with
        actingParent = (GameObject)EditorGUILayout.ObjectField("ObjectParent to add onto: ", actingParent, typeof(GameObject), true);
        GUILayout.Space(5);

        //The Base tile used in Grid assembly
        baseTile = (GameObject)EditorGUILayout.ObjectField("Grid Base Tile: ", baseTile, typeof(GameObject), true);
        GUILayout.Space(5);

        //Pulls the Prefab Names from the provided Prefab List
        List<string> prefabNames = new List<string>();
        if(objectsPrefabList != null)
        {
            foreach(Transform prefab in objectsPrefabList.transform.GetComponentInChildren<Transform>())
            {
                prefabNames.Add(prefab.name);
            }
        }

        //Checks what index the user has selected in the dropdown menu
        GUILayout.Label("Object Prefab Selection:");
        prefabIndex = EditorGUILayout.Popup(prefabIndex, prefabNames.ToArray());

        //If the count of prefabs is greater than 0 it holds onto the selecte one
        if(prefabNames.Count > 0)
        {
            selectedPrefab = objectsPrefabList.transform.GetChild(prefabIndex).gameObject;
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(15);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        /*
         * OBJECT DELETION SECTION
         */

        //Header
        GUILayout.Label("Tile Deletion Controls:", EditorStyles.boldLabel);

        //Deletes the selected objects
        if(GUILayout.Button("Delete Selected Objects"))
        {
            if(EditorUtility.DisplayDialog("Object Editor Confirm",
                "Are you sure you want to delete these objects?", "Confirm", "Cancel"))
            {
                GameObject selected = (GameObject)Selection.activeObject;

                if (selected == null)
                {
                    //Doesn't need to warn about anything
                }
                //Checks that the parent to act on has been provided
                else if (actingParent == null)
                {
                    EditorUtility.DisplayDialog("Object Editor Error",
                        "You havent provided the objectParent for objects to be added into", "Confirm");
                }
                else
                {
                    DeleteObject();
                }
            }
        }

        /*
         * OBJECT ADDITION SECTION
         */

        //Header
        GUILayout.Label("Object Addition Controls:", EditorStyles.boldLabel);

        //Adds an object onto the selected tile
        if(GUILayout.Button("Add Object on Tile"))
        {
            GameObject selected = (GameObject)Selection.activeObject;
            if(selected == null)
            {
                //Doesn't need to warn about anything
            }
            //Checks that a prefab has been selected
            else if(selectedPrefab == null)
            {
                EditorUtility.DisplayDialog("Object Editor Error",
                    "You need to select a prefab before you can add", "Confirm");
            }
            //Checks that the parent to act on has been provided
            else if(actingParent == null)
            {
                EditorUtility.DisplayDialog("Object Editor Error",
                    "You havent provided the objectParent for objects to be added into", "Confirm");
            }
            else
            {
                AddObject();
            }
        }
    }

    #endregion

    #region CustomMethods

    //Adds an Object into the Parent Object
    private void AddObject()
    {
        //Used for displaying how many failed actions there were
        bool failedLoop = false;
        int failureCount = 0;

        Transform parentTransform = actingParent.transform;

        //Determines tile sizing
        Vector3 tileSize = DetermineTileSize(baseTile.GetComponent<MeshFilter>().sharedMesh.bounds);
        Vector3 position = parentTransform.position;

        //Used for tracking the position of the tile the object will be placed on
        Vector2 tilePos;

        foreach (object Tile in Selection.gameObjects)
        {
            failedLoop = false;

            GameObject selectedTile = (GameObject)Tile;

            //Pulls the tile Grid Space position from its name
            tilePos = VectorFromString(selectedTile.name);

            //Calculates where the Object should be placed
            position.x = parentTransform.position.x + tileSize.x * tilePos.x;
            position.z = parentTransform.position.z + tileSize.y * tilePos.y;

            position.z += DetermineOffset(tilePos.x, tileSize.y);

            position.y = selectedPrefab.transform.position.y;

            //Checks that another object isn't already in that position
            foreach(Transform child in actingParent.GetComponentInChildren<Transform>())
            {
                if(child.position.x == position.x && child.position.z == position.z)
                {
                    failureCount++;
                    failedLoop = true;
                    break;
                }
            }

            //If there wasn't another object present at that location it generates it
            if(!failedLoop)
            {
                GenerateObject(selectedPrefab, position, new Vector2Int((int)tilePos.x, (int)tilePos.y), actingParent);
            }
        }

        //If any of the addition actions failed an error it throw showing how many
        if(failureCount > 0)
        {
            EditorUtility.DisplayDialog("Object Editor Error",
                    failureCount + " Objects couldn't be replaced since one was already there", "Confirm");
        }

        Selection.objects = null;
    }

    //Deletes Objects from the Parent Object
    private void DeleteObject()
    {
        List<GameObject> objectsToDestroy = new List<GameObject>();
        int failureCount = 0;

        foreach (object obj in Selection.objects)
        {
            GameObject selectedObject = (GameObject)obj;

            //Checks if the object is properly tagged.
            if(selectedObject.tag == "TileObject")
            {
                objectsToDestroy.Add(selectedObject);
            }
            else
            {
                failureCount++;
            }
        }

        //Destroys any objects that were correctly tagged
        while(objectsToDestroy.Count > 0)
        {
            DestroyImmediate(objectsToDestroy[0]);
            objectsToDestroy.RemoveAt(0);
        }

        //If any deletion actions failed an error is throw displaying the count
        if(failureCount > 0)
        {
            EditorUtility.DisplayDialog("Object Editor Error",
                    failureCount + " Objects couldn't be deleted since they weren't tileObjects", "Confirm");
        }

        Selection.objects = null;
    }

    //Pulls the Grid Space position of a tile from its name
    private Vector2 VectorFromString(string name)
    {
        string[] halves = name.Split(',');
        halves[0] = halves[0].Substring(1);
        halves[1] = halves[1].Substring(1, halves[1].Length - 2);

        Vector2 pos;
        pos.x = float.Parse(halves[0]);
        pos.y = float.Parse(halves[1]);

        return pos;
    }

    //Generates a new Object
    private void GenerateObject(GameObject objPrefab, Vector3 pos, Vector2Int id, GameObject parent)
    {
        GameObject newObject = Instantiate(objPrefab, pos, Quaternion.identity);
        newObject.transform.parent = parent.transform;
        newObject.name = id.ToString();
    }

    #endregion

    #region BreadthPackageMethods

    private float DetermineOffset(float x, float y)
    {
        return x % 2 == 0 ? y / 2 : 0f;
    }

    Vector2 DetermineTileSize(Bounds tileBounds)
    {
        return new Vector2((tileBounds.extents.x * 2) * 0.75f, (tileBounds.extents.z * 2));
    }

    #endregion
}
