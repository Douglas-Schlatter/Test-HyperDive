using UnityEngine;
using System.Collections.Generic;
using static PlayerPieceSettings;

/// <summary>
/// Every class that want to be interactable with the player must implement "Interactable"
/// </summary>
public interface Interactable
{
    
    public List<MovePattern> GetMovePatterns();

    public InteractState CanInteract();

    public enum InteractState
    {
        canInteract,cannotInteract,interacting
    }
}
