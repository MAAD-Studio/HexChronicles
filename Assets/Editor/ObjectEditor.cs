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
         * OBJECT REPLACEMENT SECTION
         */



        /*
         * OBJECT DELETION SECTION
         */

        //Header
        GUILayout.Label("Tile Deletion Controls:", EditorStyles.boldLabel);

        //Deletes the selected objects
        if(GUILayout.Button("Delete Selected Objects"))
        {
            GameObject selected = (GameObject)Selection.activeObject;
            
            if (selected == null)
            {
                //Doesn't need to warn about anything
            }
            /*else if (selected.tag != "TileObjects")
            {
                EditorUtility.DisplayDialog("Object Editor Error",
                    "Object you tried to delete isn't a tileObject", "Confirm");
            }*/
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
            else if(selected.GetComponent<Tile>() == null)
            {
                EditorUtility.DisplayDialog("Object Editor Error",
                    "Object you tried to place an object on isn't a Tile", "Confirm");
            }
            else if(selectedPrefab == null)
            {
                EditorUtility.DisplayDialog("Object Editor Error",
                    "You need to select a prefab before you can add", "Confirm");
            }
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

    private void AddObject()
    {
        bool failedLoop = false;
        int failureCount = 0;

        Transform parentTransform = actingParent.transform;

        Vector3 tileSize = DetermineTileSize(baseTile.GetComponent<MeshFilter>().sharedMesh.bounds);
        Vector3 position = parentTransform.position;

        Vector2 tilePos;

        foreach (object Tile in Selection.gameObjects)
        {
            failedLoop = false;

            GameObject selectedTile = (GameObject)Tile;
            tilePos = VectorFromString(selectedTile.name);

            //Calculates where the Object should be placed
            position.x = parentTransform.position.x + tileSize.x * tilePos.x;
            position.z = parentTransform.position.z + tileSize.y * tilePos.y;

            position.z += DetermineOffset(tilePos.x, tileSize.y);

            position.y = selectedPrefab.transform.position.y;

            foreach(Transform child in actingParent.GetComponentInChildren<Transform>())
            {
                if(child.position == position)
                {
                    failureCount++;
                    failedLoop = true;
                    break;
                }
            }

            if(!failedLoop)
            {
                GenerateObject(selectedPrefab, position, new Vector2Int((int)tilePos.x, (int)tilePos.y), actingParent);
            }
        }

        if(failureCount > 0)
        {
            EditorUtility.DisplayDialog("Object Editor Error",
                    failureCount + " Objects couldn't be replaced since one was already there", "Confirm");
        }

        Selection.objects = null;
    }

    private void DeleteObject()
    {
        List<GameObject> objectsToDestroy = new List<GameObject>();
        int failureCount = 0;

        foreach (object obj in Selection.objects)
        {
            GameObject selectedObject = (GameObject)obj;

            //if(selectedObject.tag == "TileObjects")
            //{
                objectsToDestroy.Add(selectedObject);
            //}
            /*else
            {
                failureCount++;
            }*/
        }

        while(objectsToDestroy.Count > 0)
        {
            DestroyImmediate(objectsToDestroy[0]);
            objectsToDestroy.RemoveAt(0);
        }

        if(failureCount > 0)
        {
            EditorUtility.DisplayDialog("Object Editor Error",
                    failureCount + " Objects couldn't be deleted since they weren't tileObjects", "Confirm");
        }
    }

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
