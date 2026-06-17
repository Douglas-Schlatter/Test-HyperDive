using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gives the initial settings for the generation of the Game Board
/// <br/>
/// variables like:
/// <br/>
/// cellSize, boardSize, and its possible gameobjects that will be spawned
/// </summary>
[CreateAssetMenu(fileName = "BoardSettings", menuName = "Scriptable Objects/BoardSettings")]

public class BoardSettings : ScriptableObject
{
    //Basic values for cell generation
    public int cellSize = 1;
    public int boardSizeX = 8;
    public int boardSizeY = 8;

    //List of possible spawnable objects List<BoardEntity,int >
    List<SpawnPair> PossibleSpawns;
}
/// <summary>
/// Made with struct so it can be edited in the editor directly 
/// <br/>
/// A pair (quantity to be spawned, boardEntity to be spawned)
/// </summary>
[SerializeField]public struct SpawnPair
{
    public int targetQuant;
    public BoardEntity targetEntity;

}
