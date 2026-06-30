using UnityEngine;
using static BoardController;

/// <summary>
/// Class responsable for managing the player input
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] protected Camera currentCamera;

    [SerializeField] protected BoardController boardController;

    //Starts as -1 to know it would be the first click
    [SerializeField] protected Vector2Int currentSelected = new Vector2Int ( -1, -1 );


    protected void Awake()
    {
        //If not instancieted
        if (currentCamera == null)
        {
            //get camera
            currentCamera = Camera.main;
        }

        //If not instancieted
        if (boardController == null)
        {
            //Get BoardController
            boardController = FindFirstObjectByType<BoardController>();// ---> depending of the type of project this could result in problems
                                                                       // but i presume that there only will be one board in the scene
        }
    }



    void Update()
    {
        //If player click with left button
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit info;
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

            switch (boardController.GetBoardState())
            {
                case BoardState.Idle:
                    //If we clicked in a tile in BoardTile
                    if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask(CONST_DEFAULT_TILE_LAYER)))
                    {
                        ClickedOnBoardTile(info);
                    }
                    else 
                    {
                        //it could be usefull to add something for when the raycast doesn't
                        //hit any useful layer, like unhighlight everything and canceling a move
                    }
                    break;
                case BoardState.WaitingDestination:
                    //If we clicked on a END_PATH tile
                    if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask(CONST_END_PATH_LAYER)))
                    {
                        ClickedOnEndPathTile(info);
                    }
                    break;
                case BoardState.WaitingEndMove:

                    break;
                default:
                    break;
            }



        }
    }

    protected void ClickedOnEndPathTile(RaycastHit info)
    {
        Vector2Int hitPos = boardController.GetTileIndex(info.collider.gameObject);

        //Reset current selected
        currentSelected = -Vector2Int.one;
        boardController.SelectTile(hitPos);
    }

    protected void ClickedOnBoardTile(RaycastHit info)
    {
        Vector2Int hitPos = boardController.GetTileIndex(info.collider.gameObject);

        //Fisrt time that we are selecting something
        if (currentSelected == -Vector2.one)
        {
            currentSelected = hitPos;
            boardController.ChangeLayerOfTile(hitPos, CONST_SELECTED_LAYER);
            //Call boardController to do the selection logic
            boardController.SelectTile(hitPos);
        }

        //If we had one clicked, now we need to reset the old one and select the new one
        if (currentSelected != hitPos)
        {
            boardController.ChangeLayerOfTile(currentSelected, CONST_DEFAULT_TILE_LAYER);
            currentSelected = hitPos;
            boardController.ChangeLayerOfTile(hitPos, CONST_SELECTED_LAYER);
            //Call boardController to do the selection logic
            boardController.SelectTile(hitPos);
        }
    }
}
