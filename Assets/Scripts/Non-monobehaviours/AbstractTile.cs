
// The instances of this class represent tiles in the game board.
public class AbstractTile
{
    // A tile can have a piece or no pieces.
    private Nullable<Piece> piece;
    // Coordinates of a tile.
    private const int x;
    private const int y;
    // Property to store the validity of the tile.
    private const bool isTileValid;
    private bool isHighlighted;

    public Tile(int y, int x, Nullable<Piece> piece)
    {
        this.x = x;
        this.y = y;
        // At the beginning of a game no tile is highlighted.
        this.isHighlighted = false;

        // Check for the validity of the tile. Tiles to the sides
        // of the goals are always invalid.
        if (AreCoordinatesValid(x,y))
        {
            // If the tile is valid, set the piece.
            this.piece = piece;
            this.isTileValid = true;
        }
        else
        {
            // If the tile is not valid, set the piece to null.
            this.piece = null;
            this.isTileValid = false;
        }
    }

    // A setter for the piece field. Checks the validity of the tile
    // first. If the tile is valid, the piece is set and the method
    // returns "true"; otherwise, the piece is not set and it
    // returns "false".
    public bool SetPiece(Nullable<Piece> piece)
    {
        // Check if the tile is valid.
        if (isTileValid)
        {
            this.piece = piece;
            return true;
        }

        // Invalid tile.
        return false;
    }

    // A getter for the piece field. This method will return "null"
    // if the value is null.
    public Nullable<Piece> GetPiece()
    {
        return this.piece;
    }

    // The tiles that are located on both sides of the goals aren't
    // valid tiles. Pieces cannot be moved on those tiles.
    private bool AreCoordinatesValid(int x, int y)
    {
        return (not ((x <= 2 || x >= 8) && (y == 0 || y == 14)));
    }

    // Getter for the isTileValid field.
    public bool IsTileValid()
    {
        return this.isTileValid;
    }

    // Getter for the index of the tile's column.
    public int GetX()
    {
        this.x;
    }

    // Getter for the index of the tile's row.
    public int GetY()
    {
        this.y;
    }

    // Setter for the tile highlight field.
    public void SetHighlight(bool isHighlighted)
    {
        this.isHighlighted = isHighlighted;
    }

    // Getter for the tile highlight field.
    public bool IsTileHighlighted()
    {
        return this.isHighlighted;
    }
}
