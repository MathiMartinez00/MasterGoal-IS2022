
// Enum to differentiate between different types of moves.
public enum MoveType
{
    // A move of a player piece.
    PlayerMove,
    // A ball move that constitutes a pass.
    BallPass,
    // A ball move that doesn't constitutes a pass.
    // This move ends a turn.
    BallMove
}