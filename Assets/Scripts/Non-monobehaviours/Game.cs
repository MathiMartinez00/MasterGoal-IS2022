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
        // User against machine.
        1Player,
        // Machine against machine.
        0Players
    }

    public enum GameStatus
    {
        WaitingPlayerPieceSelection,
        WaitingPlayerPieceMovement,
        WaitingBallMovement
    }

    public enum Team
    {
        White,
        Black
    }

    private Board board;
    private GameMode gameMode;
    private GameStatus gameStatus;
    private Team currentTurn;
    private List<Move> allMoves;
    private Nullable<Player> currentBallPossession;
    private Nullable<Piece> selectedPiece;
    private int whiteScore;
    private int blackScore;

    // C# class constructor.
    Game(GameMode gameMode, Player firstTurn)
    {
        this.board = new Board();
        this.gameMode = gameMode;
        this.gameStatus = WaitingPlayerPieceSelection;
        this.currentTurn = White;
        this.allMoves = new List<Move>;
        this.currentBallPossession = null;
        this.whiteScore = 0;
        this.blackScore = 0;
    }

    // This method should be called when the user taps on the screen.
    // The x and y coordinates of the tile should be passed as
    // arguments.
    public void userInput(int x, int y)
    {
        // Get the selected tile.
        Nullable<Piece> selectedTile = this.board.GetTile();

        // Get the piece, if there is any.
        Nullable<Piece> piece = selectedTile.GetPiece();

        // If there are no player pieces selected yet, go to this branch.
        if (
            gameStatus == WaitingPlayerPieceSelection &&
            piece.HasValue &&
            IsPlayerPieceInTile(piece.Value))
        {
            PlayerPiece selectedPiece = piece.Value;
            if (selectedPiece.teamColor == currentTurn)
            {
                this.gameStatus = WaitingPlayerPieceMovement;
                this.Board.CalculateMovesAndHighlightTiles(selectedTile, selectedPiece)
            }
        }
        else if (gameStatus == WaitingPlayerPieceMovement)
        {
        }
        else if (gameStatus == WaitingPlayerBallMovement)
        {
        }
    }

    // Calculates the valid moves for a player piece and highlights all
    // of the tiles in which this piece can be moved.
    private void CalculateMovesAndHighlightTiles(
        Tile selectedTile, PlayerPiece selectedPiece)
    {
        // Get the tile's coordinates.
        int tileX = selectedTile.GetX();
        int tileY = selectedTile.GetY();

        // A player con move no more than two squares from its position.
        // Iterate through the adjacent rows.
        for (int i = -2; i < 3; i++)
        {
            // Iterate through the adjacent columns.
            for (int j = -2; j < 3; j++)
            {
                if (CheckForValidMovementCoordinates(i, j, 2))
                // Highlight the tile.
                board.GetTile(tileX + i, tileY + j).SetHighlight(true);
            }
        }
    }

    // Checks if the movement pattern is valid. A piece can only be moved
    // straight up and down and diagonally. A piece cannot move to its
    // original tile.
    //
    // Returns true if the movement pattern is valid, false otherwise.
    private bool CheckForValidMovementCoordinates(int x, int y)
    {
        return (
            // Prevent the user from not making an effective move.
            (!(x == 0 && y == 0)) &&
            // If the piece is moved more than a tile away from its original
            // place, then allow only the diagonals and straight up and down.
            (!(Math.Abs(x) > 1 && Math.Abs(y) > 1 && Math.Abs(x) != Math.Abs(y))) &&
            (!(Math.Abs(x) == 1 && Math.Abs(y) > 1))
            (!(Math.Abs(y) == 1 && Math.Abs(x) > 1))
            )
    }

    private void CalculateMoves(PlayerPiece piece)
    {

    }

    private void CalculateMoves(Ball piece)
    {}

    private bool IsPlayerPieceInTile(PlayerPiece piece)
    {
        return true;
    }

    private bool IsPlayerPieceInTile(Piece piece)
    {
        return false;
    }

    public bool playerMove(
        Turn moveColor,
        int originX, int originY,
        int destinationX, int destinationY)
    {
        Tile originTile = board.GetTile(originX, originY);
        Tile destinationTile = board.GetTile(destinationX, destionationY);
        Move move = new Move(player, originTile, destinationTile);
        return this.makeMove(move, moveColor)
    }

    private bool makeMove(Move move, Turn moveColor)
    {
        Nullable<Piece> pieceToMoveNullable = move.GetOriginTile().GetPiece();

        // Check if the nullable piece field is null.
        if (pieceToMoveNullable == null)
        {
            return false;
        }

        // If the nullable piece field is not null, get the piece.
        Piece pieceToMove = pieceToMoveNullable.Value;

        if (currentTurn != moveColor)
        {
            return false;
        }
        else if (!pieceToMove.canMove(
            board, move.GetOriginTile, move.GetDestinationTile)
            )
        {
            return false;
        }

        // Store the move.
        allMoves.add(move);

        // Move the piece.
        move.GetDestinationTile().SetPiece(pieceToMove);
        move.GetOriginTile().SetPiece(null);

        // Switch the current turn.
        SwitchCurrentTurn();
    }

    public bool SelectPieceToMove(
        int originX, int originY, int destinationX, int destinationY)
    {
        // Selected tile.
        Tile tile = board.GetTile(x,y);

        Nullable<Piece> selectedPieceNullable = tile.GetPiece();

        // Check if the nullable piece field is null.
        if (selectedPieceNullable == null)
        {
            return false;
        }

        // If the nullable piece field is not null, get the piece.
        Piece selectedPiece = selectedPieceNullable.Value;

        if (currentTurn != moveColor)
        {
            return false;
        }
        else if (!pieceToMove.canMove(
            board, move.GetOriginTile, move.GetDestinationTile)
            )
        {
            return false;
        }
    }

    // Switches the current turn from "White" to "Black", or viceversa.
    private void SwitchCurrentTurn()
    {
        if (this.currentTurn == White)
        {
            this.currentTurn = Black;
        }
        else
        {
            this.currentTurn = White;
        }
    }
}
