using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract representation of the state of the game (it uses a matrix).
// This is a standalone C# class and doesn't inherit from Monobehavior, so it
// will not be attached to any Unity GameObject.
public class GameState
{
    // Enum datatype for keeping track of the current turn.
    public enum Team
    {
        White,
        Red,
    }

    public enum GameMode
    {
        // User against user, no AI.
        2Players,
        // User agains the machine, with AI.
        1Player
    }

    plublic enum GameState
    {
        WaitingPlayerChipSelection,
        WaitingPlayerChipMovement,
        WaitingBallMovement
    }

    public char [13,11] board;
    public (int,int) redChip1 = ();
    public (int,int) redChip2 = ();
    public (int,int) whiteChip1 = ();
    public (int,int) whiteChip2 = ();
    public (int,int) ballPosition = ();
    // Property for keeping track of the current turn.
    public Team turn;
    // Property for keeping track of each player's chips color.
    public Team player1;
    // Player 2 may be the machine or the user, depending on the game mode.
    public Team player2;

    // C# class constructor.
    GameState(GameMode gameMode, Team turn, Team player1)
    {
        // Allocate memory for the abstract game board.
        board = new char [13,11];

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                if (redChip1 == (j,i) || redChip2 == (j,i))
                {
                    board[i,j] = 'R';
                }
                else if (whiteChip1 == (j,i) || whiteChip2 == (j,i))
                {
                    board[i,j] = 'W';
                }
                else if (ballPosition == (j,i))
                {
                    board[i,j] = 'B';
                }
                else
                {
                    board[i,j] = '0';
                }
            }
        }
    }


    // Checks the playerChip can be moved to the field at destination.
    public bool IsFieldValidForPlayerChip((int,int) playerChip, (int,int) destination)
    {
        // Check if the destination coincides with the position of other chips.
        if (
            redChip1 == destination ||
            redChip2 == destination ||
            whiteChip1 == destination ||
            whiteChip2 == destination ||
            ballChip == destination
            )
        {
            return false;
        }
        // Check if the destination is out of bounds.
        else if (
            // Check on the sides of the board.
            destination.Item1 < 0 || destination.Item1 > 10 ||
            // Check on the upper and lower parts of the board.
            destination.Item2 < 0 || destination.Item2 > 14 ||
            // Check on the upper and lower parts of the board
            // around the goals.
            ((destination.Item2 == 0 || destination.Item2 == 14) &&
            (destination.Item2 < 3 || destination.Item2 > 7))
            )
        {
            return false;
        }
    }
    
    // Takes the position of a player's chip and returns the possible
    // positions that the chip can move to.
    public List<(int,int)> CalculateMovesPlayer((int,int) playerChip)
    {
        List<(int,int)> possibleMoves = new List<(int,int)>;
        // A player can move no more than 2 tiles away from its
        // current position.
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                // If the tile is valid, add it to the list of moves.
                if (IsFieldValidForPlayerChip(playerChip, (i,j)))
                {
                    possibleMoves.Add((i,j));
                }
            }
        }
        return possibleMoves;
    }

    // Checks if a goal has been scored.
    public bool PlayerScoredGoal()
    {
        return (
            ballChip.Item2 >= 3 && ballChip.Item2 <= 7 &&
            (ballChip.Item1 == 0 || ballChip.Item1 == 14)
            )
    }

    public bool IsBallPassable()
    {}

    public bool AreAdjacentFieldsValid()
    {}

    // Takes a Vector3 object and returns a tuple with its x
    // and y coordinates plus an offset, so that the board has no
    // negative indices.
    public (int,int) VectorToArrayCoordinates(Vector3 position)
    {
        return ((int)position.x + 5, (int)position.y + 6);
    }
}
