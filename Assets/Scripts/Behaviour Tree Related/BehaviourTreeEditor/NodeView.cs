using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// This class serve as a visual representation of the Behaviour Tree 
/// </summary>
public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    //Visual Related
    public Port input;
    public Port output;

    //This is our architure Behaviour Tree node
    public Node treeNode;

    public NodeView(Node targetTreeNode)
    {
        this.treeNode = targetTreeNode;
        this.title = targetTreeNode.name;
        this.viewDataKey = targetTreeNode.guid;

        style.left = targetTreeNode.positionInGuid.x;
        style.top = targetTreeNode.positionInGuid.y;

        CreateInputPorts();
        CreateOutputPorts();
    }
    
    protected void CreateInputPorts()
    {
        //Check the node type and then create a Input port
        //All the cases are the same, but if you want a node that have more than 1 input, the place to implement
        //is here
        if (treeNode is ActionNode)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (treeNode is CompositeNode)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (treeNode is DecoratorNode)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }

        //If the input exist
        if (input != null)
        {
            input.portName = "";
            inputContainer.Add(input);
        }
    }

    protected void CreateOutputPorts()
    {

        //Check the node type and then create output ports

        if (treeNode is ActionNode)
        {
            //action nodes does't have children
        }
        else if (treeNode is CompositeNode)
        {
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (treeNode is DecoratorNode)
        {
            output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        }


        //If the input exist
        if (output != null)
        {
            output.portName = "";
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        treeNode.positionInGuid.x = newPos.xMin;
        treeNode.positionInGuid.y = newPos.yMin;
    }



}
