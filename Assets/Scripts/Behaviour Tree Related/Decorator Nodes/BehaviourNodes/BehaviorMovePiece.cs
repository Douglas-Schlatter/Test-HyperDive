using UnityEngine;

public class BehaviorMovePiece : BehaviourNode
{
    public  PlayerPieceSettings.Direction targetDirection = PlayerPieceSettings.Direction.N;
    protected override void OnStart()
    {

    }
    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        Vector2Int currentPos = currentAdaptable.GetBoardEntity().GetPosition();
        if (behaviourListener.CanMoveToDirection(currentPos, targetDirection))
        {
            behaviourListener.MoveToDirection(currentPos, targetDirection);
            return State.Success;
        }
        else 
        {
            return State.Failure;
        }
    }
}
