using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Android;
using static PlayerPieceSettings;

public class BoardController : MonoBehaviour
{
    //Settings and Board Generation related
    [SerializeField] protected BoardSettings boardSettings;
    int boardSizeX, boardSizeY,cellSize; //TODO MAYBE REMOVE CELLSIZE LATTER
    [SerializeField] protected GameObject cellPrefab; // ---> filled manually in the editor
    protected GameObject[,] tiles;
    [SerializeField] protected int[,] boardState; // ---> usefull for debuging 
    [SerializeField] protected int entityCount;

    //BoardState related
    [SerializeField] protected BoardState currentBoardState;
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
        if (targetBoardEntity != null)
        {
            //Check if has boardEntity
            //Spawn and link with the cell
            BoardCell targetCellScript = currentCellObj.GetComponent<BoardCell>();

            //OPTIMIZATION here i would implement pooling -> GetObjectFromPool 
            GameObject spawnGameObj = Instantiate(targetSpawnPrefab, targetCellScript.GetSpawnLocation().transform.position, targetCellScript.GetSpawnLocation().transform.rotation);
            spawnGameObj.transform.parent = currentCellObj.transform;

            targetCellScript.SetOccupied(true);
            targetCellScript.SetBoardEntity(targetBoardEntity);
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
        BoardCell targetCellScript = tiles[targetTilePos.x, targetTilePos.y].gameObject.GetComponent<BoardCell>();


        switch (currentBoardState)
        {
            //Player is selecting a piece
            case BoardState.Idle:
                //check if occupied
                if (!targetCellScript.IsEmpty())
                {
                    //check if object is movable by the player
                    if (gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
                    {
                        //if movable go WaintingForDestination state and show possible moves
                        ShowPossibleMoves(interactable.GetMovePatterns(), targetTilePos);
                        //Lock player selecting new pieces
                        //Change to WaitingDestination
                    }
                    else
                    {
                        return;
                    }
                }
                break;
            case BoardState.WaitingDestination:
                break;
            case BoardState.WaitingEndMove:
                break;
            default:
                break;
        }


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

        
        int patternSize = pattern.moves.Count;

        List<Vector2Int> positionToHighlight;
        for (int i = 0; i < patternSize; i++)
        {
            //TODO LATER CHECK FOR OCUPING THE SAME SPACE RULES AND BEING CAPTURED RULES,maybe variable friendly fire
            //Check if the move is valid
            if(!NextMoveIsValid(startingPosition, pattern.moves[i]))
            {
                //if not a valid move because it goes out of the board just return
                return;
            }

            Vector2Int nextPosition = GetNextPosition(startingPosition, pattern.moves[i]);
            BoardCell nextCellScript = tiles[nextPosition.x, nextPosition.y].GetComponent<BoardCell>();
            //if this is the lastMove
            if (i == patternSize - 1)
            {
                //it is an end path location
                
                
            }
            else 
            {
                //it is a path location

                //Is this ocuppied?
                if (tiles[nextPosition.x, nextPosition.y])
                {

                }

            }
        }
    }


    #region MoveCalculation
    protected void MakeMoves()
    { }

    /// <summary>
    /// Given an original position and a direction to go to, returns the new position
    /// </summary>
    /// <returns></returns>
    protected Vector2Int GetNextPosition(Vector2Int startingPosition, Direction direction)
    {
        switch (direction)
        {
            //Move Up
            case Direction.N:
                return new Vector2Int(startingPosition.x, startingPosition.y + 1);
                break;

            //Move UpRight
            case Direction.NW:
                return new Vector2Int(startingPosition.x - 1, startingPosition.y + 1);
                break;

            //Move UpLeft
            case Direction.NL:
                return new Vector2Int(startingPosition.x + 1, startingPosition.y + 1); 
                break;

            //Move Left
            case Direction.W:
                return new Vector2Int(startingPosition.x - 1, startingPosition.y);
                break;

            //Move Right
            case Direction.L:
                return new Vector2Int(startingPosition.x + 1, startingPosition.y);
                break;

            //Move DownLeft
            case Direction.WS:
                return new Vector2Int(startingPosition.x - 1, startingPosition.y-1);
                break;

            //Move DownRight
            case Direction.LS:
                return new Vector2Int(startingPosition.x + 1, startingPosition.y - 1);
                break;

            //Move Down
            case Direction.S:
                return new Vector2Int(startingPosition.x, startingPosition.y - 1);
                break;
            default:
                return new Vector2Int(startingPosition.x, startingPosition.y);
                break;

        }

    }

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
