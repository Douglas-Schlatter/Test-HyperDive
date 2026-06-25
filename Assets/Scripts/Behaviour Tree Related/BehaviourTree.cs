using System.Collections.Generic;
using UnityEngine;
using static Node;
using UnityEditor;
using Unity.VisualScripting;
using State = Node.State;

[CreateAssetMenu(fileName = "BehaviourTree", menuName = "Scriptable Objects/BehaviourTree")]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;

    public State treeState = State.Running;

    //Nodes can be initially detached, so this is to be shure that we store all nodes
    public List<Node> nodes = new List<Node>();

    //Game Context Related
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


    #region Behaviour_Tree_Editing

    public Node CreateNode(System.Type type)
    {
        Node targetNode = ScriptableObject.CreateInstance(type) as Node;
        targetNode.name = type.Name;
        targetNode.guid = GUID.Generate().ToString();
        nodes.Add(targetNode);

        //TODO Check if this maintains after build / aka nmot in the editor 
        //Recs this Node SO to this Tree SO  
        AssetDatabase.AddObjectToAsset(targetNode, this);
        AssetDatabase.SaveAssets();
        return targetNode;
    }

    public void DeleteNode(Node targetNode)
    {
        Debug.Log("Tryed to delete");
        nodes.Remove(targetNode);
        AssetDatabase.RemoveObjectFromAsset(targetNode);
        AssetDatabase.SaveAssets();
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

   

    #endregion
}
