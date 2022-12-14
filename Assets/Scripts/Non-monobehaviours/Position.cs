
// This class is used to make a conversion between an Unity Vector3Int
// object and the coordinates of our game board.
public class Position
{
    private const int x;
    private const int y;

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
}