using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Android;
using static PlayerPieceSettings;
using static Helper;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;

public class BoardController : MonoBehaviour
{
    //Settings and Board Generation related
    [SerializeField] protected BoardSettings boardSettings;
    int boardSizeX, boardSizeY,cellSize; //TODO MAYBE REMOVE CELLSIZE LATTER
    [SerializeField] protected GameObject cellPrefab; // ---> filled manually in the editor
    protected GameObject[,] tiles;
    protected int[,] boardState; // ---> usefull for debuging  // TODO maybe will take it out later if only the cellScriptLogic is used
    [SerializeField] protected int entityCount;

    //Move Related
    [SerializeField]  protected Vector2Int lastSelected = -Vector2Int.one; // ---> usefull for debuging
    IInteractable lastSelectedPiece;
    BoardCell targetCellScript;

    /// <summary>
    /// After validating possible moves from a piece, add all the moves that the player can make here
    /// <br/>
    /// later when player clicks on a piece access with the position of the move with O(1) complexity
    /// </summary>
    protected Dictionary<Vector2Int, MovePattern> validMoves = new Dictionary<Vector2Int, MovePattern>();

    //BoardState related
    [SerializeField] protected BoardState currentBoardState = BoardState.Idle;

    //Layers Related
    public const string CONST_DEFAULT_TILE_LAYER = "BoardTiles";
    public const string CONST_INTERACTABLE_LAYER = "InteractableHighlight";
    public const string CONST_SELECTED_LAYER = "SelectedHighlight";
    public const string CONST_PATH_LAYER = "PathHighlight";
    public const string CONST_END_PATH_LAYER = "EndPathHightLight";

    //Event Related
    public event Action OnLockPlayerInteraction, OnLockChoosePiece;
    private void Awake()
    {
        GetStartVaraibles();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateTiles();
        PrintBoardState();
    }



    // Update is called once per frame
    void Update()
    {
    }


    #region BoardGeneration
    protected void GetStartVaraibles() // TODO maybe change this to initialize variables?
    {
        if (boardSettings.Equals(null))
        {
            Debug.LogError("BoardController doesn't have BoardSettings!");
            return;
        }
        //Variables
        boardSizeX = boardSettings.boardSizeX;
        boardSizeY = boardSettings.boardSizeY;
        cellSize = boardSettings.cellSize;

        //Populate arrays
        tiles = new GameObject[boardSizeX, boardSizeY];
        boardState = new int[boardSizeX, boardSizeY];
        //get cell prefab
        this.cellPrefab = boardSettings.cellPrefab;
    }

