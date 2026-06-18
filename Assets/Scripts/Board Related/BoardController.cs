using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Android;

public class BoardController : MonoBehaviour
{
    //Settings and Board Generation related
    [SerializeField] protected BoardSettings boardSettings;
    int boardSizeX, boardSizeY,cellSize; //TODO MAYBE REMOVE CELLSIZE LATTER
    [SerializeField] protected GameObject cellPrefab; // ---> filled manually in the editor
    protected GameObject[,] tiles;
    [SerializeField] protected int[,] boardState; // ---> usefull for debuging 
    [SerializeField] protected int entityCount;
    private void Awake()
    {
        GetStartVaraibles();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateTiles();
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
}
