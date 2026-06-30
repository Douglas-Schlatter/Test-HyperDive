using UnityEngine;
using System;

/// <summary>
/// Simple boardEntity, cannot die and be captured
/// </summary>
public class Obstacle : BoardEntity
{
    public override event Action OnRemoveByAttack;


    /// <summary>
    /// Even by bheaviours or enemies obstacles can't be captured
    /// </summary>
    /// <returns></returns>
    public override bool CanBeCaptured()
    {
        return false;
    }


    //Cannot be captured
    //I would scale this using an BoardEntity Settings (SO), where 
    //you could mark if that piece can be captured or not
    public override bool CanBeCapturedByPlayer()
    {
        return false;
    }

    public override void GetCaptured()
    {
        //Do nothing objects don't take tamege or be destroied
        // put is good to have this here, for example if we want to make a specialized version of 
        //obstecle like a bomb, you can inherent obstacle nad override this function
    }

    public override bool GetHit(int damage)
    {
        //Do nothing objects don't take tamege or be destroied
        // put is good to have this here, for example if we want to make a specialized version of 
        //obstecle like a bomb, you can inherent obstacle nad override this function
        return false;
    }
}
