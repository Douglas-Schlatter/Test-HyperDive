using System;
using UnityEngine;

/// <summary>
/// Responsible for responding to calls from the behaviour tree, it can be just calls for information 
/// <br/>
/// or calls for actions
/// </summary>
public class BehaviourListener : MonoBehaviour
{
    protected BoardController boardController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        boardController = FindFirstObjectByType<BoardController>();
        if (boardController == null)
        {
            Debug.LogError("The scene must have an object with a BoardController");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanMoveToDirection(Vector2Int currentPos, PlayerPieceSettings.Direction targetDirection)
    {
        return boardController.NextMoveIsValid(currentPos, targetDirection);
    }

    public void MoveToDirection(Vector2Int currentPos, PlayerPieceSettings.Direction targetDirection)
    {
        boardController.MovedByBehaviour(currentPos, targetDirection);
    }
}
