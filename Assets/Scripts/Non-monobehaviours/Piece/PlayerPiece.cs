
// An instance of this class represents an abstract player piece.
// This piece can be white or black and numbered one or two.
public class PlayerPiece : Piece
{
    // There are two pieces for each team. They have to be distinguishable.
    public enum PieceNumber
    {
        One,
        Two
    }

    private Team teamColor;
    private PieceNumber pieceNumber;

    public PlayerPiece(int x, int y, Team teamColor, PieceNumber number)
    {
        // Save the position with the parent class' constructor.
        super(x,y);
        this.teamColor = teamColor;
        this.pieceNumber = pieceNumber;
    }

    // Getter for the team color field.
    public Team GetTeamColor()
    {
        return this.teamColor;
    }
}
