using UnityEngine;

/// <summary>
/// Base Asbstract class for classes that can be on the board 
/// </summary>
public abstract class BoardEntity : MonoBehaviour
{
    protected BoardCell currentBoardCell;




    public BoardCell GetBoardCell() 
    {
        return currentBoardCell;
    }

    public void SetBoardCell(BoardCell targetCell)
    {
        currentBoardCell = targetCell;
    }

    public abstract void GetHit();

    public abstract bool CanBeCaptured();
}
