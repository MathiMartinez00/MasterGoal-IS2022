//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class Player
{
    private const PlayerPiece piece1;
    private const PlayerPiece piece2;

    // The constructor sets the pieces. Once they are set they
    // can't be changed.
    public Player(PlayerPiece piece1, PlayerPiece piece2)
    {
        this.piece1 = piece1;
        this.piece2 = piece2;
    }

    // Getter for the first piece.
    public PlayerPiece GetPiece1()
    {
        return this.piece1;
    }

    // Getter for the second piece.
    public PlayerPiece GetPiece2()
    {
        return this.piece2;
    }
}
