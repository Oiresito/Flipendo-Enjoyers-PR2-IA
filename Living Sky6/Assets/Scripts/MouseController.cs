using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class MouseController : MonoBehaviour
{
    public float speed;
    public List<GameObject> characterPrefabs;
    [HideInInspector] public List<CharacterInfo> characters = new List<CharacterInfo>();
    private int activeCharacterIndex = 0;
    private IAController iaController;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    public List<Vector2Int> initialCharacterPositions;
    public List<int> movementCharacterRange;
    public List<Button> buttonsListCharacters;
    public List<int> attackCharacterRange;

    private CharacterInfo personajeatacado;

    public Button btn_next_round;
    public Button btn_move;
    public Button btn_attack;
    public Button btn_return;

    public GameObject canvas_change_round;
    public GameObject canvas_chooseCharacter;
    public GameObject canvas_chooseThingToDo;

    public bool canAttack = true;
    public bool canMove = true;
    private int activeButtonIndex = -1;

    private float lastClickTime = 0f;

    public Color rangeMoveColor;
    public Color rangeAttackColor;
    public Color pointOnAttackColor;

    public Color activeColorButton;
    public Color inactiveColorButton;

    public GameObject iAController;

    public List<int> excluidos=new List<int>();
    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();

        iaController = iAController.GetComponent<IAController>();

        GameObject charactersContainer = GameObject.Find("Characters"); // Encuentra el objeto "Characters"

        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            Vector3 initialWorldPosition = MapManager.Instance.GetWorldPositionFromTileLocation(initialCharacterPositions[i]);
            CharacterInfo newCharacter = Instantiate(characterPrefabs[i], initialWorldPosition, Quaternion.identity).GetComponent<CharacterInfo>();

            OverlayTile initialTile = MapManager.Instance.GetTileFromTileLocation(initialCharacterPositions[i]);

            if (initialTile != null)
            {
                PositionCharacterOnTile(newCharacter, initialTile);
            }
            else
            {
                Debug.LogError("Tile inicial no encontrado para la posici�n: " + initialCharacterPositions[i]);
            }

            characters.Add(newCharacter);

            // Mueve el personaje al contenedor "Characters"
            if (charactersContainer != null)
            {
                newCharacter.transform.parent = charactersContainer.transform;
            }
        }

        for (int i = 0; i < buttonsListCharacters.Count; i++)
        {
            int characterIndex = i;
            buttonsListCharacters[i].GetComponent<Image>().color = activeColorButton;
            buttonsListCharacters[i].onClick.AddListener(() => ActivateChooseThingToDoPanel(characterIndex));

        }
        canvas_chooseCharacter.SetActive(true);


    }

    void Update()
    {
        btn_next_round.onClick.AddListener(SetBtnChangeRound);

        if (publicVariables.movementsMouseController < characters.Count)
        {
            var focusedTileHit = GetFocusedOnTile();

            if (focusedTileHit.HasValue)
            {
                OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
                if(IsValidMove(overlayTile))
                {
                    
                    transform.position = overlayTile.transform.position;
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 25;

                    if (Input.GetMouseButtonDown(0) && canMove)
                    {
                        if (Time.time - lastClickTime > 0.2f)
                        {
                            lastClickTime = Time.time;
                            CharacterInfo currentCharacter = characters[activeCharacterIndex];

                            if (currentCharacter == null)
                            {
                                Debug.Log("Error move");
                            }
                            else
                            {
                                path = pathFinder.FindPath(currentCharacter.activeTile, overlayTile, inRangeTiles);
                                Debug.Log("La variable es " + publicVariables.movementsMouseController);
                                canvas_chooseThingToDo.SetActive(false);

                                characters[activeCharacterIndex].BorrarCuadroTexto();
                            }
                        }
                    }
                }

                if (IsValidAtack(overlayTile))
                {

                    transform.position = overlayTile.transform.position;
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = 25;

                    if (Input.GetMouseButtonDown(0) && canAttack)
                    {

                        if (Time.time - lastClickTime > 0.2f)
                        {
                            lastClickTime = Time.time;
                            CharacterInfo currentCharacter = characters[activeCharacterIndex];

                            if (currentCharacter == null)
                            {
                                Debug.Log("Error attack");
                            }
                            else
                            {
                                publicVariables.movementsMouseController++;
                                currentCharacter.EmpezarAtaque();
                                
                                personajeatacado.Pupa(currentCharacter.GetAtaque());
                                if(personajeatacado.vida<=0){
                                    Vector2Int salirCampo=new Vector2Int(10,10);
                                    excluidos.Add(personajeatacado.index);
                                    //characters[personajeatacado.index].activeTile.gridLocation=salirCampo;
                                    //characters[personajeatacado.index].activeTile = MapManager.Instance.GetTileFromTileLocation(salirCampo);
                                    
                                    
                                }
                                foreach (var item in inRangeTiles)
                                {
                                    item.HideTile();
                                }
                                for (int i = 0; i < buttonsListCharacters.Count; i++)
                                {
                                    if (activeButtonIndex == i || excluidos.Contains(i))
                                    {
                                        buttonsListCharacters[i].interactable = false;
                                        buttonsListCharacters[i].GetComponent<Image>().color = inactiveColorButton;
                                    }
                                }
                                
                                canvas_chooseCharacter.SetActive(true);
                                canvas_chooseThingToDo.SetActive(false);
                                canAttack = false;

                                characters[activeCharacterIndex].BorrarCuadroTexto();
                            }
                        }

                    }

                }

            }


            if (path.Count > 0)
            {
                float step = speed * Time.deltaTime;
                Vector2 targetPosition = path[0].transform.position;

                characters[activeCharacterIndex].transform.position = Vector2.MoveTowards(
                    characters[activeCharacterIndex].transform.position, targetPosition, step);

                if (Vector2.Distance(characters[activeCharacterIndex].transform.position, targetPosition) < 0.001f)
                {
                    PositionCharacterOnTile(characters[activeCharacterIndex], path[0]);
                    path.RemoveAt(0);
                }

                if (path.Count == 0)
                {
                    foreach (var item in inRangeTiles)
                    {
                        item.HideTile();
                    }

                    if (publicVariables.movementsMouseController < characters.Count)
                    {
                        publicVariables.movementsMouseController++;
                        canMove = true;
                        activeCharacterIndex = (activeCharacterIndex + 1) % characters.Count;

                        for (int i = 0; i < buttonsListCharacters.Count; i++)
                        {
                            if (activeButtonIndex == i )
                            {
                                buttonsListCharacters[i].interactable = false;
                                buttonsListCharacters[i].GetComponent<Image>().color = inactiveColorButton;
                            }
                        }

                        publicVariables.rangeTower = true;
                        canvas_chooseCharacter.SetActive(true);

                        canAttack = false;
                    }
                }
            }
        }
        if (publicVariables.movementsMouseController == characters.Count && path.Count == 0)
        {
            canvas_change_round.SetActive(true);
            canvas_chooseCharacter.SetActive(false);
        }
        
        
    }
    private void ActivateChooseThingToDoPanel(int characterIndex)
    {
        activeButtonIndex = characterIndex;

        btn_move.onClick.AddListener(() => MovementCharacter(characterIndex));
        btn_move.onClick.AddListener(() => canvas_chooseCharacter.SetActive(false));
        btn_move.onClick.AddListener(() => publicVariables.rangeTower = false);
        btn_move.onClick.AddListener(() => characters[activeCharacterIndex].BorrarCuadroTexto());

        
        btn_attack.onClick.AddListener(() => ShowAttackRange(characterIndex));
        btn_attack.onClick.AddListener(() => canAttack = true);
        btn_attack.onClick.AddListener(() => canvas_chooseCharacter.SetActive(false));
        btn_attack.onClick.AddListener(() => characters[activeCharacterIndex].BorrarCuadroTexto());

        btn_return.onClick.AddListener(() => DontShowAttackRange(characterIndex));

        //btn_return.onClick.AddListener(() => scriptTorreta.ShowMovementRange());

        btn_return.onClick.AddListener(() => canAttack = false);
        btn_return.onClick.AddListener(() => canvas_chooseCharacter.SetActive(true));
        canvas_chooseThingToDo.SetActive(true);

    }
    
    private void MovementCharacter(int characterIndex)
    {
        canMove = true;
        ShowMovementRange(characterIndex);
    }
    private void SetBtnChangeRound()
    {
        if (publicVariables.movementsMouseController == characters.Count)
        {
            canMove = true;
            publicVariables.movementsMouseController = 0;
            canvas_change_round.SetActive(false);
            canvas_chooseCharacter.SetActive(true);

            for (int i = 0; i < buttonsListCharacters.Count; i++)
            {
                buttonsListCharacters[i].interactable = true;
                buttonsListCharacters[i].GetComponent<Image>().color = activeColorButton;
            }

            publicVariables.myTurn = true;
            publicVariables.rangeAttackTower = true;
        }
    }


    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    private void PositionCharacterOnTile(CharacterInfo character, OverlayTile tile)
    {
        character.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        character.GetComponent<SpriteRenderer>().sortingOrder = 30;
        character.activeTile = tile;
    }

    private void ShowMovementRange(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characters.Count)
        {
            int currentMovementRange = movementCharacterRange[characterIndex];

            foreach (var item in inRangeTiles)
            {
                item.HideTile();
            }

            List<Vector2Int> occupiedPositions = new List<Vector2Int>();
            
            for (int i = 0; i < characters.Count; i++)
            {
               
                if (i != characterIndex &&  !excluidos.Contains(i) )
                {
                    occupiedPositions.Add(characters[i].activeTile.gridLocation);
                    
                }
                
            }

            inRangeTiles = rangeFinder.GetTilesInRange(characters[characterIndex].activeTile, currentMovementRange);
            foreach (var item in inRangeTiles)
            {
                if (occupiedPositions.Contains(item.gridLocation))
                {
                    item.HideTile();
                }
                else
                {
                    item.ShowTile();
                    item.GetComponent<SpriteRenderer>().color = rangeMoveColor;
                }
            }

            activeCharacterIndex = characterIndex;
        }
    }

    private void ShowAttackRange(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characters.Count)
        {
            int attackRange = attackCharacterRange[characterIndex];

            foreach (var item in inRangeTiles)
            {
                item.HideTile();
            }

            List<Vector2Int> occupiedPositions = new List<Vector2Int>();

            for (int i = 0; i < characters.Count; i++)
            {
                if (i != characterIndex ) 
                {
                    occupiedPositions.Add(characters[i].activeTile.gridLocation);
                }
            }

            occupiedPositions.Add(iaController.characterIA.activeTileIA.gridLocation);
            inRangeTiles = rangeFinder.GetTilesInRange(characters[characterIndex].activeTile, attackRange);

            foreach (var item in inRangeTiles)
            {

                if (occupiedPositions.Contains(item.gridLocation))
                {
                    item.ShowTile();
                    item.GetComponent<SpriteRenderer>().color = pointOnAttackColor;
                }
                else
                {
                    item.ShowTile();
                    item.GetComponent<SpriteRenderer>().color = rangeAttackColor;
                }

            }

            activeCharacterIndex = characterIndex;
        }

        canMove = false;
    }
    private void DontShowAttackRange(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characters.Count)
        {
            int attackRange = attackCharacterRange[characterIndex];

            foreach (var item in inRangeTiles)
            {
                item.HideTile();
            }

            inRangeTiles = rangeFinder.GetTilesInRange(characters[characterIndex].activeTile, attackRange);


        }

        canMove = false;
    }
    private bool IsValidMove(OverlayTile overlayTile)
    {
        foreach (var c in characters)
        {   
            if (c.activeTile == overlayTile && excluidos.Contains(c.index))
            {
                return true;
            }
        }
       
        if (!inRangeTiles.Contains(overlayTile))
        {
            return false;
        }

        foreach (var character in characters)
        {
            if (character != characters[activeCharacterIndex] && character.activeTile == overlayTile )
            {   
                
                return false;
            }
        }

        return true;
    }

    private bool IsValidAtack(OverlayTile overlayTile)
    {
        bool haceDaño = false;
        // Verifica si la casilla esta dentro del rango
        if (!inRangeTiles.Contains(overlayTile))
        {
            return false;
        }

        // Verifica si la casilla esta ocupada por otro personaje
        if (characters[1].activeTile == overlayTile && !excluidos.Contains(1))
        {
            personajeatacado = characters[1];
            haceDaño = true;
        }

        return haceDaño;
    }

    public List<OverlayTile> GetTilesInRange()
    {
        return inRangeTiles.ToList();
    }
}