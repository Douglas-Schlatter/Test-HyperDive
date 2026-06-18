using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Piece : BoardEntity
{
    protected int life;

    public override event Action OnRemove;


    /// <summary>
    ///For the default result returns true, but can be overwrited later 
    /// </summary>
    /// <returns></returns>
    public override bool CanBeCapturedByPlayer()
    {
        return true;
    }


    /// <summary>
    /// In some cases even though a piece can't get captured, it can be foced on it
    /// example: friendly fire with move behaviour
    /// </summary>
    public override void GetCaptured()
    {
        OnRemove?.Invoke();

        //OPTIMIZATION here i would implement pool logic -> returnObjToPool
        Destroy(this.gameObject);
    }


    public override bool GetHit(int damage)
    {
        //I died :c
        if ((life - damage) <= 0)
        {
            OnRemove?.Invoke();

            //OPTIMIZATION here i would implement pool logic -> returnObjToPool
            Destroy(this.gameObject);
            return true;
        }
        else
        {
            //I didn't die!
            return false;
        }
    }

    public void MoveTo(Direction direction)
    { 
    
    }
    
}
