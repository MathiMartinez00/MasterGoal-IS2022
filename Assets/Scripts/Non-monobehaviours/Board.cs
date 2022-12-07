//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public class Board
{
    Tile[][] tiles;

    // Board's constructor method. Creates a new board.
    public Board()
    {
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                // Initialize a new tile and store it.
                tiles[j,i] = new Tile(j, i, null);

                // The player pieces and ball go on the fifth column (x == 5).
                if (j == 5)
                {
                    if (i == 4 || i == 2)
                    {
                        tiles[j,i].piece = new WhitePiece();
                    }
                    else if (i == 10 || i == 12)
                    {
                        tiles[j,i].piece = new BlackPiece();
                    }
                    else if (i == 7)
                    {
                        tiles[j,i].piece = new Ball();
                    }
                }

                
            }
        }
    }
    
}
