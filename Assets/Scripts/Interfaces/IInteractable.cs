using UnityEngine;
using System.Collections.Generic;
using static PlayerPieceSettings;
using System;
using System.Collections;

/// <summary>
/// Every class that want to be interactable with the player must implement "Interactable"
/// </summary>
public interface IInteractable
{
    //After the piece move, trigger this
    public abstract event Action OnEndMove;

    public List<MovePattern> GetMovePatterns();

    public IEnumerator ExecuteMoves(List<Direction> moves);
    public IEnumerator DirectionalMove(Direction targetDirection);

    public InteractState CanInteract();


    public enum InteractState
    {
        canInteract,cannotInteract,interacting
    }
}
