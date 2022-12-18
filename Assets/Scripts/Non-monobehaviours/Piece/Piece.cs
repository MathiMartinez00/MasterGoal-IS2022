
// Abstract class that represents a game piece. On this game, a
// piece can be either a ball or a player piece.
public abstract class Piece
{
    // The position of the piece.
    private int x;
    private int y;

    public Piece(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int GetX(int x)
    {
        return this.x;
    }

    public int GetY(int y)
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
}
