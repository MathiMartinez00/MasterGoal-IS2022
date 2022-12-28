using System;

// An instance of this class represents an abstract player piece.
// This piece can be white or black and numbered one or two.
[Serializable]
public class PlayerPiece : Piece
{
    public Team TeamColor { get; private set; }
    public PieceNumber PieceNumber { get; private set; }

    public PlayerPiece(
        int x, int y, Team teamColor, PieceNumber number) : base(x,y)
    {
        // (Save the position with the parent class' constructor).

        this.TeamColor = teamColor;
        this.PieceNumber = number;
    }
}