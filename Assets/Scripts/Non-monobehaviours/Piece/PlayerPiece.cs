//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class PlayerPiece : Piece
{
    private Team teamColor;

    public PlayerPiece(Team teamColor)
    {
        this.teamColor = teamColor;
    }

    // Getter for the team color field.
    public Team GetTeamColor()
    {
        return this.teamColor;
    }
}
