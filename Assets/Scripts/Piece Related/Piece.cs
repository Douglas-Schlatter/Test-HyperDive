using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Piece : BoardEntity
{
    [SerializeField] protected int life;

    public override event Action OnRemove;


    /// <summary>
    ///Behaviour of moving must accept friendly fire, i would also use this
    ///function in the future when implementing the enemy
    /// </summary>
    /// <returns></returns>
    public override bool CanBeCaptured()
    {
        return true;
    }


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


    
}
