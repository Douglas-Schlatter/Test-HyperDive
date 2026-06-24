using System.Collections.Generic;
using UnityEngine;
using static Node;

[CreateAssetMenu(fileName = "BehaviourTree", menuName = "Scriptable Objects/BehaviourTree")]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;

    public State treeState = State.Running;

    public IAdaptable currentAdaptable;

    public BehaviourListener behaviourListener;

  
    public State Update()
    {
        if (rootNode.state == State.Running)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }


    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        // cast as decorator
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            children.Add(decorator.child);
        }
        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            return composite.children;
        }
        return children;
    }

    /// <summary>
    /// Travels in the tree returning its nodes
    /// </summary>
    /// <param name="node"></param>
    /// <param name="visiter"></param>
    public void Traverse(Node node, System.Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            foreach (Node child in children)
            {
                Traverse(child, visiter);
            }

        }
    }

    public void Bind()
    {
        Traverse(rootNode, InitializeNode);
    }

    public void InitializeNode(Node node)
    {
        node.behaviourListener = this.behaviourListener;
        node.currentAdaptable = this.currentAdaptable;

    }
}
