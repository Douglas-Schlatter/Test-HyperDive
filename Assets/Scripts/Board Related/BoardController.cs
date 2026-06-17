using NUnit.Framework;
using System;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    //Settings and Board Generation related
    [SerializeField] protected BoardSettings boardSettings;
    int boardSizeX, boardSizeY,cellSize; //TODO MAYBE REMOVE CELLSIZE LATTER
    [SerializeField] protected GameObject cellPrefab; // ---> filled manually in the editor
    protected GameObject[,] tiles;
    [SerializeField] protected int[,] boardState; // ---> usefull for debuging 
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
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                tiles[x,y] = GenerateSingleTile(x,y);
                //probably also populate boardState here too
            }
        }
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
        //Set boardController as parent of the 
        targetCell.transform.parent = this.gameObject.transform;
        return targetCell;


    }

    //probably will need a spawn piece void later

    #endregion
}
