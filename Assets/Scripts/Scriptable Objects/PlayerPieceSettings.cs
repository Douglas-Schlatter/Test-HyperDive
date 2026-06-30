using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;

[CreateAssetMenu(fileName = "PlayerPieceSettings", menuName = "Scriptable Objects/PlayerPieceSettings")]
public class PlayerPieceSettings : ScriptableObject
{
    //Move Pattern Related
    [Serializable]public enum Direction
    {
        N,NW,NL,W,L,WS,LS,S
    }

    [Serializable]
    public struct MovePattern
    {
        public List<Direction> moves;
    }
    [SerializeField] public List<MovePattern> movePatterns;

    //Behaviour Tree Related
    public BehaviourTree behaviourTree;
    [HideInInspector]public BehaviourTree runtimeBehaviourTree;

    //Runtime Variables
    //These are variables initialized at run time in "InitializeBehaviourTree"
    BehaviourListener currentBehaviourListener;
    IAdaptable currentAdaptable;

    public void InitializeBehaviourTree(BehaviourListener targetBehaviourListener, IAdaptable targetAdaptable)
    {
        //Save These references if we need to reset that tree later
        currentAdaptable = targetAdaptable;
        currentBehaviourListener = targetBehaviourListener;
        if (behaviourTree != null)
        {
            ResetBehaviourTree();
        }
        
    }



   
    public IEnumerator RunBehaviourTree()
    {
        ResetBehaviourTree();
        //while bht didn't end executing
        while (runtimeBehaviourTree.treeState == Node.State.Running)
        {
            runtimeBehaviourTree.Update();
            yield return null;
        }

        
    }

    /// <summary>
    /// After Initializing the tree, reset it every time it needs to run again.
    /// </summary>
    private void ResetBehaviourTree()
    {
        //So if the same bhTree is used in multiple objects, this garantees that each one has it's one
        //also garantees a reset
        runtimeBehaviourTree = behaviourTree.Clone();


        //AssetDatabase.SaveAssets();
        runtimeBehaviourTree.behaviourListener = currentBehaviourListener;
        runtimeBehaviourTree.currentAdaptable = currentAdaptable;
        //Pass these variables to all the nodes
        runtimeBehaviourTree.Bind();
        //if the tree was already executed before reset it
        //ResetBehaviourTree();
    }

}
