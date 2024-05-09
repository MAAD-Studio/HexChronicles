using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TileEditor : EditorWindow
{
    #region Variables

    //Parent object for the created Grid
    private GameObject gridParent;

    //Variables for the Grid Creation
    private Vector2 gridPosition = Vector2.zero;
    private Vector2 gridSize = Vector2.zero;
    private GameObject baseTilePrefab;

    //Variables for editing placed tiles
    private GameObject prefabTilesList;
    private int popupIndex = 0;
    private GameObject selectedPrefab;

    //Variables for adding tiles
    private Vector2 placementPosition = Vector2.zero;

    #endregion

    #region EditorMethods

    [MenuItem("CustomTools/Tile Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TileEditor));
    }

    private void OnGUI()
    {
        /*
         * GRID CREATION SECTION
         */

        //Header
        GUILayout.Label("Grid Generation Controls:", EditorStyles.boldLabel);
        GUILayout.Space(5);

        //Grid Start Position
        gridPosition = EditorGUILayout.Vector2Field("Grid Start Pos: ", gridPosition);
        GUILayout.Space(5);

        //Grid Sizing
        gridSize = EditorGUILayout.Vector2Field("Grid Size: ", gridSize);
        GUILayout.Space(10);

        //Prefab used for generating the grid
        baseTilePrefab = (GameObject)EditorGUILayout.ObjectField("Base Tile: ", baseTilePrefab, typeof(GameObject), false);
        GUILayout.Space(10);

        //Generates a new grid based on the provided information
        if (GUILayout.Button("Spawn New Grid"))
        {
            //Confirms the user wants to generate a new grid
            if(EditorUtility.DisplayDialog("New Grid Generation", "Are you sure you want to generate a new grid? The old one will be destroyed!", "Confirm", "Cancel"))
            {
                if(baseTilePrefab != null)
                {
                    //Creates the new grid
                    CreateGrid();
                }
                else
                {
                    //Throws an error if the user didn't provide a prefab for grid generation
                    EditorUtility.DisplayDialog("Tile Editor Error", "Editor doesn't have a baseTilePrefab set.", "Confirm");
                }
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //Gets the List of Prefabs the user wants to use while replacing
        prefabTilesList = (GameObject)EditorGUILayout.ObjectField("Prefab Tiles List: ", prefabTilesList, typeof(GameObject), true);
        GUILayout.Space(10);

        //Pulls the prefab names from the provided list
        List<string> prefabNames = new List<string>();
        if(prefabTilesList != null)
        {
            foreach (Transform prefab in prefabTilesList.transform.GetComponentInChildren<Transform>())
            {
                prefabNames.Add(prefab.name);
            }
        }

        GUILayout.Label("Tile Prefab Selection: ");
        //Checks what index the user has selected in the dropdown menu
        popupIndex = EditorGUILayout.Popup(popupIndex, prefabNames.ToArray());

        //If count of prefabs is greater than 0 is holds onto the selected one
        if(prefabNames.Count > 0)
        {
            selectedPrefab = prefabTilesList.transform.GetChild(popupIndex).gameObject;
        }
        GUILayout.Space(5);

        /*
         * TILE REPLACING SECTION
         */
        GUILayout.Label("Tile Replacement Controls:", EditorStyles.boldLabel);
        GUILayout.Space(5);

        //Replaces the tiles based on selection
        if (GUILayout.Button("Replace Selected Tiles"))
        {
            //Checks if there are any objects selected
            GameObject selected = (GameObject)Selection.activeObject;
            if(selectedPrefab == null)
            {
                EditorUtility.DisplayDialog("Tile Editor Error", "You need to select a replacement prefab before you can replace.", "Confirm");
            }
            else
            {
                if (selected != null)
                {
                    if (selected.GetComponent<Tile>() != null)
                    {
                        //Replaces the selected Tiles
                        ReplaceTile();
                    }
                    else
                    {
                        //Throws an error if the thing they are trying to replace isn't a tile
                        EditorUtility.DisplayDialog("Tile Editor Error", "Object you tried to replace doesn't have a tile component", "Confirm");
                    }
                }
            }
        }
        GUILayout.Space(5);

        /*
         * TILE DELETING SECTION
         */
        GUILayout.Label("Tile Deletion Controls:", EditorStyles.boldLabel);
        GUILayout.Space(5);

        //Deletes the selected tiles
        if (GUILayout.Button("Delete Selected Tiles"))
        {
            if (EditorUtility.DisplayDialog("Tile Replacement", "Are you sure you want to remove the selected tiles?", "Confirm", "Cancel"))
            {
                //Checks if there are any objects selected
                GameObject selected = (GameObject)Selection.activeObject;
                if (selected != null)
                {
                    if (selected.GetComponent<Tile>() != null)
                    {
                        //Replaces the selected Tiles
                        RemoveTile();
                    }
                    else
                    {
                        //Throws an error if the thing they are trying to replace isn't a tile
                        EditorUtility.DisplayDialog("Tile Editor Error", "Object you tried to remove doesn't have a tile component", "Confirm");
                    }
                }
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        /*
         * TILE ADDING SECTION
         */
        GUILayout.Label("Tile Addition Controls:", EditorStyles.boldLabel);
        GUILayout.Space(5);

        //Grid Start Position
        placementPosition = EditorGUILayout.Vector2Field("Tile Addition Pos: ", placementPosition);
        GUILayout.Space(5);

        if(GUILayout.Button("Add Tile at selected location"))
        {
            if (selectedPrefab != null)
            {
                AddTile();
            }
            else
            {
                EditorUtility.DisplayDialog("Tile Editor Error", "You need to select a replacement prefab before you can replace.", "Confirm");
            }
        }
    }

    #endregion

    #region CustomMethods

    private void ReplaceTile()
    {
        //Tracks the newly created tile and the currently selected tile
        GameObject selectedObject;
        GameObject newTile;

        //Used for multi select for destroying tiles
        List<GameObject> TilesToDestroy = new List<GameObject>();

        //If only a single tile is selected
        if (Selection.objects.Length < 1)
        {
            //Grabs the selected tile
            selectedObject = (GameObject)Selection.activeObject;

            //Transfers the information over to the new tile
            newTile = Instantiate(selectedPrefab, selectedObject.transform.position, selectedObject.transform.rotation);
            newTile.name = selectedObject.name;
            newTile.transform.parent = gridParent.transform;

            //Destroys the old tile
            DestroyImmediate(selectedObject);
        }
        else
        {
            //Runs through all the selected objects
            foreach(object tile in Selection.objects)
            {
                selectedObject = (GameObject)tile;
                //Checks if the selected object is a tile
                if(selectedObject.transform.GetComponent<Tile>() != null)
                {
                    //Transfers the information over to the new tile
                    newTile = Instantiate(selectedPrefab, selectedObject.transform.position, selectedObject.transform.rotation);
                    newTile.name = selectedObject.name;
                    newTile.transform.parent = gridParent.transform;

                    //Inserts the old tile into a list of tils to destroy
                    TilesToDestroy.Add(selectedObject);
                }
            }

            //Runs through the list of tiles to destroy and gets rid of them
            while (TilesToDestroy.Count > 0)
            {
                DestroyImmediate(TilesToDestroy[0]);
                TilesToDestroy.RemoveAt(0);
            }
        }
    }

    private void RemoveTile()
    {
        //Tracks the currently selected tile
        GameObject selectedObject;

        //Used for multi select for destroying tiles
        List<GameObject> TilesToDestroy = new List<GameObject>();

        //If only a single tile is selected
        if (Selection.objects.Length < 1)
        {
            //Grabs the selected tile
            selectedObject = (GameObject)Selection.activeObject;

            //Destroys the old tile
            DestroyImmediate(selectedObject);
        }
        else
        {
            //Runs through all the selected objects
            foreach (object tile in Selection.objects)
            {
                selectedObject = (GameObject)tile;
                //Checks if the selected object is a tile
                if (selectedObject.transform.GetComponent<Tile>() != null)
                {
                    //Inserts the old tile into a list of tils to destroy
                    TilesToDestroy.Add(selectedObject);
                }
            }

            //Runs through the list of tiles to destroy and gets rid of them
            while (TilesToDestroy.Count > 0)
            {
                DestroyImmediate(TilesToDestroy[0]);
                TilesToDestroy.RemoveAt(0);
            }
        }
    }

    private void AddTile()
    {
        if(gridParent != null)
        {
            Transform gridTransform = gridParent.transform;

            Vector2 tileSize = DetermineTileSize(selectedPrefab.GetComponent<MeshFilter>().sharedMesh.bounds);
            Vector3 position = gridTransform.position;

            position.x = gridTransform.position.x + tileSize.x * placementPosition.x;
            position.z = gridTransform.position.z + tileSize.y * placementPosition.y;

            position.z += DetermineOffset(placementPosition.x, tileSize.y);

            GenerateTile(selectedPrefab, position, new Vector2Int((int)placementPosition.x, (int)placementPosition.y));
        }
        else
        {
            EditorUtility.DisplayDialog("Tile Editor Error", "No grid created to add onto", "Confirm");
        }


    }

    #endregion

    #region BreadthPackageMethods

    private void CreateGrid()
    {
        DestroyGrid();

        gridParent = new GameObject();
        gridParent.name = "GridParent";
        gridParent.transform.position = gridPosition;
        Transform gridTransform = gridParent.transform;

        Vector2 tileSize = DetermineTileSize(baseTilePrefab.GetComponent<MeshFilter>().sharedMesh.bounds);
        Vector3 position = gridTransform.position;

        // Generate the positions
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                position.x = gridTransform.position.x + tileSize.x * x;
                position.z = gridTransform.position.z + tileSize.y * y;

                position.z += DetermineOffset(x, tileSize.y);

                GenerateTile(baseTilePrefab, position, new Vector2Int(x, y));
            }
        }
    }

    private void DestroyGrid()
    {
        if(gridParent != null)
        {
            Transform gridTransform = gridParent.transform;
            for (int i = gridTransform.childCount; i >= gridTransform.childCount; i--)
            {
                if(gridTransform.childCount == 0)
                {
                    break;
                }

                int child = Mathf.Clamp(i - 1, 0, gridTransform.childCount);
                DestroyImmediate(gridTransform.GetChild(child).gameObject);
            }

            DestroyImmediate(gridParent);
        }
    }

    private float DetermineOffset(float x, float y)
    {
        return x % 2 == 0 ? y / 2 : 0f;
    }

    Vector2 DetermineTileSize(Bounds tileBounds)
    {
        return new Vector2((tileBounds.extents.x * 2) * 0.75f, (tileBounds.extents.z * 2));
    }

    private void GenerateTile(GameObject tile, Vector3 pos, Vector2Int id)
    {
        GameObject newTile = Instantiate(tile.gameObject, pos, Quaternion.identity);
        newTile.transform.parent = gridParent.transform;
        newTile.name = id.ToString();
    }

    #endregion
}
