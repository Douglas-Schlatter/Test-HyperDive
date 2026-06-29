using UnityEngine;

public class BehaviorAttackAdjacentTile : BehaviourNode
{
    public PlayerPieceSettings.Direction targetDirection = PlayerPieceSettings.Direction.N;
    public int damageOfTheAttack = 1;
    [HideInInspector] public Vector2Int currentPos;
    protected override void OnStart()
    {
        currentPos = currentAdaptable.GetBoardEntity().GetPosition();
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        switch (this.currentBehaviourState)
        {
            case BehaviourState.NotStarted:
                //Set as running before starting to call attackAdjacentTile
                currentBehaviourState = BehaviourState.Running;
                
                behaviourListener.OnBehaviourExecutionEnd += MarkAsFinishedAndUnsubscribe;
                behaviourListener.AttackAdjacentTile(currentPos, targetDirection, damageOfTheAttack);
                
                return State.Running;
                break;
            case BehaviourState.Running:
                
                return State.Running;
                break;
            case BehaviourState.Success:
                //When the behaviour is marked as success with the father Class "MarkAsFinishedAndUnsubscribe", continue the behaviour sequence
               
                return ContinueSequence(State.Success);
                break;
            default:
                //This behaviour cannot fail
                return State.Success;
                break;
        }
    }
}
