
#nullable enable

// The instances of this class is useful to store all of the moves that
// players make.
public class Move
{
    public Team TeamColor { get; private set; }
    public AbstractTile Origin { get; private set; }
    public AbstractTile Destination { get; private set; }
    public PlayerPiece? PlayerPiece { get; private set; }
    public Ball? BallMoved { get; private set; }
    // Field that tells you if the move resulted in a goal.
    public bool IsGoal { get; set; }

    public Move(Team teamColor, AbstractTile origin, AbstractTile destination)
    {
        TeamColor = teamColor;
        Origin = origin;
        Destination = destination;
        // False by default. Can be modified with setter.
        IsGoal = false;
    }

    public Move(
        Team teamColor, AbstractTile origin, AbstractTile destination,
        PlayerPiece playerPieceMoved) : this(teamColor, origin, destination)
    {
        PlayerPiece = playerPieceMoved;
        BallMoved = null;
    }

    public Move(
        Team teamColor, AbstractTile origin, AbstractTile destination,
        Ball ball) : this(teamColor, origin, destination)
    {
        BallMoved = ball;
        PlayerPiece = null;
    }

    /*
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
    */
}