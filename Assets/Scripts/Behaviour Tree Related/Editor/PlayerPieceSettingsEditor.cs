using System;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Basic class to implement a button that creates or edits the Behaviour Tree SO
/// </summary>
[CustomEditor(typeof(PlayerPieceSettings))]
public class PlayerPieceSettingsEditor:Editor
{
    PlayerPieceSettings currentPieceSettings;
    string noTreeCreationPath = "Assets/Scripts";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        
        currentPieceSettings = (PlayerPieceSettings)target;
        if (GUILayout.Button("Edit Behaviour Tree"))
        {
            OpenBehaviourTreeEditor();
        }
    }

    protected void OpenBehaviourTreeEditor()
    {
        //If it doesn't have an Behaviour Tree create it!
        if (currentPieceSettings.behaviourTree == null)
        {
            CreateBehaviourTreeAsset();
        }

        //Open the editor with the selection on that BhTree Asset!
        BehaviourTreeEditor.OpenWindow();
        Selection.activeObject = currentPieceSettings.behaviourTree;
        EditorGUIUtility.PingObject(currentPieceSettings.behaviourTree);
    }

    protected void CreateBehaviourTreeAsset()
    {
        //Create the asset
        BehaviourTree targetBhTree = ScriptableObject.CreateInstance<BehaviourTree>();

        if (!AssetDatabase.IsValidFolder(noTreeCreationPath))
        {
            AssetDatabase.CreateFolder("Assets", "Scripts");
        }

        string newTreePath = AssetDatabase.GenerateUniqueAssetPath($"{noTreeCreationPath}/" + currentPieceSettings.name + " BehaviourTree.asset");

        AssetDatabase.CreateAsset(targetBhTree, newTreePath);

        EditorUtility.SetDirty(targetBhTree);
        currentPieceSettings.behaviourTree = targetBhTree;

        EditorUtility.SetDirty(currentPieceSettings);
        AssetDatabase.SaveAssets();

        
    }
}
