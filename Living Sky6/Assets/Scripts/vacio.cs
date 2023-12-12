using UnityEngine;
using UnityEngine.Tilemaps;

public class BorrarTileAleatorio : MonoBehaviour
{
    public Tilemap tilemap; // Asigna el Tilemap desde el Editor de Unity
    public RuleTile ruleTile;
    public RuleTile ruleTileTower;

    public Vector3Int vectorTower;
    void Start()
    {
        for (int i = 0; i < 25; i++)
        {
            BorrarTileAleatorioEnTilemap();
        }

    }

    void BorrarTileAleatorioEnTilemap()
    {
        // Genera una posición aleatoria dentro del tamaño del Tilemap
        Vector3Int randomTilePosition = new Vector3Int(
            Random.Range(-8, 8),
            Random.Range(-6, 3),
            0
        );

        tilemap.SetTile(randomTilePosition, ruleTile);
        tilemap.SetTile(vectorTower, ruleTile);
        tilemap.RefreshTile(randomTilePosition);
    }
}