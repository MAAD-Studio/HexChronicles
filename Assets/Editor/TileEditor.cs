using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TileEditor : EditorWindow
{
    #region Variables

    //Variable storing the grid the editor is acting on
    private GameObject actingGrid;

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
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
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
            if(EditorUtility.DisplayDialog("New Grid Generation", "Are you sure you want to generate a new grid?", "Confirm", "Cancel"))
            {
                if(baseTilePrefab == null)
                {
                    //Throws an error if the user didn't provide a prefab for grid generation
                    EditorUtility.DisplayDialog("Tile Editor Error", "Editor doesn't have a baseTilePrefab set.", "Confirm");
                }
                else
                {
                    //Creates the new grid
                    CreateGrid();
                }
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(15);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        /*
         * TILE MODIFICATION SECTION
         */

        //Header
        GUILayout.Label("Tile Modification Information:", EditorStyles.boldLabel);
        GUILayout.Space(5);

        //Gets the List of Prefabs the user wants to use while replacing
        prefabTilesList = (GameObject)EditorGUILayout.ObjectField("Prefab Tiles List: ", prefabTilesList, typeof(GameObject), true);
        GUILayout.Space(5);

        //The grid the controls will interact with
        actingGrid = (GameObject)EditorGUILayout.ObjectField("Grid to add onto: ", actingGrid, typeof(GameObject), true);
        GUILayout.Space(5);

        //Pulls the prefab names from the provided prefab list
        List<string> prefabNames = new List<string>();
        if(prefabTilesList != null)
        {
            foreach (Transform prefab in prefabTilesList.transform.GetComponentInChildren<Transform>())
            {
                prefabNames.Add(prefab.name);
            }
        }

        //Checks what index the user has selected in the dropdown menu
        GUILayout.Label("Tile Prefab Selection: ");
        popupIndex = EditorGUILayout.Popup(popupIndex, prefabNames.ToArray());

        //If count of prefabs is greater than 0 is holds onto the selected one
        if(prefabNames.Count > 0)
        {
            selectedPrefab = prefabTilesList.transform.GetChild(popupIndex).gameObject;
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(15);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        /*
         * TILE REPLACING SECTION
         */
        
        //Header
        GUILayout.Label("Tile Replacement Controls:", EditorStyles.boldLabel);

        //Replaces the tiles based on selection
        if (GUILayout.Button("Replace Selected Tiles"))
        {
            //Checks if there are any objects selected
            GameObject selected = (GameObject)Selection.activeObject;

            //Checks if a Prefab has been selected
            if(selectedPrefab == null)
            {
                EditorUtility.DisplayDialog("Tile Editor Error", 
                    "You need to select a replacement prefab before you can replace.", "Confirm");
            }
            //Checks if any object has been selected
            else if(selected == null)
            {
                //Doesn't need to warn about anything
            }
            //Checks if the selected object has a Tile component
            else if(selected.GetComponent<Tile>() == null)
            {
                EditorUtility.DisplayDialog("Tile Editor Error", 
                    "Object you tried to replace doesn't have a tile component", "Confirm");
            }
            //Checks if the grid we are acting on has been provided
            else if(actingGrid == null)
            {
                EditorUtility.DisplayDialog("Tile Editor Error", 
                    "You havent provided the gridParent for replaced objects to be added into", "Confirm");
            }
            else
            {
                //Replaces the selected Tiles
                ReplaceTile();
            }
        }
        GUILayout.Space(5);

        /*
         * TILE DELETING SECTION
         */
        GUILayout.Label("Tile Deletion Controls:", EditorStyles.boldLabel);

        //Deletes the selected tiles
        if (GUILayout.Button("Delete Selected Tiles"))
        {
            //Checks if there are any objects selected
            GameObject selected = (GameObject)Selection.activeObject;
            if (selected == null)
            {
                //Doesn't need to warn about anything
            }
            //Checks if the selected object has a Tile component
            else if (selected.GetComponent<Tile>() == null)
            {
                EditorUtility.DisplayDialog("Tile Editor Error", 
                    "Object you tried to remove doesn't have a tile component", "Confirm");
            }
            //Checks if the grid we are acting on has been provided
            else if (actingGrid == null)
            {
                EditorUtility.DisplayDialog("Tile Editor Error", 
                    "You havent provided the gridParent for objects to be removed from", "Confirm");
            }
            else
            {
                //Confirms the user wants to delete the selected tiles
                if (EditorUtility.DisplayDialog("Tile Replacement",
                    "Are you sure you want to remove the selected tiles?", "Confirm", "Cancel"))
                {
                    RemoveTile();
                }
            }
            
        }
        GUILayout.Space(5);

        /*
         * TILE ADDING SECTION
         */

        //Header
        GUILayout.Label("Tile Addition Controls:", EditorStyles.boldLabel);
        GUILayout.Space(5);

        //Grid Start Position
        placementPosition = EditorGUILayout.Vector2Field("Tile Addition Pos: ", placementPosition);
        GUILayout.Space(5);

        //Adds a new tile to the coordinates provided by the user
        if (GUILayout.Button("Add Tile at given location"))
        {
            //Checks if the user selected a prefab
            if (selectedPrefab == null)
            {
                EditorUtility.DisplayDialog("Tile Editor Error", 
                    "You need to select a prefab to add before you can add.", "Confirm");
            }
            //Checks if the grid we are acting on has been provided
            else if (actingGrid == null)
            {
                EditorUtility.DisplayDialog("Tile Editor Error", 
                    "You havent provided the gridParent for objects to be added into", "Confirm");
            }
            else
            {
                AddTile();
            }
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    #endregion

    #region CustomMethods

    /*
     * Replaces the Tiles selected by the user
     */
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
            selectedObject = (GameObject)Selection.activeObject;

            //Transfers the information over to the new tile
            newTile = Instantiate(selectedPrefab, selectedObject.transform.position, selectedObject.transform.rotation);
            newTile.name = selectedObject.name;
            newTile.transform.parent = actingGrid.transform;

            DestroyImmediate(selectedObject);
        }
        else
        {
            //Runs through all the selected objects
            foreach(object tile in Selection.objects)
            {
                selectedObject = (GameObject)tile;

                if(selectedObject.transform.GetComponent<Tile>() != null)
                {
                    //Transfers the information over to the new tile
                    newTile = Instantiate(selectedPrefab, selectedObject.transform.position, selectedObject.transform.rotation);
                    newTile.name = selectedObject.name;
                    newTile.transform.parent = actingGrid.transform;

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

    /*
     * Removes the Tiles selected by the user
     */
    private void RemoveTile()
    {
        //Tracks the currently selected tile
        GameObject selectedObject;

        //Used for multi select for destroying tiles
        List<GameObject> TilesToDestroy = new List<GameObject>();

        //If only a single tile is selected
        if (Selection.objects.Length < 1)
        {
            selectedObject = (GameObject)Selection.activeObject;

            DestroyImmediate(selectedObject);
        }
        else
        {
            //Runs through all the selected objects
            foreach (object tile in Selection.objects)
            {
                selectedObject = (GameObject)tile;

                if (selectedObject.transform.GetComponent<Tile>() != null)
                {
                    //Inserts the old tile into a list of tiles to destroy
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
        //Grabs the transform of the grid the user provided
        Transform gridTransform = actingGrid.transform;

        Vector2 tileSize = DetermineTileSize(selectedPrefab.GetComponent<MeshFilter>().sharedMesh.bounds);
        Vector3 position = gridTransform.position;

        //Calculates where the tile should be placed
        position.x = gridTransform.position.x + tileSize.x * placementPosition.x;
        position.z = gridTransform.position.z + tileSize.y * placementPosition.y;

        position.z += DetermineOffset(placementPosition.x, tileSize.y);

        //Checks if a tile already exists where the user is trying to add one
        foreach (Transform child in actingGrid.GetComponentInChildren<Transform>())
        {
            if (child.position == position)
            {
                EditorUtility.DisplayDialog("Tile Editor Error", 
                    "There is already a tile where your trying to add one", "Confirm");
                return;
            }
        }

        GenerateTile(selectedPrefab, position, new Vector2Int((int)placementPosition.x, (int)placementPosition.y), actingGrid);
    }

    #endregion

    #region BreadthPackageMethods

    private void CreateGrid()
    {
        GameObject gridParent = new GameObject();
        gridParent.name = "GridParent";
        Vector3 gridPos = new Vector3(gridPosition.x, 0, gridPosition.y);
        gridParent.transform.position = gridPos;
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

                GenerateTile(baseTilePrefab, position, new Vector2Int(x, y), gridParent);
            }
        }
    }

    /*private void DestroyGrid()
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
    }*/

    private float DetermineOffset(float x, float y)
    {
        return x % 2 == 0 ? y / 2 : 0f;
    }

    Vector2 DetermineTileSize(Bounds tileBounds)
    {
        return new Vector2((tileBounds.extents.x * 2) * 0.75f, (tileBounds.extents.z * 2));
    }

    private void GenerateTile(GameObject tile, Vector3 pos, Vector2Int id, GameObject grid)
    {
        GameObject newTile = Instantiate(tile.gameObject, pos, Quaternion.identity);
        newTile.transform.parent = grid.transform;
        newTile.name = id.ToString();
    }

    #endregion
}
