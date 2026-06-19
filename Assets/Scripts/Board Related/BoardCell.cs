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
    [SerializeField] protected bool occupied; // Left it as SerializeField beacuse is good for debuging

    //Material Related
    [SerializeField] protected Material black, grey;
    


    public void HighLight(string targetLayer) 
    {
        this.gameObject.layer = LayerMask.NameToLayer(targetLayer);
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

    public void SetOccupied(bool targetBool)
    {
        occupied = targetBool;
    }


    #region Gets_and_sets
    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }

    public BoardEntity GetBoardEntity()
    {
        if (currentEntity.Equals(null))
        {
            Debug.LogWarning("Tried to get board entity at " +posX+","+posY+ "but the space is empty");
        }
        return currentEntity;
    }





    public bool IsEmpty()
    {
        return !occupied;
    }


    public Transform GetSpawnLocation()
    {
        return spawnLocation;
    }
    #endregion

}
