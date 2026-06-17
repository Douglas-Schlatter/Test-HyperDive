using UnityEngine;

public class BoardCell : MonoBehaviour
{
    //Position Related
    protected int posX;
    protected int posY;

    //Occupied Related
    protected BoardEntity currentEntity;
    protected bool occupied;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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

    public bool IsEmpty()
    {
        return !occupied;
    }
    public void HighLight() // Todo put an enum here for the types of highlight
    { 
        
    }
}
