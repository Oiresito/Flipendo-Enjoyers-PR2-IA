using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder
{
    public List<OverlayTile> GetTilesInRange(OverlayTile startingTile, int range)
    {
        var inRangeTiles = new List<OverlayTile>();
        int stepCount = 0;

        inRangeTiles.Add(startingTile);

        var tileForPreviousStep = new List<OverlayTile>();
        tileForPreviousStep.Add(startingTile);

        while (stepCount < range)
        {
            var sorroundingTiles = new List<OverlayTile>();

            foreach (var item in tileForPreviousStep)
            {
                sorroundingTiles.AddRange(MapManager.Instance.GetNeighbourTiles(item , new List<OverlayTile>()));
            }

            inRangeTiles.AddRange(sorroundingTiles);
            tileForPreviousStep = sorroundingTiles.Distinct().ToList();
            stepCount++;
        }
        return inRangeTiles.Distinct().ToList();
    }
    public bool IsTileInRange(OverlayTile startingTile, Vector2Int targetPosition, int range)
    {
        // Obtener todos los tiles en el rango de movimiento desde el startingTile
        List<OverlayTile> tilesInRange = GetTilesInRange(startingTile, range);

        // Verificar si el targetPosition está entre los tiles en el rango
        foreach (var tile in tilesInRange)
        {
            if (tile.gridLocation == targetPosition)
            {
                return true;
            }
        }

        return false;
    }
}
