using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAController : MonoBehaviour
{
    public GameObject characterPrefab;
    public Vector2Int initialPosition;

    public int currentRangeIA;
    [HideInInspector]
    public IAInfo characterIA;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    private List<OverlayTile> yellowRangeTiles = new List<OverlayTile>();

    public List<Transform> rangeMovement = new List<Transform>();
    public List<Transform> inRangeYellowMovement = new List<Transform>();

    private List<OverlayTile> redRangeTiles = new List<OverlayTile>();
    public List<Transform> inRangeRedMovement = new List<Transform>();


    public Color rangeColorBlue = new Color(0f, 0f, 1f, 0.5f);
    public Color rangeColorYellow = new Color(1f, 1f, 0f, 0.5f);
    public Color rangeColorRed = new Color(1f, 0f, 0f, 0.5f);

    private bool checkMove = true;

    private Vector2Int randomPosition;

    private void Start()
    {
        rangeFinder = new RangeFinder();
        pathFinder = new PathFinder();
        // Instantiate the character
        Vector3 initialWorldPosition = MapManager.Instance.GetWorldPositionFromTileLocation(initialPosition);
        characterIA = Instantiate(characterPrefab, initialWorldPosition, Quaternion.identity).GetComponent<IAInfo>();

        OverlayTile initialTile = MapManager.Instance.GetTileFromTileLocation(initialPosition);

        if (initialTile != null)
        {
            PositionCharacterOnTile(characterIA, initialTile);
        }
        else
        {
            Debug.LogError("Initial tile not found for position: " + initialPosition);
        }

        GameObject charactersObject = GameObject.Find("Characters");

        if (charactersObject != null)
        {
            foreach (Transform child in charactersObject.transform)
            {
                rangeMovement.Add(child);
            }
        }
    }

    void Update()
    {
        ShowYellowRange();
        ShowMovementRange();
        ShowRedRange();

        if (inRangeRedMovement.Count == 0)
        {
            MovementIA();
        }

    }
    public void MovementIA()
    {
        if (checkMove)
        {
            BehaviorNode behaviorTree = ConstructBehaviorTree();
            behaviorTree.Execute();
        }
    }

    public void RandomMove()
    {
        if (checkMove)
        {
            randomPosition = GetRandomPositionInMovementRange();
            checkMove = false;

            StartCoroutine(MoveIA());
        }
    }

    public void LookMove()
    {
        if (checkMove)
        {
            Transform targetTransform = inRangeYellowMovement[0];

            CharacterInfo targetIA = targetTransform.GetComponent<CharacterInfo>();

            MoveToPosition(targetIA.activeTile.gridLocation);
        }
        
    }
    private void MoveToPosition(Vector2Int targetPosition)
    {
        if (characterIA.activeTileIA.gridLocation == targetPosition)
        {
            path = pathFinder.FindPath(characterIA.activeTileIA, MapManager.Instance.GetTileFromTileLocation(targetPosition), inRangeTiles);

            StartCoroutine(MoveCharacterOnPath(targetPosition));
        }
        else
        {
            Vector2Int nearestPosition = FindNearestPositionInMovementRange(targetPosition);

            path = pathFinder.FindPath(characterIA.activeTileIA, MapManager.Instance.GetTileFromTileLocation(nearestPosition), inRangeTiles);

            StartCoroutine(MoveCharacterOnPath(nearestPosition));
        }
    }
    private Vector2Int FindNearestPositionInMovementRange(Vector2Int targetPosition)
    {
        List<OverlayTile> validTiles = rangeFinder.GetTilesInRange(characterIA.activeTileIA, Mathf.Clamp(currentRangeIA, 1, 2));

        Vector2Int nearestPosition = characterIA.activeTileIA.gridLocation;
        int minDistance = int.MaxValue;

        foreach (var tile in validTiles)
        {
            int distance = pathFinder.GetManhattenDistance(tile, MapManager.Instance.GetTileFromTileLocation(targetPosition));

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPosition = tile.gridLocation;
            }
        }

        return nearestPosition;
    }
    private IEnumerator MoveIA()
    {
        path = pathFinder.FindPath(characterIA.activeTileIA, MapManager.Instance.GetTileFromTileLocation(randomPosition), inRangeTiles);

        if (path.Count > 0)
        {
            Vector2Int targetPosition = path[path.Count - 1].gridLocation;
            yield return StartCoroutine(MoveCharacterOnPath(targetPosition));
        }

        publicVariables.myTurn = false;
        checkMove = true;
    }

    private BehaviorNode ConstructBehaviorTree()
    {
        return new ActionNode(characterIA, this, inRangeYellowMovement.Count, inRangeRedMovement.Count);
    }
    private IEnumerator MoveCharacterOnPath(Vector2Int targetPosition)
    {
        float journeyLength = 0f;
        float startTime = Time.time;
        float speed = 2f;

        foreach (var tile in path)
        {
            Vector2 startPosition = characterIA.transform.position;
            Vector2 tilePosition = tile.transform.position;
            journeyLength = Vector2.Distance(startPosition, tilePosition);

            while (Vector2.Distance(characterIA.transform.position, tilePosition) > 0.001f)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distCovered / journeyLength;

                characterIA.transform.position = Vector2.Lerp(startPosition, tilePosition, fractionOfJourney);

                yield return null;
            }

            PositionCharacterOnTile(characterIA, tile);

            foreach (var item in inRangeTiles)
            {
              //  item.HideTile();

            }

            yield return new WaitForSeconds(0f);

            publicVariables.myTurn = false;
            checkMove = true;

            startTime = Time.time;
            Debug.Log("�Personaje ha llegado al objetivo en el grid: " + targetPosition);
        }

    }
    private void PositionCharacterOnTile(IAInfo ia, OverlayTile tile)
    {
        ia.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        ia.GetComponent<SpriteRenderer>().sortingOrder = 30;
        ia.activeTileIA = tile;
    }

    
    private void ShowYellowRange()
    {
        foreach (var item in yellowRangeTiles)
        {
            //item.HideTile();
        }

        yellowRangeTiles = rangeFinder.GetTilesInRange(characterIA.activeTileIA, 9);

        inRangeYellowMovement.Clear();

        foreach (var item in yellowRangeTiles)
        {
           // item.ShowTile();
            //item.GetComponent<SpriteRenderer>().color = rangeColorYellow;

            foreach (var characterTransform in rangeMovement)
            {
                if (Vector3.Distance(item.transform.position, characterTransform.position) < 0.1f)
                {
                    inRangeYellowMovement.Add(characterTransform);
                    break;
                }
            }
        }
    }
    private void ShowMovementRange()
    {
        foreach (var item in inRangeTiles)
        {
            if (item != null)
            {
               // item.HideTile();
            }
        }

        inRangeTiles = rangeFinder.GetTilesInRange(characterIA.activeTileIA, currentRangeIA);

        foreach (var item in inRangeTiles)
        {
            if (item != null)
            {
                //item.ShowTile();
                //item.GetComponent<SpriteRenderer>().color = rangeColorBlue;
            }
        }
    }
    private void ShowRedRange()
    {
        foreach (var item in redRangeTiles)
        {
           // item.HideTile();
        }

        redRangeTiles = rangeFinder.GetTilesInRange(characterIA.activeTileIA, 2); // Ajusta el rango de ataque seg�n tus necesidades

        inRangeRedMovement.Clear();

        foreach (var item in redRangeTiles)
        {
           // item.ShowTile();
            //item.GetComponent<SpriteRenderer>().color = rangeColorRed;

            foreach (var characterTransform in rangeMovement)
            {
                if (Vector3.Distance(item.transform.position, characterTransform.position) < 0.1f)
                {
                    inRangeRedMovement.Add(characterTransform);
                    break;
                }
            }
        }
    }
    private Vector2Int GetRandomPositionInMovementRange()
    {
        List<OverlayTile> validTiles = rangeFinder.GetTilesInRange(characterIA.activeTileIA, Mathf.Clamp(currentRangeIA, 1, 2));

        if (validTiles.Count > 0)
        {
            float leftWeight = 5f;
            float rightWeight = 1f;

            int randomIndex = GetWeightedRandomIndex(validTiles.Count, leftWeight, rightWeight);

            return validTiles[randomIndex].gridLocation;
        }

        return characterIA.activeTileIA.gridLocation;
    }

    private int GetWeightedRandomIndex(int count, float leftWeight, float rightWeight)
    {
        float totalWeight = leftWeight + rightWeight;
        float randomValue = Random.value * totalWeight;

        float cumulativeProbabilityLeft = leftWeight / totalWeight;

        for (int i = 0; i < count; i++)
        {
            if (randomValue <= cumulativeProbabilityLeft)
            {
                return i;
            }

            cumulativeProbabilityLeft += 1.0f / count;
        }

        return Random.Range(0, count);
    }

    public void AttackMove()
    {
        if (checkMove)
        {
            checkMove = false;
        }
    }
}