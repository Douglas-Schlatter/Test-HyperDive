using UnityEngine;


/// <summary>
/// This class serve as a visual representation of the Behaviour Tree 
/// </summary>
public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    //This is our architure Behaviour Tree node
    public Node treeNode;

    public NodeView(Node targetTreeNode)
    {
        this.treeNode = targetTreeNode;
        this.title = targetTreeNode.name;
    }
}
