using UnityEngine;

public class Interact : MonoBehaviour // Handles mouse interaction
                                      
{
    #region member fields
    [SerializeField]
    AudioClip click;
    [SerializeField]
    AudioClip pop;
    [SerializeField]
    LayerMask interactMask;

    Camera mainCam;
    Tile currentTile;
    Character selectedCharacter;
    Pathfinder pathfinder;
    #endregion

    private void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();

        if (pathfinder == null)
            pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
    }

    private void Update()
    {
        Clear();
        MouseUpdate();
    }

    // <Function>
    // Uses array cast to inspect tiles, and can only hit tiles
    // allows interaction with tiles that are in the frontier
    private void MouseUpdate()
    {
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask))
            return;

        currentTile = hit.transform.GetComponent<Tile>();
        InspectTile();
    }

    private void InspectTile()
    {
        if (currentTile.Occupied)
            InspectCharacter();
        else
            NavigateToTile();
    }

    private void InspectCharacter()
    {
        if (currentTile.occupyingCharacter.Moving)
            return;

        currentTile.SetColor(TileColor.Highlighted);

        if (Input.GetMouseButtonDown(0))
            SelectCharacter();
    }

    private void Clear()
    {
        if (currentTile == null  || currentTile.Occupied == false)
            return;

        currentTile.ClearColor();
        currentTile = null;
    }

    // <Function>
    // If the tile is occupied, call the Pathfinder to generate new paths from this tile
    private void SelectCharacter() 
    {
        selectedCharacter = currentTile.occupyingCharacter;
        pathfinder.FindPaths(selectedCharacter);
        GetComponent<AudioSource>().PlayOneShot(pop);
    }

    // <Function>
    // If the tile is empty, attempt to navigate selected character to this tile
    private void NavigateToTile()
    {
        if (selectedCharacter == null)
            return;

        if (selectedCharacter.Moving == true || currentTile.CanBeReached == false)
            return;

        Path currentPath = pathfinder.PathBetween(currentTile, selectedCharacter.characterTile);

        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<AudioSource>().PlayOneShot(click);
            selectedCharacter.Move(currentPath);
            pathfinder.ResetPathfinder();
            selectedCharacter = null;
        }
    }
}