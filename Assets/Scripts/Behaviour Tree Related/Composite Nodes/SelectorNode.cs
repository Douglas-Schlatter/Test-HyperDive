using UnityEngine;

/// <summary>
/// Funcitons as an "Or" statement
///Tries to execute the first chil, if it fails goes to the next
///if all children fail then returns Failure, otherwise keep updating the child that is running
/// </summary>
public class SelectorNode : CompositeNode
{
    int childIndex;
    protected override void OnStart()
    {
        childIndex = 0;
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        Node currentChild = children[childIndex];

        switch (currentChild.Update())
        {
            case State.Running:
                return State.Running;
                break;
            case State.Failure:
                childIndex++;
                break;
            case State.Success:
                return State.Success;
                break;
            default:
                break;
        }

        //If we passed through all the children and all failed, return failure
        if (childIndex == children.Count)
        {
            return State.Failure;
        }
        else
        {
            return State.Running;
        }
        
    }
}
