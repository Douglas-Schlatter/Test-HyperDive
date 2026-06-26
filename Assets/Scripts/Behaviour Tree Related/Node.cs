using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    //UI related
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 positionInGuid;

    //Game Context Related
    [HideInInspector] public BehaviourListener behaviourListener;
    [HideInInspector] public IAdaptable currentAdaptable;

    //State Related
    //Maybe add idle state later so it can start idle, insted of running
    public State state = State.Running;
    //Bool to check if it ever started
    public bool started = false;


    public State Update()
    {
        if (!started)
        {
            OnStart();

            started = true;
        }

        state = OnUpdate();

        //If node finished executing 
        if (state == State.Failure || state == State.Success)
        {
            OnStop();
            started = false;
        }

        return state;

    }

    public virtual Node Clone()
    {
        return Instantiate(this);

    }

    public virtual void ResetNode()
    {
        state = State.Running;
        started = false;
    }

    protected abstract void OnStart();
    protected abstract State OnUpdate();
    protected abstract void OnStop();






    /// <summary>
    /// States that a node can be
    /// </summary>
    public enum State 
    {
        Running,
        Failure,
        Success
    }
}
