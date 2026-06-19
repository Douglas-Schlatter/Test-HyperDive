using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

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
}
