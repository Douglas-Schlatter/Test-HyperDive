using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerPieceSettings", menuName = "Scriptable Objects/PlayerPieceSettings")]
public class PlayerPieceSettings : ScriptableObject
{
    public enum Direction
    {
        N,NW,NL,W,L,WS,LS,S
    }

    public struct MovePattern
    {
        public List<Direction> moves;
    }
    public List<MovePattern> movePatterns;
}
