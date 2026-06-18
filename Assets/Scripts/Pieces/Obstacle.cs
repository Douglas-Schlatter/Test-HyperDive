using UnityEngine;
using System;

/// <summary>
/// Simple boardEntity, cannot die and be captured
/// </summary>
public class Obstacle : BoardEntity
{

    

    public override bool CanBeCaptured()
    {
        return false;
    }

    public override void GetHit()
    {
       //Do nothing objects don't take tamege or be destroied
       // put is good to have this here, for example if we want to make a specialized version of 
       //obstecle like a bomb, you can inherent obstacle nad override this function
    }
}
