using UnityEngine;

/// <summary>
/// Nodes that have only one child. In this category are nodes that 
/// <br/>
/// alter the functionality or result of it's child, or they are Behaviours of the game
/// </summary>
public abstract class DecoratorNode: Node
{
    [HideInInspector] public Node child;
    /// <summary>
    /// if you have a child, execute him and propagate its result,
    /// <br/> otherwise, return parameter state (this is a leaf node)
    /// </summary>
    /// <returns></returns>
    public State ContinueSequence(State noChildReturn)
    {
        if (child != null)
        {
            return child.Update();
        }
        else
        {
            //if you dont have a child 
            return State.Success;
        }
    }

    public override Node Clone()
    {
        DecoratorNode nodeClone = Instantiate(this);
        if (nodeClone.child != null)
        {
            nodeClone.child = child.Clone();
        }
        return nodeClone;
    }

}
