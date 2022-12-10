//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class Board
{
    private Tile[][] tiles;

    // Dimension of the board matrix.
    private const int boardXLength = 11;
    private const int boardYLength = 15;

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
                if (j == 5)
                {
                    if (i == 4 || i == 2)
                    {
                        tiles[i,j].piece = new WhitePiece();
                    }
                    else if (i == 10 || i == 12)
                    {
                        tiles[i,j].piece = new BlackPiece();
                    }
                    else if (i == 7)
                    {
                        tiles[i,j].piece = new Ball();
                    }
                }
            }
        }
    }
    
    // A getter method for a tile at a given pair of coordinates.
    public Tile GetTile(int x, int y)
    {
        return this.tiles[y,x];
    }

    // Clear all of the highlighted tiles.
    public void ClearAllTiles()
    {
        for (int i = 0; i < boardYLength; i++)
        {
            for (int j = 0; j < boardXLength; j++)
            {
                GetTile(j,i).SetHighlight(false);
            }
        }
    }
}
