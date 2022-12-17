
// This class is used to make a conversion between an Unity Vector3Int
// object and the coordinates of our game board, one way or the other.
public class Position
{
    private const int x;
    private const int y;
    private const Vector3Int vector3Int;
    private const Vector3 vector3;

    // The dimensions of the board.
    private const boardXLength = 11;
    private const boardYLength = 15;

    public Position(Vector3Int vector)
    {
        // In the implementation, the X axis has the origin in the
        // middle of the board.
        this.x = vector.x + ((this.boardXLength - 1) / 2);

        // In the implementation, the Y axis has the origin in the
        // middle of the board and it doesn't use the computer graphics
        // convention of going from top to bottom, in increasing order.
        this.y = ((this.boardYLength - 1) / 2) - vector.y;

        // No need to convert this.
        this.vector3Int = vector;

        // Convert the Vector3Int to a Vector3 with float components.
        this.vector3 = new Vector((float)vector.x, (float)vector.y);
    }

    public Position(int x, int y)
    {
        // No need to convert these.
        this.x = x;
        this.y = y;

        // Convert from abstract coordinates to concrete coordinates.
        int vectorX = x + (this.boardXLength - 1) / 2;
        int vectorY = (this.boardYLength - 1) / 2 - y;

        // Initialize a new Vector3Int object and store it.
        this.vector3 = new Vector3Int(vectorX, vectorY);

        // Initialize a new Vector3 object with float components.
        this.vector3 = new Vector((float)vectorX, (float)vectorY);
    }

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
}