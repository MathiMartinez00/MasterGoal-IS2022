
// The instances of this class is useful to store all of the moves that
// players make.
public class Move
{
    private Team teamColor;
    private Tile origin;
    private Tile destination;
    private Piece pieceMoved;

    public Move(
        Team teamColor, Tile origin, Tile destination, Piece pieceMoved)
    {
        this.teamColor = teamColor;
        this.origin = origin;
        this.destination = destination;
        this.pieceMoved = pieceMoved;
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
}
