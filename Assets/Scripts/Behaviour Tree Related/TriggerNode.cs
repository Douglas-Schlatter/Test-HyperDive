using UnityEngine;

/// <summary>
/// This is the root node of the behaviour tree, has no parents and only one child
/// <br/>
/// It is from here that the Behaviour Tree starts to get executed
/// </summary>
public class TriggerNode : Node
{

    [HideInInspector] public Node child;
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
       
    }

    protected override State OnUpdate()
    {
        return child.Update();
    }

    public override Node Clone()
    {
        TriggerNode nodeClone = Instantiate(this);
        if (nodeClone.child != null)
        {
            nodeClone.child = child.Clone();
        }
        return nodeClone;
    }
}
