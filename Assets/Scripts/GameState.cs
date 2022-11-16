using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract representation of the state of the game (it uses a matrix).
public class GameState : ScriptController
{
    char [,] board;
    (int,int)[] redChips;
    (int,int)[] whiteChips;
    (int,int) ballPosition;
    Team currentTurn;

    // Class constructor.
    public GameState(
        GameObject[] playerChips, GameObject ballChip,
        Team currentTurn
        )
    {
        this.redChips[0] = VectorToArrayCoordinates(playerChips[0]);
        this.redChips[1] = VectorToArrayCoordinates(playerChips[1]);
        this.whiteChips[0] = VectorToArrayCoordinates(playerChips[2]);
        this.whiteChips[1] = VectorToArrayCoordinates(playerChips[3]);
        this.ballChipCoordinates = VectorToArrayCoordintes(ballChip);
        this.currentTurn = currentTurn;

        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (redChips.Contains((i,j)))
                {
                    board[i,j] = 'r';
                }
                else if (whiteChips.Contains((i,j)))
                {
                    board[i,j] = 'w';
                }
                else if (ballChipCoordinates == (i,j)))
                {
                    board[i,j] = 'b';
                }
                else
                {
                    board[i,j] = '0';
                }
            }
        }
    }

    // Takes a Vector3 object and returns a tuple with its x
    // and y coordinates.
    public (int, int) VectorToArrayCoordinates(Vector3 position)
    {
        return (position.x, position.y);
    }
}
