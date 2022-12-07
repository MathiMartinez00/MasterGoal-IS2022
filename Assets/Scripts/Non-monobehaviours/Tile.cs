//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class Tile
{
    // A tile can have a piece or no pieces.
    private Nullable<Piece> piece;
    // Coordinates of a tile.
    private const int x;
    private const int y;
    // Property to store the validity of the tile.
    private const bool isTileValid;

    public Tile(int x, int y, Nullable<Piece> piece)
    {
        this.x = x;
        this.y = y;
        this.piece = piece;
        this.isTileValid = AreCoordinatesValid(x,y);
    }

    // A setter for the piece field. Checks the validity of the tile
    // first. If the tile is valid, the piece is set and the method
    // returns "true"; otherwise, the piece is not set and it
    // returns "false".
    public bool SetPiece(Piece piece)
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

    // The tiles that are located on both sides of the goals aren't
    // valid tiles. Pieces cannot be moved on those tiles.
    private bool AreCoordinatesValid(int x, int y)
    {
        return (not ((x <= 2 || x >= 8) && (y == 0 || y == 14)));
    }
}
