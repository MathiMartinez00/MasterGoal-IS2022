using System;

#nullable enable

// The instances of this class represent tiles in the game board.
[Serializable]
public class AbstractTile
{
    // A tile can have a piece or no pieces.
    public PlayerPiece? PlayerPiece { get; set; }
    public Ball? Ball { get; set; }
    // Coordinates of a tile.
    public int X { get; }
    public int Y { get; }
    // Property to store the validity of the tile. The only invalid
    // tiles are the tiles that are next to each of the goals.
    public bool IsValid { get; }
    // A highlighted tile indicates that a piece is allowed to move
    // there.
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

    // The tiles that are located on both sides of the goals aren't
    // valid tiles. Pieces cannot be moved on those tiles.
    private bool AreCoordinatesValid(int x, int y)
    {
        return !((x <= 2 || x >= 8) && (y == 0 || y == 14));
    }
}