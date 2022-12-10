//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class Move
{
    private Player player;
    private Tile origin;
    private Tile destination;
    private Piece pieceToMove;

    public Move(Player player, Tile origin, Tile destination)
    {
        this.player = player;
        this.origin = origin;
        this.destination = destination;
        // The GetPiece method returns an instance of Nullable.
        this.pieceToMove = origin.GetPiece.Value;
    }

    // Origin tile getter.
    public Tile GetOriginTile()
    {
        return origin;
    }

    // Destination tile getter.
    public Tile GetDestinationTile()
    {
        return destination;
    }
}
