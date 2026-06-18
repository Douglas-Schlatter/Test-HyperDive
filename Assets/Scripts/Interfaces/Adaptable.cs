using UnityEngine;
/// <summary>
/// Every class that want to use the Behaviour Tree System must implement "Adaptable"
/// <br/>
/// This interface garantees that the behaviours implemented
/// has a way to contact the behaviour listener
/// </summary>
public interface Adaptable
{
    public BehaviourTreeListener GetBehaviourTreeListener();
}
