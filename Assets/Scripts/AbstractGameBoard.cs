using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract representation of the state of the game (it uses a matrix).
public class AbstractGameBoard : MonoBehaviour
{
    public char [,] board;
    [SerializeField] (int,int) redChip1;
    [SerializeField] (int,int) redChip2;
    [SerializeField] (int,int) whiteChip1;
    [SerializeField] (int,int) whiteChip2;
    [SerializeField] (int,int) ballPosition;
    [SerializeField] Team turn;

    public void Start()
    {
        // Allocate memory for the abstract game board.
        board = new char [13,11];
    }

    // Class constructor.
    public void Initialize(
        GameObject[] playerChips, GameObject ballChip,
        Team currentTurn)
    {
        redChip1 = VectorToArrayCoordinates(playerChips[0].transform.position);
        redChip2 = VectorToArrayCoordinates(playerChips[1].transform.position);
        whiteChip1 = VectorToArrayCoordinates(playerChips[2].transform.position);
        whiteChip2 = VectorToArrayCoordinates(playerChips[3].transform.position);
        ballPosition = VectorToArrayCoordinates(ballChip.transform.position);
        turn = currentTurn;

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                if (redChip1 == (j,i) || redChip2 == (j,i))
                {
                    board[i,j] = 'r';
                }
                else if (whiteChip1 == (j,i) || whiteChip2 == (j,i))
                {
                    board[i,j] = 'w';
                }
                else if (ballPosition == (j,i))
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
    // and y coordinates plus an offset, so that the board has no
    // negative indices.
    public (int, int) VectorToArrayCoordinates(Vector3 position)
    {
        return ((int)position.x + 5, (int)position.y + 6);
    }
}
