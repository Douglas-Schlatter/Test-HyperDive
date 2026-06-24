using System;
using UnityEngine;
/// <summary>
/// Every class that want to use the Behaviour Tree System must implement "Adaptable"
/// <br/>
/// This interface garantees that the behaviours implemented
/// has a way to contact the behaviour listener
/// </summary>
public interface IAdaptable
{
    //After a Behvaiour tree execution trigger this
    public abstract event Action OnEndBehaviourTree;

    public BoardEntity GetBoardEntity();
    public BehaviourListener GetBehaviourTreeListener();
}
