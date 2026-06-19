using System.Collections.Generic;
using UnityEngine;
using static IInteractable;
using static PlayerPieceSettings;

public class PlayerPiece : Piece, IAdaptable, IInteractable
{
    [SerializeField] protected PlayerPieceSettings playerSettings;// -->> filled in the editor
    [SerializeField] protected InteractState interactState  = InteractState.canInteract;


    void Start()
    {
        UpdateLayer();
    }
    void Update()
    {
        
    }

    public void SetInteractState(InteractState targetInteractState)
    {
        //update material by its interactState
        interactState = targetInteractState;
        UpdateLayer();
    }

    /// <summary>
    /// I am using color coding pieces by the render pipiline, depending of the layer
    /// a different color will appear
    /// </summary>
    private void UpdateLayer()
    {
        switch (interactState)
        {
            case InteractState.canInteract:
                ChangeLayer("InteractableHighlight");

                break;
            case InteractState.cannotInteract:
                ChangeLayer("Default");
                break;
            case InteractState.interacting:
                ChangeLayer("InteractableHighlight");
                break;
            default:
                ChangeLayer("Default");
                break;
        }
    }

    /// <summary>
    /// Changes this obj layer, and all its children too
    /// <br/>
    /// This is needed because the player pieces have multiple subforms using unity prefabs,
    ///so we need to change the children layer too
    /// </summary>
    /// <param name="targetLayer"></param>
    private void ChangeLayer(string targetLayer)
    {
        this.gameObject.layer = LayerMask.NameToLayer(targetLayer);

        //Since i am making the player pieces having multiple subforms using unity prefabs,
        //i need to change children layer too
        foreach (Transform child in this.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(targetLayer);
        }
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
