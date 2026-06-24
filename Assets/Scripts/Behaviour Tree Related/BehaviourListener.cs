using System;
using UnityEngine;

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