    protected void GenerateTiles() /// todo make a generate board state
    {
        
        List<GameObject> possiblePieces = PopulatePossiblePieces();



        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                //generate the tile
                tiles[x,y] = GenerateSingleTile(x,y);

                GameObject currentCellObj = tiles[x, y];
                //Populate tile
                int index = UnityEngine.Random.Range(0, possiblePieces.Count); 

                GameObject targetSpawnPrefab = possiblePieces[index];
                possiblePieces.RemoveAt(index);

                if (targetSpawnPrefab != null) // if not drawed an empty space
                {
                    SpawnObjectInCell(currentCellObj, targetSpawnPrefab);
                }


            }
        }
    }


    protected void SpawnObjectInCell(GameObject currentCellObj, GameObject targetSpawnPrefab)
    {
        BoardEntity targetBoardEntity = targetSpawnPrefab.GetComponent<BoardEntity>();
        //Check if prefab has BoardEntity
        if (targetBoardEntity != null)
        {
            //Check if has boardEntity
            //Spawn and link with the cell
            BoardCell targetCellScript = currentCellObj.GetComponent<BoardCell>();

            //OPTIMIZATION here i would implement pooling -> GetObjectFromPool 
            GameObject spawnGameObj = Instantiate(targetSpawnPrefab, targetCellScript.GetSpawnLocation().transform.position, targetCellScript.GetSpawnLocation().transform.rotation);
            BoardEntity objBoardEntity = spawnGameObj.GetComponent<BoardEntity>();
            //testing not puting them as child
            spawnGameObj.transform.parent = currentCellObj.transform;

            targetCellScript.SetOccupied(true);
            //set the spawned object board entity to this cell
            targetCellScript.SetBoardEntity(objBoardEntity);
            //Set boardEntity cell as this one as well
            targetCellScript.GetBoardEntity().SetBoardCell(targetCellScript);
            //Update boardState to notify it has an object there
            boardState[targetCellScript.GetPosition().x, targetCellScript.GetPosition().y] = 1; 
        }
        else
        {
            Debug.LogError("The prefab " + targetSpawnPrefab.name + " tring to be spawned does not have a BoardEntity component!");

        }
    }

    /// <summary>
    /// Prepares a list simulating a bag of pieces, that can have all the objectes in BoardSettings PossibleSpawns + empty spaces
    /// </summary>
    /// <param name="possiblePieces"></param>
    /// <exception cref="NotImplementedException"></exception>
    List<GameObject> PopulatePossiblePieces()
    {
        
        //for each sp add that many gameObj in the bag
        List<GameObject> possiblePieces = new();
        foreach (SpawnPair sp in boardSettings.PossibleSpawns)
        {
            //Here it would be good to make a check if targetEntityGameObject has
            //an boardEntity component,if not throw an error, but the way this is treated is dependend
            // on the company you are working for, so i only annoted it
            entityCount += sp.targetQuant;
            for (int i = 0;  i < sp.targetQuant; i++)
            {
                possiblePieces.Add(sp.targetEntityGameObject);
            }
        }

        int tileSpaces = boardSizeX * boardSizeY;
        if (entityCount >= tileSpaces)
        {
            Debug.LogWarning("The quantity of pieces you want to spawn is equal or bigger then the size of the board!" +
                "/n Only a selection of them will be spawned");
        }
        else //if tileSize -pieces >0 it means there is empty spaces to simulate in the bag
        {
            int emptySpaces = tileSpaces - entityCount;
            for (int i = 0; i < emptySpaces; i++)
            {
                possiblePieces.Add(null);
            }
        }


            return possiblePieces;

    }


    /// <summary>
    /// Spawn a tile in determined location in relation of the parent transform
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    protected GameObject GenerateSingleTile(int posX, int posY)
    {

        // Calculate the new tile position based on this object
        Vector3 targetPosition = new Vector3(this.transform.position.x + posX, this.transform.position.y, this.transform.position.z + posY); // --->since board is 2d but the project is  3D we need to make this adjustment
        GameObject targetCell = Instantiate(cellPrefab, targetPosition, cellPrefab.transform.rotation);
        targetCell.name = "BoardCell_" + posX + "_" + posY;


        //Set boardController as parent of the tile
        targetCell.transform.parent = this.gameObject.transform;


        // Check if the tile has boardCell Component
        BoardCell targetCellScript;
        if (cellPrefab.GetComponent<BoardCell>().Equals(null))
        {
            Debug.LogWarning("The prefab used for the tile cells doesn't have a boardCell component! Added as a component");
            targetCellScript  = targetCell.AddComponent<BoardCell>();

        }
        else
        {
            targetCellScript = targetCell.GetComponent<BoardCell>();
        }
        //Initialize targetCellScript
        targetCellScript.SetPosition(posX, posY);
        targetCellScript.SetOccupied(false);
        targetCellScript.ResetMaterial();

        OnLockPlayerInteraction += targetCellScript.ResetLayer;

        return targetCell;

    }


    //probably will need a spawn piece void later

    #endregion

    #region BoardComunication


    /// <summary>
    /// Given an GameObject of a cell,  returns it index
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public Vector2Int GetTileIndex(GameObject gameObject)
    {
        BoardCell cellScript = gameObject.GetComponent<BoardCell>();
        return cellScript.GetPosition();
    }

    /// <summary>
    /// Changes the layer of a tile, used in the highlight of the tiles
    /// </summary>
    /// <param name="targetTilePos"></param>
    /// <param name="targetLayer"></param>
    public void ChangeLayerOfTile(Vector2Int targetTilePos, string targetLayer)
    {
        tiles[targetTilePos.x, targetTilePos.y].layer = LayerMask.NameToLayer(targetLayer);
    }

    public void SelectTile(Vector2Int targetTilePos)
    {
        //verify is the tile is ocuppied, verifiy if it implements movable, verify state, if movable and interacable show moves 

        

        switch (currentBoardState)
        {
            //Player is selecting a piece
            case BoardState.Idle:
                lastSelected = targetTilePos;
                targetCellScript = tiles[targetTilePos.x, targetTilePos.y].gameObject.GetComponent<BoardCell>();
                //check if occupied
                if (!targetCellScript.IsEmpty())
                {
                    //check if object is movable by the player
                    if (targetCellScript.GetBoardEntity().TryGetComponent<IInteractable>(out IInteractable interactable))
                    {
                        //if movable go WaintingForDestination state and show possible moves
                        ShowPossibleMoves(interactable.GetMovePatterns(), targetTilePos);
                        //Change to WaitingDestination
                        currentBoardState = BoardState.WaitingDestination;
                    }
                    else
                    {
                        //Do nothing
                        return;
                    }
                }
                break;
            case BoardState.WaitingDestination:
                //If player clikced in a valid space
                if (validMoves.ContainsKey(targetTilePos))
                {
                    //Lock player interaction while piece moves, reset all layers of the cells
                    OnLockPlayerInteraction?.Invoke();

                    //Make Series of Moves
                    validMoves.TryGetValue(targetTilePos, out MovePattern targetPattern);
                    MakeMoves(targetCellScript, targetPattern);
                    //Trafers the entity from the previews location to the new one
                    TranferEntityBetweenCells(tiles[lastSelected.x, lastSelected.y].GetComponent<BoardCell>(), tiles[targetTilePos.x, targetTilePos.y].GetComponent<BoardCell>());


                    //Change to WaitingEndMove // TODO TEST THIS, I THINK THAT WHEN IT ENDS MOVING IT WOULD IMIDIATLY GO TO DILE
                    currentBoardState = BoardState.WaitingEndMove;
                }
                else
                {
                    //Do nothing
                    return;
                }
                break;
            case BoardState.WaitingEndMove:
                //Do nothing you are waiting the move to end, but it is good to have
                //a space to impklement stuff that happens in this moment
                //for example if we want to implement clickable powers while pieces move
                break;
            default:
                break;
        }


    }

    /// <summary>
    /// given a boardCell and a series of moves, makes the piece in the boardCell make that series of moves
    /// </summary>
    /// <param name="targetCellScript"></param>
    /// <param name="targetPattern"></param>
    private void MakeMoves(BoardCell targetCellScript, MovePattern targetPattern)
    {
        //cell cannot be empty
        if (!targetCellScript.IsEmpty())
        {
            IInteractable movablePiece = targetCellScript.GetBoardEntity().GetComponent<IInteractable>();
            lastSelectedPiece = movablePiece;

            movablePiece.OnEndInteraction += GoToIdle;

            StartCoroutine(movablePiece.ExecuteMoves(targetPattern.moves));
        }
        else 
        {
            //This should not happen, but this cell is empty
            Debug.LogError("The cell you are trtrying to execute 'Make moves' is empty");
        }
    }





    /// <summary>
    /// Transfer BoardEntity from one Board Cell to another
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="targetCellScript"></param>
    protected void TranferEntityBetweenCells(BoardCell lastSelected, BoardCell nextSelected)
    {
        //change parent
        lastSelected.GetBoardEntity().transform.parent = nextSelected.transform;

        //removes last selected form its previous owner
        lastSelected.SetOccupied(false);
        BoardEntity targetBoardEntity = lastSelected.GetBoardEntity();
        lastSelected.SetBoardEntity(null);
        //Update boardState to notify it has an object there
        boardState[lastSelected.GetPosition().x, lastSelected.GetPosition().y] = 0;


        //Check if it is a capture here!

        nextSelected.SetOccupied(true);
        nextSelected.SetBoardEntity(targetBoardEntity);
        //Set boardEntity cell as this one as well
        targetBoardEntity.SetBoardCell(nextSelected);
        //Update boardState to notify it has an object there
        boardState[nextSelected.GetPosition().x, nextSelected.GetPosition().y] = 1;
    }

    /// <summary>
    /// Given possible moves and a targetPosition, higlights the cells of that moves
    /// </summary>
    protected void ShowPossibleMoves(List<MovePattern> movePatterns, Vector2Int targetTilePos)
    {
        foreach (MovePattern pattern in movePatterns)
        {
            ShowPattern(pattern, targetTilePos);
        }
    }

    /// <summary>
    /// Given an Pattern and a targetPosition, higlights the cells of that moves
    /// </summary>
    protected void ShowPattern(MovePattern pattern, Vector2Int startingPosition)
    {

        //initialize move pattern that will be saved on validMoves
        MovePattern saveMovePattern = new MovePattern();
        saveMovePattern.moves = new List<Direction>();

        int patternSize = pattern.moves.Count;
        Vector2Int nextPosition = startingPosition;
        List<Vector2Int> positionToHighlight = new List<Vector2Int>();
        for (int i = 0; i < patternSize; i++)
        {
            //Check if the move is valid
            if(!NextMoveIsValid(nextPosition, pattern.moves[i]))
            {
                //if not a valid move because it goes out of the board just return and dont highlight anything
                return;
            }

            nextPosition = GetNextPosition(nextPosition, pattern.moves[i]);
            BoardCell nextCellScript = tiles[nextPosition.x, nextPosition.y].GetComponent<BoardCell>();

            //Save the move that you did to get to this position
            saveMovePattern.moves.Add(pattern.moves[i]);

            //if this is the lastMove
            if (i == patternSize - 1)
            {
                //Is this ocuppied?
                if (!nextCellScript.IsEmpty())
                {
                    //If the entity occuping this place can be captured by the player
                    if (nextCellScript.GetBoardEntity().CanBeCapturedByPlayer())
                    {
                        //if there is a piece here and it can be captured this is the end of this pattern
                        HighlightAndUpdateValidMoves(nextPosition, positionToHighlight, saveMovePattern);

                        return;
                    }
                    else
                    {
                        //we encountered something we cant capture!
                        //return without highlighting this path
                        return;
                    }
                }
                else 
                {
                    //Nothing here, just the end of the path, highlight all the stored path + last location
                    HighlightAndUpdateValidMoves(nextPosition, positionToHighlight, saveMovePattern);

                    return;
                }

            }
            else 
            {
                //it is a path location

                //Is this ocuppied?
                if (!nextCellScript.IsEmpty())
                {
                    //If the entity occuping this place can be captured by the player
                    if (nextCellScript.GetBoardEntity().CanBeCapturedByPlayer())
                    {
                        //if there is a piece here and it can be captured this is the end of this pattern
                        HighlightAndUpdateValidMoves(nextPosition, positionToHighlight, saveMovePattern);

                        return;
                    }
                    else
                    {
                        //we encountered something we cant capture!
                        //return without highlighting this path
                        return;
                    }
                }
                else 
                {
                    //it is just an emprty space in the path, so add this location to be highlighted later
                    //if all goes well
                    positionToHighlight.Add(nextPosition);

                }

            }
        }
    }

    /// <summary>
    /// As well as higlighting paths, adds the last path position in the dicionary of valid postions to click
    /// </summary>
    /// <param name="nextPosition"></param>
    /// <param name="positionToHighlight"></param>
    protected void HighlightAndUpdateValidMoves(Vector2Int nextPosition, List<Vector2Int> positionToHighlight, MovePattern saveMovePattern)
    {
        foreach (Vector2Int cell in positionToHighlight)
        {
            tiles[cell.x, cell.y].GetComponent<BoardCell>().HighLight(CONST_PATH_LAYER);
        }

        tiles[nextPosition.x, nextPosition.y].GetComponent<BoardCell>().HighLight(CONST_END_PATH_LAYER);
        
        //If this position wasn't already added
        if (!validMoves.ContainsKey(nextPosition))
        {
            //Co relates this end position to this series of moves
            validMoves.Add(nextPosition, saveMovePattern);
        }
       


    }

    /// <summary>
    /// This resets the boardStateMachine, it is activated when the piece ends its
    /// move + Behaviour Tree execution
    /// </summary>
    public void GoToIdle()
    {

        //reset last selected
        lastSelectedPiece.OnEndInteraction -= GoToIdle;
        //lastSelected = -Vector2Int.one;
        targetCellScript = null;
        lastSelectedPiece = null;
        currentBoardState = BoardState.Idle;
    }

    #region MoveCalculation




    /// <summary>
    /// Given an original position and a direction to go to, return true if it is a valid position in the board
    /// </summary>
    protected bool NextMoveIsValid(Vector2Int startingPosition, Direction direction)
    {
        //TODO LATER CHECK FOR OCUPING THE SAME SPACE RULES AND BEING CAPTURED RULES
        switch (direction)
        {
            case Direction.N:
                //Invalid position out of the board
                if (startingPosition.y + 1 >= boardSizeY)
                {
                    return false;
                }
                else 
                {
                    return true;
                }
                break;
            case Direction.NW:
                //Invalid position out of the board
                if ((startingPosition.y + 1 >= boardSizeY) ||( startingPosition.x -1 < 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case Direction.NL:
                //Invalid position out of the board
                if ((startingPosition.y + 1 >= boardSizeY) || (startingPosition.x + 1 >= boardSizeX))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case Direction.W:
                //Invalid position out of the board
                if ((startingPosition.x - 1 < 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case Direction.L:
                //Invalid position out of the board
                if ((startingPosition.x + 1 >= boardSizeX))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case Direction.WS:
                //Invalid position out of the board
                if ((startingPosition.x - 1 < 0)|| (startingPosition.y -1 < 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case Direction.LS:
                //Invalid position out of the board
                if ((startingPosition.x + 1 >= boardSizeX) || (startingPosition.y - 1 < 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            case Direction.S:
                //Invalid position out of the board
                if ((startingPosition.y - 1 < 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                break;
            default:
                return false;
                break;

        }

    }
    #endregion
    #endregion

    /// <summary>
    /// Debugging print boardState, made with AI
    /// </summary>
    protected void PrintBoardState()
    {
        if (boardState == null)
        {
            Debug.Log("BoardState is null");
            return;
        }

        string output = "Board State:\n";

        for (int y = boardSizeY - 1; y >= 0; y--) // Print top row first
        {
            for (int x = 0; x < boardSizeX; x++)
            {
                output += boardState[x, y] + " ";
            }
            output += "\n";
        }

        Debug.Log(output);
    }

    



    /// <summary>
    /// Idle --> Wainting for player to click on an interactive piece
    /// <br/>
    /// WaitingDestination --> Wainting for player to click on a possible endPathLocation
    /// <br/>
    /// WaitingEndMove -->Waits for notify from the piece that ended move + behaviour tree execution
    /// </summary>
    protected enum BoardState
    {
        Idle, WaitingDestination, WaitingEndMove //In the future here i would implement "EnemyTurn" state to make the enemies turn in the game
    }
}
