
// The instances of this class is useful to store all of the moves that
// players make.
public class Move
{
    private Team teamColor;
    private Tile origin;
    private Tile destination;
    private Piece pieceMoved;
    // Field that tells you if the move resulted in a goal.
    private bool isGoal;

    public Move(
        Team teamColor, Tile origin, Tile destination, Piece pieceMoved)
    {
        this.teamColor = teamColor;
        this.origin = origin;
        this.destination = destination;
        this.pieceMoved = pieceMoved;
        this.isGoal = false;
    }

    // Origin tile getter.
    public Tile GetOriginTile()
    {
        return this.origin;
    }

    // Destination tile getter.
    public Tile GetDestinationTile()
    {
        return this.destination;
    }

    // Setter for the isGoal field.
    public void SetIsGoal(bool isGoal)
    {
        this.isGoal = isGoal;
    }
}