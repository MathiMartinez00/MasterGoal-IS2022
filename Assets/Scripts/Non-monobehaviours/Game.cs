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

    // The number of tiles that each type of piece can move per action.
    private const int playerPieceReach = 2;
    private const int ballPieceReach   = 4;

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
            IsPlayerPiece(piece.Value))
        {
            PlayerPiece selectedPiece = piece.Value;
            // Check if the current turn matches with the piece's color.
            if (selectedPiece.teamColor == currentTurn)
            {
                // Highlight the tiles that indicate valid destinations.
                CalculatePlayerMovesAndHighlightTiles(
                    selectedTile, selectedPiece)
                // Change the status of the game.
                this.gameStatus = WaitingPlayerPieceMovement;
                // Store the selected piece.
                this.selectedPiece = piece.Value;
            }
        }
        else if (
            gameStatus == WaitingPlayerPieceMovement &&
            selectedTile.IsTileHighlighted())
        {
            // Move the piece.
            selectedTile.SetPiece(this.selectedPiece);
            // Clear all of the highlighted tiles.
            this.board.ClearAllTiles();
            // Deselect the piece.
            this.selectedPiece = null;
            // Change the status of the game.
            this.gameStatus = WaitingPlayerBallMovement;
            // Highlight the valid destinations for the ball.
            CalculateBallMovesAndHighlightTiles();
        }
        else if (gameStatus == WaitingPlayerBallMovement)
        {
        }
    }

    // Calculates the valid moves for a player piece and highlights all
    // of the tiles in which this piece can be moved.
    private void CalculatePlayerMovesAndHighlightTiles(
        Tile selectedTile, PlayerPiece selectedPiece)
    {
        // Get the selected tile's coordinates.
        int tileX = selectedTile.GetX();
        int tileY = selectedTile.GetY();

        // Clamp the coordinates, so it doesn't go out of the board.
        int startX = Math.Max(tileX - this.playerPieceReach, 0);
        int startY = Math.Max(tileY - this.playerPieceReach, 0);
        int endX = Math.Min(tileX + this.playerPieceReach, 10);
        int endY = Math.Min(tileY + this.playerPieceReach, 14);

        // A player con move no more than two squares from its position.
        // Iterate through the adjacent rows.
        for (int i = startY; i <= endY; i++)
        {
            // Iterate through the adjacent columns.
            for (int j = startX; j <= endX; j++)
            {
                // Check for the other conditions: no placing pieces on their
                // own corners, no jumping over the ball and other players and
                // restrict the movement to up-down, right-left and diagonal.
                if (CheckForValidPlayerMove(
                    tileX, tileY, x2, y2, selectedPiece.GetTeamColor()))
                {
                    // Highlight the tile.
                    board.GetTile(x2, y2).SetHighlight(true);
                }
            }
        }
    }

    // Check for the three conditions:
    // 1) No placing pieces on their own corners;
    // 2) No jumping over the ball and other players;
    // 3) Allow movement to just be over a single row, a single column or
    // diagonally.
    // 4) The tile is valid (is not next to either one of the goals).
    //
    // If all of those conditions are met, returns "true"; false otherwise.
    private bool CheckForValidPlayerMove(
        int x1, int y1, int x2, int y2, Team teamColor)
    {
        return (
            !IsAnotherPieceInTheWay(x1, y1, x2, y2) &&
            !IsItsOwnCorner(x2, y2) &&
            CheckForValidMovementDirections(x2-x1, y2-y1) &&
            this.board.GetTile(x2, y2).IsTileValid()
        )
    }

    // Takes the coordinates of two tiles, origin and destination, and
    // traverses the tiles between them to check if another piece is
    // on the way (because a player can't jump over another player).
    //
    // Returns "true" if a piece is on the way and false otherwise.
    private bool IsAnotherPieceInTheWay(int x1, int y1, int x2, int y2)
    {
        int minX = Math.Min(x1,x2);
        int minY = Math.Min(y1,y2);
        int maxX = Math.Max(x1,x2);
        int maxY = Math.Max(y1,y2);

        if (x1 == x2)
        {
            // Traverse the board over a single column.
            for (int i = minY; i <= maxY; i++)
            {
                // Check if another piece is on the path from the origin
                // tile to the destination tile.
                if (this.board.GetTile(i, x1).GetPiece().HasValue)
                {
                    return true;
                }
            }
        }
        else if (y1 == y2)
        {
            // Traverse the board over a single row.
            for (int j = minX; j <= maxX; j++)
            {
                if (this.board.GetTile(y1, j).GetPiece().HasValue)
                {
                    return true;
                }
            }
        }
        else
        {
            // Traverse the board diagonally.
            for (int k = minX; k <= maxX; k++)
            {
                if (this.board.GetTile(k, k).GetPiece().HasValue)
                {
                    return true;
                }
            }
        }
        
        // If no pieces were found on the traversal, return false.
        return false;
    }

    // Checks if the given coordinates are the coordinates of one of the
    // given team's corners (players can't move to their own corners).
    private bool IsItsOwnCorner(int x, int y, Team teamColor)
    {
        return (
            (y == 0 || y == 10) &&
            // The white team play on the top side of the board.
            ((teamColor == White && x == 1 ) ||
            // The black team play on the bottom side of the board.
             (teamColor == Black && x == 13)
             )
            )
    }

    // Checks if the movement pattern is valid. A piece can only be moved
    // straight up and down and diagonally. A piece cannot move to its
    // original tile.
    //
    // Returns true if the movement pattern is valid, false otherwise.
    private bool CheckForValidMovementDirections(int x, int y)
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

    private void CalculateBallMovesAndHighlightTiles(Ball ballPiece)
    {

    }

    // Pattern matching method to check if a piece is a PlayerPiece.
    private bool IsPlayerPiece(PlayerPiece piece)
    {
        return true;
    }

    // Pattern matching method to check if a piece is a PlayerPiece.
    private bool IsPlayerPiece(Piece piece)
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
