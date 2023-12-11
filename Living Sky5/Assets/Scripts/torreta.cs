using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Torreta : MonoBehaviour
{
    public GameObject torretPrefab;
    private Vector2Int initialPosition;

    public int currentRangeIA;
    private IAInfo characterIATORRETA;

    public GameObject characterPrefab;
    private MouseController mouseController;

    public GameObject characterPrefabIA;
    private IAController iaController;
    
    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    private List<Vector2Int> occupiedPositions = new List<Vector2Int>();
    private List<Vector2Int> listaTiles = new List<Vector2Int>();
    private List<CharacterInfo> charactersApuntados = new List<CharacterInfo>();
    private List<IAInfo> iAsApuntados = new List<IAInfo>();

    public Color rangeColorBlue = new Color(1f, 0f, 0f, 0.5f);
    
    private void Start()
    {
        
        iaController = characterPrefabIA.GetComponent<IAController>();
        mouseController = characterPrefab.GetComponent<MouseController>();

        rangeFinder = new RangeFinder();
        pathFinder = new PathFinder();
        // Instantiate the character

        int minRange2 = -4;
        int maxRange2 = 0;
        
        initialPosition= new Vector2Int(0, -2);
        Vector3 initialWorldPosition = MapManager.Instance.GetWorldPositionFromTileLocation(initialPosition);
        characterIATORRETA = Instantiate(torretPrefab, initialWorldPosition, Quaternion.identity).GetComponent<IAInfo>();

        OverlayTile initialTile = MapManager.Instance.GetTileFromTileLocation(initialPosition);

        if (initialTile != null)
        {
            PositionCharacterOnTile(characterIATORRETA, initialTile);
        }
        else
        {
            Debug.LogError("Initial tile not found for position: " + initialPosition);
        }
        ShowMovementRange();    

    }
    void Update()
    {
        
    }

    private void PositionCharacterOnTile(IAInfo ia, OverlayTile tile)
    {
        ia.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        ia.GetComponent<SpriteRenderer>().sortingOrder = 30;
        ia.activeTileIA = tile;
    }
    public void ShowMovementRange()
    {
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }

        inRangeTiles = rangeFinder.GetTilesInRange(characterIATORRETA.activeTileIA, currentRangeIA);

        foreach (var item in inRangeTiles)
        {
            listaTiles.Add(item.grid2DLocation);
            item.ShowTile();
            item.GetComponent<SpriteRenderer>().color = rangeColorBlue;
        }
        
        
    }

    public void objetives(){

        occupiedPositions.Add(iaController.characterIA.activeTileIA.gridLocation);
        occupiedPositions.Add(mouseController.characters[0].activeTile.gridLocation);

        inRangeTiles = rangeFinder.GetTilesInRange(characterIATORRETA.activeTileIA, currentRangeIA);

        foreach (var item in inRangeTiles)
        {
            if (occupiedPositions.Contains(item.gridLocation))
            {   
                foreach (var p in mouseController.characters)
                {
                    if(p.activeTile.gridLocation==item.gridLocation){
                        charactersApuntados.Add(p);
                    }
                }
                if(iaController.characterIA.activeTileIA.gridLocation==item.gridLocation){
                    iAsApuntados.Add(iaController.characterIA);
                }
                item.ShowTile();
                item.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 0.5f); // Color verde semitransparente
            }
        }
    }
    
    public List<CharacterInfo> ObtenerCharactersApuntados()
    {
        return charactersApuntados;
    }

    public List<IAInfo> ObteneriAsApuntados()
    {
        return iAsApuntados;
    }

    public List<Vector2Int> ObtenerTiles()
    {
        return listaTiles;
    }
    public void ResetListas()
    {
        charactersApuntados=new List<CharacterInfo>();
        iAsApuntados = new List<IAInfo>();
    }

    public void ResetTiles()
    {
        listaTiles = new List<Vector2Int>();
    }

}