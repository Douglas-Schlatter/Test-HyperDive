using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    //sub windows
    BehaviourTreeView bhTreeView;
    InspectorView inspectorView;

    [MenuItem("BehaviourTreeEditor/Editor")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;


        // Instantiate UXML
        /*
           //OLD BASE CODE FROM UNITY
                VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
         */
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Behaviour Tree Related/BehaviourTreeEditor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);
        //Add StyleSheet
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Behaviour Tree Related/BehaviourTreeEditor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        //From the root searches for a bhTreeView
        bhTreeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();
        //When you call on node selected also call OnNodeSelectionChanged
        bhTreeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        //Try cas the selection thing as a behaviour tree
        BehaviourTree bhTree = Selection.activeObject as BehaviourTree;
        //If there is a bhTree and it is ready to beedited
        if (bhTree && AssetDatabase.CanOpenAssetInEditor(bhTree.GetInstanceID()))
        {
            bhTreeView.PopulateView(bhTree);
        }
    }

    protected void OnNodeSelectionChanged(NodeView node)
    {
        inspectorView.UpdateSelection(node);
    }
    
}
