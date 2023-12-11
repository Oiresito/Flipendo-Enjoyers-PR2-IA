using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get { return _instance; }
    }


    public RuleTile ruleTile;

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();

        map = new Dictionary<Vector2Int, OverlayTile>();

        BoundsInt bounds = tileMap.cellBounds;

        for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
        {
            for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
            {
                var tileLocation = new Vector2Int(x, y);
                var tileKey = new Vector2Int(x, y);

                if (tileMap.HasTile(new Vector3Int(x, y, 0)) && !map.ContainsKey(tileKey))
                {
                    var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                    var cellWorldPosition = tileMap.GetCellCenterWorld(new Vector3Int(x, y, 0));

                    overlayTile.transform.position = new Vector2(cellWorldPosition.x, cellWorldPosition.y);
                    overlayTile.GetComponent<SpriteRenderer>().sortingOrder = 20;
                    overlayTile.gridLocation = tileLocation;


                    map.Add(tileKey, overlayTile);



                }

            }
        }

    }
    public OverlayTile GetTileFromTileLocation(Vector2Int tileLocation)
    {
        if (map.ContainsKey(tileLocation))
        {
            return map[tileLocation];
        }
        return null;
    }
    public Vector3 GetWorldPositionFromTileLocation(Vector2Int tileLocation)
    {
        var tile = GetTileFromTileLocation(tileLocation);
        if (tile != null)
        {
            return tile.transform.position;
        }
        return Vector3.zero;
    }

    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> searchableTiles)
    {
        Dictionary<Vector2Int, OverlayTile> tileToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if (searchableTiles.Count > 0)
        {
            foreach (var item in searchableTiles)
            {
                tileToSearch.Add(item.grid2DLocation, item);

            }

        }
        else
        {
            tileToSearch = map;
        }

        List<OverlayTile> neighbours = new List<OverlayTile>();

        // Top
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        TileBase tile = tileMap.GetTile(new Vector3Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1, 0));
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);
        if (tile != ruleTile)
        {
            if (tileToSearch.ContainsKey(locationToCheck) && tileToSearch != null)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }
        // Botton
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);
        tile = tileMap.GetTile(new Vector3Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1, 0));
        if (tile != ruleTile)
        {
            if (tileToSearch.ContainsKey(locationToCheck))
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }


        // Right
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y);
        tile = tileMap.GetTile(new Vector3Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y, 0));
        if (tile != ruleTile)
        {
            if (tileToSearch.ContainsKey(locationToCheck))
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }
        // Left
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y);
        tile = tileMap.GetTile(new Vector3Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y, 0));
        if (tile != ruleTile)
        {
            if (tileToSearch.ContainsKey(locationToCheck))
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }

        return neighbours;
    }

}