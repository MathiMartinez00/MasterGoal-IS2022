//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

// Abstract representation of the state of the game.
// This is a standalone C# class and doesn't inherit from Monobehavior, so it
// will not be attached to any Unity GameObject.
public class Game
{
    public enum GameMode
    {
        // User against user, no AI.
        2Players,
        // User agains the machine, with AI.
        1Player
    }

    public enum GameStatus
    {
        WaitingPlayerPieceSelection,
        WaitingPlayerPieceMovement,
        WaitingBallMovement
    }

    private Player player1;
    private Player player2;
    private Board board;
    private Player currentTurn;
    private List<Move> movesPlayed;
    private Nullable<Player> currentBallPossession;

    // C# class constructor.
    Game(GameMode gameMode, Player firstTurn)
    {
        board = new Board();

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