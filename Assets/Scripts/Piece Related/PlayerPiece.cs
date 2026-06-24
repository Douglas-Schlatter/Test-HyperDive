using System.Collections.Generic;
using UnityEngine;
using static IInteractable;
using static PlayerPieceSettings;
using static Helper;
using System.Collections;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;
using System;
using JetBrains.Annotations;
using UnityEditor.SearchService;

public class PlayerPiece : Piece, IAdaptable, IInteractable
{
    [SerializeField] protected PlayerPieceSettings pieceSettings;// -->> filled in the editor
    [SerializeField] protected InteractState interactState  = InteractState.canInteract;
    [SerializeField] protected float animationTime = 1.0f;

    //Used for calculating movement
    [SerializeField] protected Vector2Int deltaPos = -Vector2Int.one;

    //Behaviour tree related
    public BehaviourListener behaviourListener;

    public event Action OnEndMove;
    public event Action OnEndBehaviourTree;

    void Awake()
    {

    }

    void Start()
    {
        //starts as own position
        deltaPos = currentBoardCell.GetPosition();
        OnEndMove += ExecuteBehaviourTree;

        UpdateLayer();

        behaviourListener = UnityEngine.Object.FindFirstObjectByType<BehaviourListener>();
        if (behaviourListener == null)
        {
            Debug.LogError("The scene must have an object with a BehaviourListener");
        }

        pieceSettings.InitializeBehaviourTree(behaviourListener, this);
        //pieceSettings
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

    protected void ExecuteBehaviourTree()
    {
        //throw new NotImplementedException();
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
    public IEnumerator ExecuteMoves(List<Direction> moves)
    {
        //starts as own position
        deltaPos = currentBoardCell.GetPosition();
        foreach (Direction move in moves)
        {
            //uses deltaPos to accumulate the movement
            yield return StartCoroutine(DirectionalMove(move));
        }


        //Notify everyone that is waiting that the movement + BT is done! 
        //TODO Here it would launch and wait for the behaviour tree execution
        //probably use co routines here
        OnEndMove?.Invoke();

        yield return StartCoroutine( CallBehaviourTree());
        //TODO fazer voltar a poder ter interações so depois daqui depois
        //TODO MAKE RESET BEHAVIOUR TREE AFTER ENDING EXECUTING IT
        OnEndBehaviourTree?.Invoke();
    }

    public IEnumerator CallBehaviourTree()
    {
        this.pieceSettings.TestBHT();
        yield return StartCoroutine(pieceSettings.RunBehaviourTree());
    }

    public IEnumerator DirectionalMove(Direction targetDirection)
    {
        Vector2Int nextPosition = GetNextPosition(deltaPos, targetDirection);
        
        Debug.Log("was in"+ deltaPos + "tried to make a move to " + nextPosition);
        //Vector3 nextPosVec3 = new Vector3(nextPosition.x, this.gameObject.transform.position.y, nextPosition.y);

        //this.gameObject.transform.position = nextPosVec3; //just to test
        //Wait until Object Move to that new position //TODO LATER BE THE FUNCITION  MoveAndExecuteBT
        yield return StartCoroutine(MoveTo(deltaPos, nextPosition, animationTime));
        deltaPos = nextPosition;
    }


    public IEnumerator MoveTo(Vector2Int currentPos, Vector2Int nextPosition, float timeToMove)
    {
        Vector3 currentPosVec3 = new Vector3(currentPos.x, this.gameObject.transform.position.y, currentPos.y);//since logic is 2d but game is 3d it needs this convertion
        Vector3 nextPosVec3 = new Vector3(nextPosition.x, this.gameObject.transform.position.y, nextPosition.y);//since logic is 2d but game is 3d it needs this convertion 
        float deltaTime = 0;
        while (deltaTime< timeToMove)
        {
            //where we are at the lerp
            float t = deltaTime / timeToMove;

            this.gameObject.transform.position = Vector3.Lerp(currentPosVec3, nextPosVec3, t);
            deltaTime += Time.deltaTime;
            //wait For Next Frame
            yield return null;
        }

        // garantee to snap to nextPosition
        this.gameObject.transform.position = nextPosVec3;
    }

    public InteractState CanInteract()
    {
        return interactState;
    }

    public BehaviourListener GetBehaviourTreeListener()
    {
        //Todo 
        throw new System.NotImplementedException();
    }

    public List<MovePattern> GetMovePatterns()
    {
        return pieceSettings.movePatterns;
    }

    public override bool CanBeCapturedByPlayer()
    {
        return false;
    }

    public BoardEntity GetBoardEntity()
    {
        return this;
    }
}
