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
    //private Nullable<Player> currentBallPossession;
    private Nullable<Piece> selectedPiece;
    private int passCount;
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
        this.passCount = 0;
        this.whiteScore = 0;
        this.blackScore = 0;
    }

    // This method should be called when the user taps on the screen.
    // The x and y coordinates of the tile should be passed as
    // arguments.
    public void UserInput(int x, int y)
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
            // Check if the ball can be played, either to make the first
            // pass or to kick the ball away.
            if (CanBallBePlayed())
            {
                // Change the status of the game.
                this.gameStatus = WaitingBallMovement;
                // Highlight the valid destinations for the ball.
                CalculateBallMovesAndHighlightTiles();
            }
            else
            {
                // Change the status of the game.
                this.gameStatus = WaitingPlayerPieceSelection;
                // Switch the current turn.
                SwitchCurrentTurn();
            }
        }
        else if (
            gameStatus == WaitingPlayerBallMovement &&
            selectedTile.IsTileHighlighted())
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
    // 5) The piece cannot stay on its original tile.
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
    // straight up and down, and diagonally. A piece cannot stay on its
    // tile and count that action as a "move".
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

    // Calculates all the possible moves for the ball from its current
    // position on the board and highlights the tiles on which the ball
    // can be moved to.
    private void CalculateBallMovesAndHighlightTiles()
    {
        Ball ball = this.board.GetBall();

        // Clamp the coordinates, so it doesn't go out of the board.
        int startX = Math.Max(ball.GetX() - this.ballPieceReach, 0);
        int startY = Math.Max(ball.GetY() - this.ballPieceReach, 0);
        int endX = Math.Min(
            ball.GetX() + this.ballPieceReach, this.board.GetXLength() - 1);
        int endY = Math.Min(
            ball.GetY() + this.ballPieceReach, this.board.GetYLength() - 1);

        // The ball can move no more than 4 squares from its
        // current position.
        // Iterate through the adjacent rows.
        for (int i = startY; i <= endY; i++)
        {
            // Iterate through the adjacent columns.
            for (int j = startX; j <= endX; j++)
            {
                // Check for the other conditions: no placing the ball
                // in the team's own area, restrict the movement to
                // up-down, right-left and diagonal.
                if (CheckForValidBallMove(ball.GetX(), ball.GetY(), j, i))
                {
                    // Highlight the tile.
                    board.GetTile(j, i).SetHighlight(true);
                }
            }
        }
    }

    // Check for the three conditions:
    // 1) The player can't move the ball to its own area;
    // 2) Allow movement to just be over a single row, a single column or
    // diagonally.
    // 3) The tile is valid (is not next to either one of the goals).
    // 4) The piece cannot stay on its original tile.
    // 5) The ball cannot be in the 
    //
    // If all of those conditions are met, returns "true"; false otherwise.
    private bool CheckForValidBallMove(int x1, int y1, int x2, int y2)
    {
        return (
            !IsItsOwnArea(x2, y2) &&
            !IsItsOwnCorner(x2, y2) &&
            CheckForValidMovementDirections(x2-x1, y2-y1) &&
            this.board.GetTile(x2, y2).IsTileValid()
            )
    }

    // Checks to see if the given coordinates corresponds to the area
    // of the team whose turn it is (determined by the currentTurn field).
    //
    // The area of each team is defined as a 9 by 4 rectangle contiguous
    // to the team's goal.
    private bool IsItsOwnArea(int x, int y)
    {
        return (
            // These are the columns that span the area.
            (x >= 1 && x <= 9) &&
             // The rows that span the area of each team.
            ((this.currentTurn == White && y >= 1  && y <= 4 ) ||
             (this.currentTurn == Black && y >= 10 && y <= 13))
            )
    }

    // Checks if a goal has been scored based on the current position
    // of the ball.
    //
    // Returns the scoring team if a goal has been scored; null otherwise.
    private Nullable<Team> CheckForGoalScored()
    {
        Ball ball = this.board.GetBall();

        // Look for the ball on the white team's goal.
        if (ball.GetY() == 0)
        {
            return White;
        }
        // Look for the ball on the black team's goal.
        else if (ball.GetY() == 14)
        {
            return Black;
        }

        // If no goal has been scored, return null.
        return null;
    }

    // Checks if a team is currently in possession of the ball and if
    // that team is the team whose turn it is, currently.
    //
    // If those conditions are met, the player can move the ball, either
    // to make it a pass or not.
    private bool CanBallBePlayed()
    {
        // Check if the ball can be passed or if we must switch turns.
        Nullable<Team> ballPossesion = GetBallPossession();
        return (
            ballPossesion.HasValue &&
            ballPossesion.Value == this.currentTurn)
    }

    // Takes the ball piece and returns the team that is currently in
    // possession of the ball. Returns null if no team is in possession
    // of the ball.
    private Nullable<Team> GetBallPossession(Ball ballPiece)
    {
        // Set the limits for the nested iteration.
        int minX = Math.Max(0, ballPiece.GetX() - 1);
        int maxX = Math.Min(
            this.board.GetXLength() - 1, ballPiece.GetX() + 1);
        int minY = Math.Max(0, ballPiece.GetY() - 1);
        int maxY = Math.Min(
            this.board.GetYLength() - 1, ballPiece.GetY() + 1);
        
        int whiteCount = 0;
        int blackCount = 0;

        // Check the tiles that are contiguous to the ball for player pieces.
        for (int i = minY; i <= maxX; i++)
        {
            for (int j = minX; j <= maxY; j++)
            {
                // Get the piece, if there is any.
                Nullable<Piece> piece = this.board.GetTile(j,i).GetPiece();

                // Count the white pieces.
                if (
                    (j != ballPiece.GetX() && i != ballPiece.GetY()) &&
                    piece.HasValue && piece.Value.teamColor == White)
                {
                    whiteCount++;
                }

                // Count the black pieces.
                else if (
                    (j != ballPiece.GetX() && i != ballPiece.GetY()) &&
                    piece.HasValue && piece.Value.teamColor == Black)
                {
                    blackCount++;
                }
            }
        }

        // Return the team color that is in possession of the ball.
        if (whiteCount > blackCount)
        {
            return White;
        }
        else if (blackCount > whiteCount)
        {
            return Black;
        }
        else
        {
            return null;
        }
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
