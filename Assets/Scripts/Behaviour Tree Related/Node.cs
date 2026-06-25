using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    //UI related
    public string guid;
    public Vector2 positionInGuid;

    //Game Context Related
    public BehaviourListener behaviourListener;
    public IAdaptable currentAdaptable;

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
