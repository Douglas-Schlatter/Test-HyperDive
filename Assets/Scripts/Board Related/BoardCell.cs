using System;
using System.Net.Sockets;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BoardCell : MonoBehaviour
{
    //Position Related
    protected int posX;
    protected int posY;

    //Occupied Related
    protected BoardEntity currentEntity;
    [SerializeField] protected Transform spawnLocation; //--->  filled in the editor usefull
    protected bool occupied;

    //Material Related
    [SerializeField] protected Material black, grey;
    


    public void HighLight() // Todo put an enum here for the types of highlight
    {
        throw new NotImplementedException();
    }

    public void ResetMaterial()
    {
        if ((posX + posY) % 2 == 0)
        {
            this.gameObject.GetComponent<MeshRenderer>().material = grey;
        }
        else
        {
            this.gameObject.GetComponent<MeshRenderer>().material = black;
        }
    }

    protected void EntityRemoved()
    {
        //my entity got removed
        currentEntity = null;
        occupied = false;
    }


    #region Gets_and_sets
    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(posX, posY);
    }

    public BoardEntity GetBoardEntity()
    {
        if (currentEntity.Equals(null))
        {
            Debug.LogWarning("Tried to get board entity at " +posX+","+posY+ "but the space is empty");
        }
        return currentEntity;
    }

    public void SetBoardEntity(BoardEntity targetEntity)
    {
        //Unsubrcribe from the previews entity
        if (currentEntity != null)
        {
            currentEntity.OnRemove -= EntityRemoved;
        }
        //Update and Subcribe to the new entity
        targetEntity.OnRemove += EntityRemoved;
        this.currentEntity = targetEntity;

    }



    public bool IsEmpty()
    {
        return !occupied;
    }
    public void SetOccupied(bool targetBool)
    {
        occupied = targetBool;
    }

    public Transform GetSpawnLocation()
    {
        return spawnLocation;
    }
    #endregion

}
