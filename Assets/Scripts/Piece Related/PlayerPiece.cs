using System.Collections.Generic;
using UnityEngine;
using static IInteractable;
using static PlayerPieceSettings;
using static Helper;
using System.Collections;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

public class PlayerPiece : Piece, IAdaptable, IInteractable
{
    [SerializeField] protected PlayerPieceSettings playerSettings;// -->> filled in the editor
    [SerializeField] protected InteractState interactState  = InteractState.canInteract;
    [SerializeField] protected float animationTime = 3.0f;

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
    //Continue from here later
    public void DirectionalMove(Direction targetDirection)
    {
        Vector2Int currentPos =   currentBoardCell.GetPosition();
        Vector2Int nextPosition = GetNextPosition(currentPos, targetDirection);
        //Wait until Object Move to that new position //TODO LATER BE THE FUNCITION  MoveAndExecuteBT
        yield return StartCoroutine(MoveTo(currentPos, nextPosition, animationTime));
    }


    public IEnumerator MoveTo(Vector2Int currentPos, Vector2Int nextPosition, float timeToMove)
    {
        float deltaTime = 0;
        while (deltaTime< timeToMove)
        {
            //where we are at the lerp
            float t = deltaTime / timeToMove;

            this.gameObject.transform.position = Vector3.Lerp(currentPos.ConvertTo<Vector3>(), nextPosition.ConvertTo<Vector3>(), t);
            deltaTime += Time.deltaTime;
            //wait For Next Frame
            yield return null;
        }

        // garantee to snap to nextPosition
        this.gameObject.transform.position = nextPosition.ConvertTo<Vector3>();
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

    public override bool CanBeCapturedByPlayer()
    {
        return false;
    }


}
