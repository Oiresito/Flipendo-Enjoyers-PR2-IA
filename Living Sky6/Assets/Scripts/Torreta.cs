using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Torreta : MonoBehaviour
{
    public GameObject characterPrefab;

    public int currentRangeIA;
    private TowerInfo towerInfo;
    private OverlayTile initialTile;

    private RangeFinder rangeFinder;
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    public GameObject characterPrefabScript;
    private MouseController mouseController;

    public Color rangeColorRed = new Color(1f, 0f, 0f, 0.5f);

    public GameObject characterPrefabIA;
    private IAController iaController;

    private List<CharacterInfo> charactersApuntados = new List<CharacterInfo>();
    private List<IAInfo> iAsApuntados = new List<IAInfo>();
    private List<Vector2Int> occupiedPositions = new List<Vector2Int>();

    private Vector2Int positiontower;
    private void Start()
    {
        int minRange = -4;
        int maxRange = 0;

        positiontower = new Vector2Int(0, Random.Range(minRange, maxRange + 1));

        iaController = characterPrefabIA.GetComponent<IAController>();
        mouseController = characterPrefabScript.GetComponent<MouseController>();

        rangeFinder = new RangeFinder();
        // Instantiate the character
        Vector3 initialWorldPosition = MapManager.Instance.GetWorldPositionFromTileLocation(positiontower);
        towerInfo = Instantiate(characterPrefab, initialWorldPosition, Quaternion.identity).GetComponent<TowerInfo>();

        initialTile = MapManager.Instance.GetTileFromTileLocation(positiontower);

        PositionCharacterOnTile(towerInfo, initialTile);
    }
    void Update()
    {
        ShowMovementRange();

        if (publicVariables.rangeAttackTower == true)
        {
            Objetives();

            foreach (var item in charactersApuntados)
            {
                item.Pupa(1);
                mouseController.excluidos.Add(item.index);
            }

            foreach (var item in iAsApuntados)
            {
                item.Pupa(1);
            }

            
            ResetListas();
            publicVariables.rangeAttackTower = false;
        }
    }
    public void ResetListas()
    {
        charactersApuntados = new List<CharacterInfo>();
        iAsApuntados = new List<IAInfo>();
    }
    private void PositionCharacterOnTile(TowerInfo tower, OverlayTile tile)
    {
        tower.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        tower.GetComponent<SpriteRenderer>().sortingOrder = 30;
        tower.activeTileTower = tile;
    }
    public void ShowMovementRange()
    {
        List<OverlayTile> tilesInRange = mouseController.GetTilesInRange();

        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }

        inRangeTiles = rangeFinder.GetTilesInRange(towerInfo.activeTileTower, currentRangeIA);

        foreach (var item in inRangeTiles)
        {
            if (tilesInRange.Contains(item))
            {
                if (publicVariables.rangeTower)
                {
                    item.ShowTile();
                    item.GetComponent<SpriteRenderer>().color = rangeColorRed;
                }
                else
                {
                    item.ShowTile();
                    item.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 1f, 0.5f);
                }
            }
            else
            {
                item.ShowTile();
                item.GetComponent<SpriteRenderer>().color = rangeColorRed;
            }
        }
    }

    public void Objetives()
    {

        occupiedPositions.Add(iaController.characterIA.activeTileIA.gridLocation);
        occupiedPositions.Add(mouseController.characters[0].activeTile.gridLocation);

        inRangeTiles = rangeFinder.GetTilesInRange(towerInfo.activeTileTower, currentRangeIA);

        foreach (var item in inRangeTiles)
        {
            if (occupiedPositions.Contains(item.gridLocation))
            {
                foreach (var p in mouseController.characters)
                {
                    if (p.activeTile.gridLocation == item.gridLocation)
                    {
                        charactersApuntados.Add(p);
                    }
                }
                if (iaController.characterIA.activeTileIA.gridLocation == item.gridLocation)
                {
                    iAsApuntados.Add(iaController.characterIA);
                }
                item.ShowTile();
                item.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 0.5f); // Color verde semitransparente
            }
        }
    }
    
}