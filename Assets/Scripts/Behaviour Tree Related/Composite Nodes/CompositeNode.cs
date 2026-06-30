using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Composite nodes regularly dictades the flow of the tree,
/// <br/>
/// having multiple children and deciding wich one will execute and when
/// </summary>
public abstract class CompositeNode : Node
{
    [HideInInspector] public List<Node> children = new List<Node>();

    public override Node Clone()
    {
        CompositeNode nodeClone = Instantiate(this);
        nodeClone.children = children.ConvertAll(c => c.Clone());
        return nodeClone;
    }
}
