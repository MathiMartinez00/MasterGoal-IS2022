using System.Collections.Generic;
using System;

// An instance of this class represents a game board. It is composed of
// tiles (stored in a matrix), player pieces and a ball.
[Serializable]
public class Board
{
    // The game board stored in a matrix. Declare the matrix.
    private AbstractTile[,] tiles = new AbstractTile[
        boardYLength, boardXLength];

    // Store a reference to the pieces on this class' fields for
    // easy access to their position.
    public PlayerPiece WhitePiece1 { get; private set; }
    public PlayerPiece WhitePiece2 { get; private set; }
    public PlayerPiece BlackPiece1 { get; private set; }
    public PlayerPiece BlackPiece2 { get; private set; }
    public Ball Ball { get; private set; }

    // Dimension of the board matrix.
    private const int boardXLength = 11;
    private const int boardYLength = 15;

    // Constants for the starting coordinates of each piece.
    public readonly int piecesX = 5;
    public readonly int white1Y = 2;
    public readonly int white2Y = 4;
    public readonly int black1Y = 10;
    public readonly int black2Y = 12;
    public readonly int ballY = 7;

    // Board's constructor method. Creates a new board for a new game.
    public Board()
    {
        // Store a reference to the pieces on this class' fields for
        // easy access to their position.
        WhitePiece1 = new PlayerPiece(
            piecesX,white1Y,Team.White,PieceNumber.One);
        WhitePiece2 = new PlayerPiece(
            piecesX,white2Y,Team.White,PieceNumber.Two);
        BlackPiece1 = new PlayerPiece(
            piecesX,black1Y,Team.Black,PieceNumber.One);
        BlackPiece2 = new PlayerPiece(
            piecesX,black2Y,Team.Black,PieceNumber.Two);
        Ball        = new Ball(piecesX,ballY);

        // Create the abstract tiles and store the pieces in them.
        for (int i = 0; i < boardYLength; i++)
        {
            for (int j = 0; j < boardXLength; j++)
            {
                // Initialize a new tile and store it.
                tiles[i,j] = new AbstractTile(j, i);

                // The player pieces and ball go on the fifth column (x == 5).
                if (j == piecesX)
                {
                    if (i == white1Y)
                        tiles[i,j].PlayerPiece = WhitePiece1;
                    else if (i == white2Y)
                        tiles[i,j].PlayerPiece = WhitePiece2;
                    else if (i == black1Y)
                        tiles[i,j].PlayerPiece = BlackPiece1;
                    else if (i == black2Y)
                        tiles[i,j].PlayerPiece = BlackPiece2;
                    else if (i == ballY)
                        tiles[i,j].Ball = Ball;
                }
            }
        }
    }

    // Resets the pieces to their initial position (after a goal,
    // for example).
    public void ResetPieces()
    {
        // Set the tiles at the current position of the pieces
        // to null.
        GetTile(WhitePiece1).PlayerPiece = null;
        GetTile(WhitePiece2).PlayerPiece = null;
        GetTile(BlackPiece1).PlayerPiece = null;
        GetTile(BlackPiece2).PlayerPiece = null;
        GetTile(Ball).Ball = null;

        // Update the "x" fields of the pieces.
        WhitePiece1.X = piecesX;
        WhitePiece2.X = piecesX;
        BlackPiece1.X = piecesX;
        BlackPiece2.X = piecesX;
        Ball.X = piecesX;

        // Update the "y" fields of the pieces.
        WhitePiece1.Y = white1Y;
        WhitePiece2.Y = white2Y;
        BlackPiece1.Y = black1Y;
        BlackPiece2.Y = black2Y;
        Ball.Y = ballY;

        // Set the tiles at the new position of the pieces to reference
        // those pieces, respectively.
        GetTile(piecesX, white1Y).PlayerPiece = WhitePiece1;
        GetTile(piecesX, white2Y).PlayerPiece = WhitePiece2;
        GetTile(piecesX, black1Y).PlayerPiece = BlackPiece1;
        GetTile(piecesX, black2Y).PlayerPiece = BlackPiece2;
        GetTile(piecesX, ballY).Ball = Ball;
    }
    
    // A getter method for a tile at a given pair of coordinates.
    public AbstractTile GetTile(int x, int y)
    {
        return this.tiles[y,x];
    }

    // Another getter method for the tiles.
    public AbstractTile GetTile(Piece piece)
    {
        return GetTile(piece.X, piece.Y);
    }

    // Returns an iterator with the valid tiles of the board.
    public IEnumerable<AbstractTile> GetIterativeTiles()
    {
        for (int i = 0; i < boardYLength; i++)
        {
            for (int j = 0; j < boardXLength; j++)
            {
                // Get the tile and check if it's valid.
                AbstractTile tile = GetTile(j, i);
                if (tile.IsValid)
                    // Return the tile iteratively.
                    yield return tile;
            }
        }
    }

    // Returns an iterator with the highlighted tiles of the board.
    public IEnumerable<AbstractTile> GetHighlightedTilesIterative()
    {
        // Iterate through the abstract tiles and return the highlighted ones.
        foreach(AbstractTile tile in GetIterativeTiles())
        {
            if (tile.IsHighlighted)
                // Return the tile iteratively.
                yield return tile;
        }
    }

    // Clear all of the highlights on the tiles.
    public void ClearAllHighlights()
    {
        for (int i = 0; i < boardYLength; i++)
        {
            for (int j = 0; j < boardXLength; j++)
                GetTile(j,i).IsHighlighted = false;
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

    // Takes a player piece of a certain color and returns the other
    // piece of the same color.
    public PlayerPiece GetTheOtherPieceOfTheSameTeam(PlayerPiece piece)
    {
        if (piece == WhitePiece1)
            return WhitePiece2;
        else if (piece == WhitePiece2)
            return WhitePiece1;
        else if (piece == BlackPiece1)
            return BlackPiece2;
        else
            return BlackPiece1;
    }
}