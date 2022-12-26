using UnityEngine;

// This class is used to make a conversion between an Unity Vector3Int
// object and the coordinates of our game board, one way or the other.
public class Position
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Vector3Int Vector3Int { get; private set; }

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
    }
}