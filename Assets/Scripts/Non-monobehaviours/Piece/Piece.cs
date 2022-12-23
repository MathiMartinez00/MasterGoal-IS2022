
// Abstract class that represents a game piece. On this game, a
// piece can be either a ball or a player piece.
public abstract class Piece
{
    // The position of the piece.
    public int X { get; set; }
    public int Y { get; set; }

    public Piece(int x, int y)
    {
        X = x;
        Y = y;
    }

    /*
    public int GetX()
    {
        return this.x;
    }

    public int GetY()
    {
        return this.y;
    }

    public void SetX(int x)
    {
        this.x = x;
    }

    public void SetY(int y)
    {
        this.y = y;
    }
    */
}
