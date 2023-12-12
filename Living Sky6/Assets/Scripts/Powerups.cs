using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Powerups : MonoBehaviour
{
    private MovPowers newPowerup;

    public List<GameObject> powerupsPrefabs;

    private Vector2Int initialPowerupsPositions;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < powerupsPrefabs.Count; i++)
        {
            int minRange1 = -9;
            int maxRange1 = -6;

            int minRange2 = -1;
            int maxRange2 = 1;

            initialPowerupsPositions= new Vector2Int(Random.Range(minRange1, maxRange1 + 1), Random.Range(minRange2, maxRange2 + 1));
            
            Vector3 initialWorldPosition = MapManager.Instance.GetWorldPositionFromTileLocation(initialPowerupsPositions);
            newPowerup = Instantiate(powerupsPrefabs[i], initialWorldPosition, Quaternion.identity).GetComponent<MovPowers>();
            
            OverlayTile initialTile = MapManager.Instance.GetTileFromTileLocation(initialPowerupsPositions);

            if (initialTile != null)
            {
                PositionPowerupOnTile(newPowerup, initialTile);
            }
            else
            {
                Debug.LogError("Tile inicial no encontrado para la posicion: " + initialPowerupsPositions);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PositionPowerupOnTile(MovPowers powerup, OverlayTile tile)
    {
        powerup.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);  
        powerup.GetComponent<SpriteRenderer>().sortingOrder = 30;
        powerup.activeTilePowerups = tile;
    }

}
