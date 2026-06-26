using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    [HideInInspector] public List<Node> children = new List<Node>();

    public override Node Clone()
    {
        CompositeNode nodeClone = Instantiate(this);
        foreach (Node child in children)
        {
            if (child != null)
            {
                nodeClone.children.Add(child.Clone());
            }
        }
        return nodeClone;
    }
}
