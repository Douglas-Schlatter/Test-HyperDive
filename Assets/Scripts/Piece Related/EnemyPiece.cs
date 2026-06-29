using UnityEngine;

/// <summary>
/// It is empty because for now the enemy does not have any custom behaviour in relation
/// to the base piece, but in the future, it would make sense to implement him in a specialized way
/// </summary>
public class EnemyPiece : Piece
{
    protected void Awake()
    {
        //here i would put an BaseEnemySettings so that the enemy stats could be configurable
        life = 1;
    }
}
