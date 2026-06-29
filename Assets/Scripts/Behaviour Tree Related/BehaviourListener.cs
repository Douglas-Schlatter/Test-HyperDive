using System;
using UnityEngine;
using static PlayerPieceSettings;

/// <summary>
/// Responsible for responding to calls from the behaviour tree, it can be just calls for information 
/// <br/>
/// or calls for actions
/// </summary>
public class BehaviourListener : MonoBehaviour
{
    protected BoardController boardController;

    public event Action OnBehaviourExecutionEnd;

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

    public bool CanMoveToDirection(Vector2Int currentPos, Direction targetDirection)
    {
        return boardController.NextMoveIsValid(currentPos, targetDirection);
    }

    public void MoveToDirection(Vector2Int currentPos, Direction targetDirection)
    {
        boardController.OnBehaviourExecutionEnd += PassTheEventCall;
        boardController.MovedByBehaviour(currentPos, targetDirection);
    }

    public void AttackAdjacentTile(Vector2Int currentPos, Direction targetDirection, int damage)
    {
        boardController.OnBehaviourExecutionEnd += PassTheEventCall;
        boardController.BehaviourAttackAdjacentTile(currentPos, targetDirection, damage);
    }


    public void PassTheEventCall()
    {
        OnBehaviourExecutionEnd?.Invoke();
        boardController.OnBehaviourExecutionEnd -= PassTheEventCall;
    }
}
