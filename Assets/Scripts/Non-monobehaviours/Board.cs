using System.Collections.Generic;

// An instance of this class represents a game board. It is composed of
// tiles (stored in a matrix), player pieces and a ball.
public class Board
{
    // The game board stored in a matrix.
    private AbstractTile[,] tiles;

    // Store a reference to the pieces on this class' fields for
    // easy access to their position.
    private readonly PlayerPiece whitePiece1;
    private readonly PlayerPiece whitePiece2;
    private readonly PlayerPiece blackPiece1;
    private readonly PlayerPiece blackPiece2;
    private readonly Ball ball;

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
        // Store a reference to the pieces on this class' fields for
        // easy access to their position.
        this.whitePiece1 = new PlayerPiece(
            piecesX,white1Y,Team.White,PieceNumber.One);
        this.whitePiece2 = new PlayerPiece(
            piecesX,white2Y,Team.White,PieceNumber.Two);
        this.blackPiece1 = new PlayerPiece(
            piecesX,black1Y,Team.Black,PieceNumber.One);
        this.blackPiece2 = new PlayerPiece(
            piecesX,black2Y,Team.Black,PieceNumber.Two);
        this.ball        = new Ball(piecesX,ballY);

        // Create the abstract tiles and store the pieces in them.
        for (int i = 0; i < boardYLength; i++)
        {
            for (int j = 0; j < boardXLength; j++)
            {
                // Initialize a new tile and store it.
                tiles[i,j] = new AbstractTile(j, i, null);

                // The player pieces and ball go on the fifth column (x == 5).
                if (j == piecesX)
                {
                    if (i == white1Y)
                    {
                        tiles[i,j].SetPiece(whitePiece1);
                    }
                    else if (i == white2Y)
                    {
                        tiles[i,j].SetPiece(whitePiece2);
                    }
                    else if (i == black1Y)
                    {
                        tiles[i,j].SetPiece(blackPiece1);
                    }
                    else if (i == black2Y)
                    {
                        tiles[i,j].SetPiece(blackPiece2);
                    }
                    else if (i == ballY)
                    {
                        tiles[i,j].SetPiece(ball);
                    }
                }
            }
        }
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
        this.whitePiece1.SetX(piecesX);
        this.whitePiece2.SetX(piecesX);
        this.blackPiece1.SetX(piecesX);
        this.blackPiece2.SetX(piecesX);
        this.ball.SetX(piecesX);

        // Update the "y" fields of the pieces.
        this.whitePiece1.SetY(white1Y);
        this.whitePiece2.SetY(white2Y);
        this.blackPiece1.SetY(black1Y);
        this.blackPiece2.SetY(black2Y);
        this.ball.SetY(ballY);

        // Set the tiles at the new position of the pieces to reference
        // those pieces, respectively.
        GetTile(piecesX, white1Y).SetPiece(this.whitePiece1);
        GetTile(piecesX, white2Y).SetPiece(this.whitePiece2);
        GetTile(piecesX, black1Y).SetPiece(this.blackPiece1);
        GetTile(piecesX, black2Y).SetPiece(this.blackPiece2);
        GetTile(piecesX, ballY).SetPiece(this.ball);
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

    // Returns an iterator with the tiles of the board.
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
        return boardXLength;
    }

    // Getter for the number of rows on the board.
    public int GetYLength()
    {
        return boardYLength;
    }

    // Getter for the reference to the ball.
    public Ball GetBall()
    {
        return this.ball;
    }
}
