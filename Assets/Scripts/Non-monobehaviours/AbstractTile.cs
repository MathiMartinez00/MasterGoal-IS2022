
#nullable enable

// The instances of this class represent tiles in the game board.
public class AbstractTile
{
    // A tile can have a piece or no pieces.
    public PlayerPiece? PlayerPiece { get; set; }
    public Ball? Ball { get; set; }
    // Coordinates of a tile.
    public int X { get; }
    public int Y { get; }
    // Property to store the validity of the tile.
    public bool IsValid { get; }
    public bool IsHighlighted { get; set; }

    // Constructor meant to be used when no piece is on the tile.
    public AbstractTile(int x, int y)
    {
        X = x;
        Y = y;
        // At the beginning of a game no tile is highlighted.
        IsHighlighted = false;
        PlayerPiece = null;
        Ball = null;

        // Check for the validity of the tile. Tiles to the sides
        // of the goals are always invalid.
        IsValid = AreCoordinatesValid(x,y) ? true : false;
    }

    // Constructor meant to be used when a player piece is on the tile.
    public AbstractTile(int x, int y, PlayerPiece playerPiece) : this(x,y)
    {
        PlayerPiece = this.IsValid ? playerPiece : null;
        Ball = null;
    }

    // Constructor meant to be used when a ball is on the tile.
    public AbstractTile(int x, int y, Ball ballPiece) : this(x,y)
    {
        Ball = this.IsValid ? ballPiece : null;
        PlayerPiece = null;
    }

    /*
    public AbstractTile(int x, int y, Piece piece) : this(x,y)
    {
        SetPiece(piece);
        //X = x;
        //Y = y;
        // At the beginning of a game no tile is highlighted.
        //IsHighlighted = false;

        // Check for the validity of the tile. Tiles to the sides
        // of the goals are always invalid.
        if (AreCoordinatesValid(x,y))
        {
            // If the tile is valid, set the piece.
            //Piece = piece;
            IsValid = true;
        }
        else
        {
            // If the tile is not valid, set the piece to null.
            //Piece = null;
            SetPiece(null);
            IsValid = false;
        }
    }

    */

    /*
    private void SetPiece(null nullValue)
    {
        PlayerPiece = null;
        Ball = null;
    }

    private void SetPiece(PlayerPiece piece)
    {
        PlayerPiece = piece;
        Ball = null;
    }

    private void SetPiece(Ball ballPiece)
    {
        PlayerPiece = null;
        Ball = ballPiece;
    }
    */

    // The tiles that are located on both sides of the goals aren't
    // valid tiles. Pieces cannot be moved on those tiles.
    private bool AreCoordinatesValid(int x, int y)
    {
        return !((x <= 2 || x >= 8) && (y == 0 || y == 14));
    }

    /*
    public Piece? Piece
    {
        get { return piece; }
        set { piece = value;
        }
    }

    // A setter for the piece field. Checks the validity of the tile
    // first. If the tile is valid, the piece is set and the method
    // returns "true"; otherwise, the piece is not set and it
    // returns "false".
    public bool SetPiece(Piece? piece)
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
    public Piece? GetPiece()
    {
        return this.piece;
    }



    // Getter for the isTileValid field.
    public bool IsTileValid()
    {
        return this.isTileValid;
    }

    // Getter for the index of the tile's column.
    public int GetX()
    {
        return this.x;
    }

    // Getter for the index of the tile's row.
    public int GetY()
    {
        return this.y;
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
    */
}
