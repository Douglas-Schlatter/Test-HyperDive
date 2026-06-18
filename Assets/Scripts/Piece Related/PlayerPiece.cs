using System.Collections.Generic;
using UnityEngine;
using static Interactable;
using static PlayerPieceSettings;

public class PlayerPiece : Piece, Adaptable, Interactable
{
    [SerializeField] protected PlayerPieceSettings playerSettings;// -->> filled in the editor
    [SerializeField] protected InteractState interactState;



    void Update()
    {
        
    }

    public void SetInteractState(InteractState targetInteractState)
    {
        //update material by its interactState
        interactState = targetInteractState;
    }

    public InteractState CanInteract()
    {
        return interactState;
    }

    public BehaviourTreeListener GetBehaviourTreeListener()
    {
        //Todo 
        throw new System.NotImplementedException();
    }

    public List<MovePattern> GetMovePatterns()
    {
        return playerSettings.movePatterns;
    }
}
