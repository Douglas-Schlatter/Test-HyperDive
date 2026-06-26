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

    public BehaviourTree behaviourTree;
    public BehaviourTree runtimeBehaviourTree;

    public void InitializeBehaviourTree(BehaviourListener targetBehaviourListener, IAdaptable targetAdaptable)
    {
        //behaviourTree = ScriptableObject.CreateInstance<BehaviourTree>();// TODO REMOVE LATER JUST FOR TESTING
        behaviourTree = behaviourTree.Clone();
        runtimeBehaviourTree = behaviourTree.Clone();

        //AssetDatabase.SaveAssets();
        runtimeBehaviourTree.behaviourListener = targetBehaviourListener;
        runtimeBehaviourTree.currentAdaptable = targetAdaptable;
        //Pass these variables to all the nodes
        runtimeBehaviourTree.Bind();
    }

    // TODO REMOVE LATER JUST FOR TESTING
    public void TestBHT()
    {
        

        var log1 = ScriptableObject.CreateInstance<DebugLogNode>();

        log1.message = "Sup1";

        var log2 = ScriptableObject.CreateInstance<DebugLogNode>();

        log2.message = "Sup2";

        log1.child = log2;

        var log3 = ScriptableObject.CreateInstance<DebugLogNode>();

        log3.message = "Sup3";

        log2.child = log3;

        var bh = ScriptableObject.CreateInstance<BehaviorMovePiece>();
        bh.targetDirection = Direction.S;
        //var rpt = ScriptableObject.CreateInstance<RepeatUntilFailNode>();
        //rpt.child = log;
        behaviourTree.rootNode = bh;

        //yield return StartCoroutine(RunBehaviourTree);
        behaviourTree.Bind();
    }

    public IEnumerator RunBehaviourTree()
    {
        //while bht didn't end executing
        while (runtimeBehaviourTree.treeState == Node.State.Running)
        {
            behaviourTree.Update();
            yield return null;
        }
        
    }


}
