using UnityEngine;

/// <summary>
/// This class would keep updating its child unitl fail, this is a bonus class from the implementation
/// currently doesn't do anything
/// </summary>
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

        /*
         * 
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
         */
        return State.Success;
    }
}
