//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class PlayerPiece : Piece
{
    private Team teamColor;

    public PlayerPiece(int x, int y, Team teamColor)
    {
        // Save the position with the parent class' constructor.
        super(x,y);
        this.teamColor = teamColor;
    }

    // Getter for the team color field.
    public Team GetTeamColor()
    {
        return this.teamColor;
    }
}
