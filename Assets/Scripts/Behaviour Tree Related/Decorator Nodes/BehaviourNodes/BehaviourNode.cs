using UnityEngine;

/// <summary>
/// All implemented bhaviours inherit from this class, it garantees that
/// <br/> behaviours have a way to communicate with the rest of the game
/// </summary>
public abstract class BehaviourNode: DecoratorNode
{
    
    /// <summary>
    /// Most behaviours will need to executing or check something in the game, if so 
    /// mark this  as true when the execution ended
    /// </summary>
    [HideInInspector] public BehaviourState currentBehaviourState = BehaviourState.NotStarted;



    public virtual void MarkAsFinishedAndUnsubscribe()
    {
        currentBehaviourState = BehaviourState.Success;
        behaviourListener.OnBehaviourExecutionEnd -= MarkAsFinishedAndUnsubscribe;
    }

    public enum BehaviourState
    {
        NotStarted, Running, Success, Failure
    }
}
