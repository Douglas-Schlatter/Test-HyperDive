using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

/// <summary>
/// Responsible for the left panel of the split view, inspecs whatever we clicked in the
/// TreeView window
/// </summary>
public class InspectorView: VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    Editor editor;
    public InspectorView()
    { 
    
    }

    
    internal void UpdateSelection(NodeView nodeView)
    {
        Clear();
        //Destroy old wrapper
        UnityEngine.Object.DestroyImmediate(editor);


        editor = Editor.CreateEditor(nodeView.treeNode);
        //Wrapper to update GUID
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        Add(container);

    }
}
