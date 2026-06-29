using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class BehaviorMovePiece : BehaviourNode
{
    public  PlayerPieceSettings.Direction targetDirection = PlayerPieceSettings.Direction.N;
    public Vector2Int currentPos;
    protected override void OnStart()
    {
        currentPos = currentAdaptable.GetBoardEntity().GetPosition();
    }
    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {

        switch (currentBehaviourState)
        {
            case BehaviourState.NotStarted:
                //If it is the first time executing
                //check if you can do your behaviour
                //subcribre to check when its done
                //and execute it
                if (behaviourListener.CanMoveToDirection(currentPos, targetDirection))
                {
                    behaviourListener.OnBehaviourExecutionEnd += MarkAsFinishedAndUnsubscribe;
                    behaviourListener.MoveToDirection(currentPos, targetDirection);
                    currentBehaviourState = BehaviourState.Running;
                    return State.Running;
                }
                else
                {
                    currentBehaviourState = BehaviourState.Failure;
                    return State.Failure;
                }
            case BehaviourState.Running:
                return State.Running;
                break;
            case BehaviourState.Success:
                //When the behaviour is marked as success with the father Mark method  return success in the next update
                return State.Success;
                //TODO Make it execute child and return its state later!
                break;
            case BehaviourState.Failure:
                //this should never reach here, but is good redundenci anyways
                return State.Failure;
                break;
            default:
                return State.Failure;
                break;
        }



    }


}
