using System;

#nullable enable

// The instances of this class is useful to store all of the moves that
// players make.
[Serializable]
public class Move
{
    public Team TeamColor { get; private set; }
    public AbstractTile Origin { get; private set; }
    public AbstractTile Destination { get; private set; }
    public PlayerPiece? PlayerPiece { get; private set; }
    public Ball? BallMoved { get; private set; }
    // Player move, ball pass or ball move.
    public MoveType MoveType { get; set; }
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
        // If the parameter is a PlayerPiece, a move type of type PlayerMove
        // is the only option.
        MoveType = MoveType.PlayerMove;
    }

    public Move(
        Team teamColor, AbstractTile origin, AbstractTile destination,
        Ball ball) : this(teamColor, origin, destination)
    {
        BallMoved = ball;
        PlayerPiece = null;
        // When the piece parameter is of type Ball, then the default
        // move type is a BallMove. This should be modifier later with
        // the setter, if necessary.
        MoveType = MoveType.BallMove;
    }
}