using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Linq;
using System.Collections.Generic;

public class BehaviourTreeView: GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

    public BehaviourTree currentBhTree;

    public Action<NodeView> OnNodeSelected;
    public BehaviourTreeView()
    {
        //Add grid
        Insert(0, new GridBackground());

        //Basic manipulators
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());



        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Behaviour Tree Related/BehaviourTreeEditor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }


    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //TODO here if i have spare time, have to implement with Reflection


        //get all nodes that derive from actionNodes
        ShowTypesInTheDropDownMenu(evt, typeof(ActionNode));

        //get all nodes that derive from compositeNodes
        ShowTypesInTheDropDownMenu(evt, typeof(CompositeNode));

        //get all nodes that derive from decoratorNodes
        ShowTypesInTheDropDownMenu(evt, typeof(DecoratorNode));

    }

    /// <summary>
    /// given the event of contextual menu, and a type, insert the option to create a nodeView of that type,
    /// <br/> if that type is an abstract type, will recusivly added the derived types to the menu
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="targetType"></param>
    private void ShowTypesInTheDropDownMenu(ContextualMenuPopulateEvent evt,System.Type targetType)
    {
        var decoratorTypes = TypeCache.GetTypesDerivedFrom(targetType);
        foreach (Type type in decoratorTypes)
        {
            //If it an abstract class
            if (!type.IsAbstract)
            {
                //for each drop down action that will be spawned in the menu
                //assing the action of creating a Node of that type
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
            else
            {
                //if it is abstract show the derived from that abstract
                ShowTypesInTheDropDownMenu(evt, type);
            }
        }
    }


    /// <summary>
    /// Given a behaviour tree generates an equivalant in the Editor
    /// </summary>
    /// <param name="bhTree"></param>
    /// <exception cref="NotImplementedException"></exception>
    public  void PopulateView(BehaviourTree bhTree)
    {
        this.currentBhTree = bhTree;

        graphViewChanged -= OnGraphViewChanged;

        //Clear board
        DeleteElements(graphElements);

        graphViewChanged += OnGraphViewChanged;
        //IF we don't have an triggerNode or rootNode, create one!
        if (currentBhTree.rootNode == null)
        { 
            currentBhTree.rootNode = currentBhTree.CreateNode(typeof(TriggerNode)) as TriggerNode;
            EditorUtility.SetDirty(currentBhTree);
            AssetDatabase.SaveAssets();
        }

        //Creating nodeviews
        foreach (Node node in currentBhTree.nodes)
        {
            CreateNodeView(node);
        }


        //Creates edges by getting its parent and child, and then connecting them
        foreach (Node parentNode in currentBhTree.nodes)
        {
            var children = currentBhTree.GetChildren(parentNode);
            if (children.Count > 0)
            {
                foreach (Node childNode in children)
                {
                    if (childNode != null)
                    {
                        NodeView parentView = FindNodeView(parentNode);
                        NodeView childView = FindNodeView(childNode);

                        Edge newEdge = parentView.output.ConnectTo(childView.input);
                        AddElement(newEdge);
                    }
                }
            }
        }

    }


    /// <summary>
    /// This is how we check if someting changed in the graph
    /// </summary>
    /// <param name="graphViewChange"></param>
    /// <returns></returns>
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        
        if (graphViewChange.elementsToRemove != null)
        {
            //Get the list of things to be removed
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                //If it is a node that we are deleting
                NodeView nodeView = elem as NodeView;
                if (nodeView != null)
                {
                    currentBhTree.DeleteNode(nodeView.treeNode);
                }
                //If it is an edge that we are deleting
                Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    currentBhTree.RemoveChild(parentView.treeNode, childView.treeNode);
                }

            });
        }

        //Detect when creating an edge add it
        if (graphViewChange.edgesToCreate != null)
        {
            foreach (var edge in graphViewChange.edgesToCreate)
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                currentBhTree.AddChild(parentView.treeNode, childView.treeNode);
            }
        }


        return graphViewChange;
    }

    /// <summary>
    /// Creates a node from the passed type
    /// </summary>
    /// <param name="type"></param>
    protected void CreateNode(System.Type type)
    {
        Node node = currentBhTree.CreateNode(type);
        CreateNodeView(node);

    }
    protected void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        //bubble the event up in the structure
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    /// <summary>
    /// From an architeture bhTreeNode get its equivalant NodeView in the GUID
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        //Itarete over every element in the list garantees that inpu with input can't
        //connect to each other, same for out with out.
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction &&
        endPort.node != startPort.node
        ).ToList();
    }
}
