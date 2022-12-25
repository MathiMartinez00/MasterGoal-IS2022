using UnityEngine;

// This class is used to make a conversion between an Unity Vector3Int
// object and the coordinates of our game board, one way or the other.
public class Position
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Vector3Int Vector3Int { get; private set; }
    //public Vector3 Vector3 { get; private set; }

    // The dimensions of the board.
    private const int boardXLength = 11;
    private const int boardYLength = 15;

    public Position(Vector3Int vector)
    {
        // In the implementation, the X axis has the origin in the
        // middle of the board.
        X = vector.x + ((boardXLength - 1) / 2) + 1;

        // In the implementation, the Y axis has the origin in the
        // middle of the board and it doesn't use the computer graphics
        // convention of going from top to bottom, in increasing order.
        Y = ((boardYLength - 1) / 2) - vector.y - 1;

        // No need to convert this.
        Vector3Int = vector;

        // Convert the Vector3Int to a Vector3 with float components.
        //Vector3 = new Vector3((float)vector.x, (float)vector.y);
    }

    public Position(int x, int y)
    {
        // No need to convert these.
        X = x;
        Y = y;

        // Convert from abstract coordinates to concrete coordinates.
        int vectorX = x - ((boardXLength - 1) / 2 + 1);
        int vectorY = (boardYLength - 1) / 2 - (y + 1);

        // Initialize a new Vector3Int object and store it.
        Vector3Int = new Vector3Int(vectorX, vectorY);

        // Initialize a new Vector3 object with float components.
        //Vector3 = new Vector3((float)vectorX, (float)vectorY);
    }

    /*
    // Getter for the x coordinate.
    public int GetX()
    {
        return this.x;
    }

    // Getter for the y coordinate.
    public int GetY()
    {
        return this.y;
    }

    // Getter for the Vector3Int field.
    public Vector3Int GetVector3Int()
    {
        return this.vector3Int;
    }

    // Getter for the Vector3 field.
    public Vector3 GetVector3()
    {
        return this.vector3;
    }
    */
}