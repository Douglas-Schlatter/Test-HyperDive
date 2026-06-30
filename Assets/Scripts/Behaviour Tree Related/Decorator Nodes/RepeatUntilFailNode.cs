using UnityEngine;

public class RepeatUntilFailNode : DecoratorNode
{


    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        state = child.Update();

        switch (state)
        {
            case State.Running:
                return State.Running;
                break;
            case State.Failure:
                return state;
                break;
            case State.Success:
                child.ResetNode();
                return State.Running;
                break;
            default:
                return State.Running;
                break;
        }
    }
}
