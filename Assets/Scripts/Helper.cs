using UnityEngine;
using static PlayerPieceSettings;

public  static class Helper 
{
    /// <summary>
    /// Given an original position and a direction to go to, returns the new position
    /// </summary>
    /// <returns></returns>
    public  static Vector2Int GetNextPosition(Vector2Int startingPosition, Direction direction)
    {
        switch (direction)
        {
            //Move Up
            case Direction.N:
                return new Vector2Int(startingPosition.x, startingPosition.y + 1);
                break;

            //Move UpRight
            case Direction.NW:
                return new Vector2Int(startingPosition.x - 1, startingPosition.y + 1);
                break;

            //Move UpLeft
            case Direction.NL:
                return new Vector2Int(startingPosition.x + 1, startingPosition.y + 1);
                break;

            //Move Left
            case Direction.W:
                return new Vector2Int(startingPosition.x - 1, startingPosition.y);
                break;

            //Move Right
            case Direction.L:
                return new Vector2Int(startingPosition.x + 1, startingPosition.y);
                break;

            //Move DownLeft
            case Direction.WS:
                return new Vector2Int(startingPosition.x - 1, startingPosition.y - 1);
                break;

            //Move DownRight
            case Direction.LS:
                return new Vector2Int(startingPosition.x + 1, startingPosition.y - 1);
                break;

            //Move Down
            case Direction.S:
                return new Vector2Int(startingPosition.x, startingPosition.y - 1);
                break;
            default:
                return new Vector2Int(startingPosition.x, startingPosition.y);
                break;

        }

    }
}
