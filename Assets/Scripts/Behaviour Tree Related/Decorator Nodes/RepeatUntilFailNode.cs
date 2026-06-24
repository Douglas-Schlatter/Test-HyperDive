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
        if (state == State.Failure)
        {
            return state;
        }
        else 
        {
            return State.Running;
        }
    }
}
