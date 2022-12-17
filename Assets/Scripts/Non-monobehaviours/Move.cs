
// The instances of this class is useful to store all of the moves that
// players make.
public class Move
{
    private Team teamColor;
    private AbstractTile origin;
    private AbstractTile destination;
    private Piece pieceMoved;
    // Field that tells you if the move resulted in a goal.
    private bool isGoal;

    public Move(
        Team teamColor, AbstractTile origin, AbstractTile destination, Piece pieceMoved)
    {
        this.teamColor = teamColor;
        this.origin = origin;
        this.destination = destination;
        this.pieceMoved = pieceMoved;
        this.isGoal = false;
    }

    // Origin tile getter.
    public AbstractTile GetOriginTile()
    {
        return this.origin;
    }

    // Destination tile getter.
    public AbstractTile GetDestinationTile()
    {
        return this.destination;
    }

    // Setter for the isGoal field.
    public void SetIsGoal(bool isGoal)
    {
        this.isGoal = isGoal;
    }

    // Getter for the isGoal field.
    public bool GetIsGoal()
    {
        return this.isGoal;
    }

    // Getter for the piece that has been moved.
    public Piece GetPieceMoved()
    {
        return this.pieceMoved;
    }
}