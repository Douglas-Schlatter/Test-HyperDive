using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System;

public class BehaviourTreeView: GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

    public BehaviourTree currentBhTree;
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

        //Clear board
        DeleteElements(graphElements);

        foreach (Node node in currentBhTree.nodes)
        {
            CreateNodeView(node);
        }
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
        AddElement(nodeView);
    }
}
