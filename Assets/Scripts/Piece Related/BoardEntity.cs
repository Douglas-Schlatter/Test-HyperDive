using System;
using UnityEngine;

/// <summary>
/// Base Asbstract class for classes that can be on the board 
/// </summary>
public abstract class BoardEntity : MonoBehaviour
{
    [SerializeField] protected BoardCell currentBoardCell;


    // needed to notify boardcell that is now empty,
    // in the future pieces can be removed for many reason
    // (Piece death,Piece movement, leaves options for destructuble objects to be implemented if wanted)
    public abstract event Action OnRemoveByAttack;  //todo maybe rename to on death, because movable situations passes through the boardController anyways

    public BoardCell GetBoardCell() 
    {
        return currentBoardCell;
    }
    public Vector2Int GetPosition()
    {
        return GetBoardCell().GetPosition();
    }

    public void SetBoardCell(BoardCell targetCell)
    {
        //this.gameObject.transform.parent = targetCell.gameObject.transform;
        currentBoardCell = targetCell;
    }

    /// <summary>
    /// Tells the entity to get hit, then:
    /// <br/>
    /// if the piece is destroied returns true
    /// <br/>
    /// if the piece doesn't get destroid returns true
    /// </summary>
    /// <param name="damage"></param>
    public abstract bool GetHit(int damage);

    public abstract void GetCaptured();

    public abstract bool CanBeCapturedByPlayer();

    public abstract bool CanBeCaptured();
}
