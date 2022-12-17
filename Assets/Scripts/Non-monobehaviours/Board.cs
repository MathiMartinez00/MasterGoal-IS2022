
// An instance of this class represents a game board. It is composed of
// tiles (stored in a matrix), player pieces and a ball.
public class Board
{
    // The game board stored in a matrix.
    private AbstractTile[][] tiles;

    // Store a reference to the pieces on this class' fields for
    // easy access to their position.
    private const PlayerPiece whitePiece1;
    private const PlayerPiece whitePiece2;
    private const PlayerPiece blackPiece1;
    private const PlayerPiece blackPiece2;
    private const Ball ball;

    // Dimension of the board matrix.
    private const int boardXLength = 11;
    private const int boardYLength = 15;

    // Constants for the starting coordinates of each piece.
    private const int piecesX = 5;
    private const int white1Y = 2;
    private const int white2Y = 4;
    private const int black1Y = 10;
    private const int black2Y = 12;
    private const int ballY = 7;

    // Board's constructor method. Creates a new board for a new game.
    public Board()
    {
        for (int i = 0; i < boardYLength; i++)
        {
            for (int j = 0; j < boardXLength; j++)
            {
                // Initialize a new tile and store it.
                tiles[i,j] = new Tile(i, j, null);

                // The player pieces and ball go on the fifth column (x == 5).
                if (j == this.piecesX)
                {
                    if (i == white1Y || i == white2Y)
                    {
                        tiles[i,j].piece = new PlayerPiece(j,i,White);
                    }
                    else if (i == black1Y || i == black2Y)
                    {
                        tiles[i,j].piece = new PlayerPiece(j,i,Black);
                    }
                    else if (i == ballY)
                    {
                        tiles[i,j].piece = new Ball(j,i);
                    }
                }
            }
        }

        // Store a reference to the pieces on this class' fields for
        // easy access to their position.
        this.whitePiece1 = GetTile(this.piecesX, this.white1Y);
        this.whitePiece2 = GetTile(this.piecesX, this.white2Y);
        this.blackPiece1 = GetTile(this.piecesX, this.black1Y);
        this.blackPiece2 = GetTile(this.piecesX, this.black2Y);
        this.ball        = GetTile(this.piecesX, this.ballY);
    }

    // Resets the pieces to their initial position (after a goal,
    // for example).
    private void ResetPieces()
    {
        // Set the tiles at the current position of the pieces
        // to null.
        GetTile(this.whitePiece1).SetPiece(null);
        GetTile(this.whitePiece2).SetPiece(null);
        GetTile(this.blackPiece1).SetPiece(null);
        GetTile(this.blackPiece2).SetPiece(null);
        GetTile(this.ball).SetPiece(null);

        // Update the "x" fields of the pieces.
        this.whitePiece1.SetX(this.piecesX);
        this.whitePiece2.SetX(this.piecesX);
        this.blackPiece1.SetX(this.piecesX);
        this.blackPiece2.SetX(this.piecesX);
        this.ball.SetX(this.piecesX);

        // Update the "y" fields of the pieces.
        this.whitePiece1.SetY(this.white1Y);
        this.whitePiece2.SetY(this.white2Y);
        this.blackPiece1.SetY(this.black1Y);
        this.blackPiece2.SetY(this.black2Y);
        this.ball.SetY(this.ballY);

        // Set the tiles at the new position of the pieces to reference
        // those pieces, respectively.
        GetTile(this.piecesX, this.white1Y).SetPiece(this.whitePiece1);
        GetTile(this.piecesX, this.white2Y).SetPiece(this.whitePiece2);
        GetTile(this.piecesX, this.black1Y).SetPiece(this.blackPiece1);
        GetTile(this.piecesX, this.black2Y).SetPiece(this.blackPiece2);
        GetTile(this.piecesX, this.ballY).SetPiece(this.ball);
    }
    
    // A getter method for a tile at a given pair of coordinates.
    public AbstractTile GetTile(int x, int y)
    {
        return this.tiles[y,x];
    }

    // Another getter method for the tiles.
    public AbstractTile GetTile(Piece piece)
    {
        return GetTile(piece.GetX(), piece.GetY());
    }

    public IEnumerable<AbstractTile> GetIterativeTiles()
    {
        for (int i = 0; i < boardYLength; i++)
        {
            for (int j = 0; j < boardXLength; j++)
            {
                // Get the tile and check if it's valid.
                AbstractTile tile = GetTile(j, i);
                if (tile.IsTileValid())
                {
                    // Return the tile iteratively.
                    yield return tile;
                }
            }
        }
    }

    // Clear all of the highlights on the tiles.
    public void ClearAllHighlights()
    {
        for (int i = 0; i < boardYLength; i++)
        {
            for (int j = 0; j < boardXLength; j++)
            {
                GetTile(j,i).SetHighlight(false);
            }
        }
    }

    // Getter for the number of columns on the board.
    public int GetXLength()
    {
        return this.boardXLength;
    }

    // Getter for the number of rows on the board.
    public int GetYLength()
    {
        return this.boardYLength;
    }

    // Getter for the reference to the ball.
    public Ball GetBall()
    {
        return this.ball;
    }
}
