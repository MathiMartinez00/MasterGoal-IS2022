using System.Collections.Generic;
using System;

#nullable enable

// An instance of this class represents an abstract representation of the
// state of the game.
// This is a standalone C# class and doesn't inherit from Monobehavior,
// so it will not be attached to any Unity GameObject.
public class Game
{
    public Board Board { get; private set; }
    // GAME MODE NOT USED YET.
    public GameMode GameMode { get; private set; }
    public GameStatus GameStatus { get; private set; }
    private Team currentTurn;
    private List<Move> allMoves;
    private PlayerPiece selectedPiece;
    private int passCount;
    private int whiteScore;
    private int blackScore;

    // The number of tiles that each type of piece can move per action.
    private readonly int playerPieceReach = 2;
    private readonly int ballPieceReach   = 4;

    // C# class constructor.
    public Game(GameMode gameMode, Team firstTurn)
    {
        Board = new Board();
        GameMode = gameMode;
        GameStatus = GameStatus.WaitingPlayerPieceSelection;
        this.currentTurn = Team.White;
        // Select a piece just as a placeholder for the field.
        this.selectedPiece = Board.WhitePiece1;
        this.allMoves = new List<Move>();
        this.passCount = 0;
        this.whiteScore = 0;
        this.blackScore = 0;
    }

    // This method should be called when the user taps on the screen.
    // A Position instnace with the coordinates of the selected tile
    // should be passed as the argument.
    //
    // Returns the move that was performed on every particular method
    // call. This is useful to do a reactive update on the board. If no
    // moves were made (just a selection of a piece, for example), it
    // returns null.
    public Move? UserInput(Position position)
    {
        // Conver the coordinates to x and y.
        int x = position.X;
        int y = position.Y;

        // Get the selected tile.
        AbstractTile selectedTile = Board.GetTile(x,y);

        // Get the player piece, if there is any.
        PlayerPiece? selectedPlayerPiece = selectedTile.PlayerPiece;

        // If there are no player pieces selected yet, go to this branch.
        if (
            GameStatus == GameStatus.WaitingPlayerPieceSelection &&
            selectedPlayerPiece != null)
        {
            // Check if the piece is a PlayerPiece, check if it is a piece
            // of the right team, highlight the valid moves, change the
            // game status and store the selected piece.
            SelectPlayerPiece(selectedPlayerPiece, selectedTile);

            // Because no moves were made on this conditional branch.
            return null;
        }
        else if (
            GameStatus == GameStatus.WaitingPlayerPieceMovement &&
            selectedTile.IsHighlighted)
        {
            // Get the origin tile.
            AbstractTile originTile = Board.GetTile(
                this.selectedPiece.X,this.selectedPiece.Y);

            // Update the piece's position fields.
            this.selectedPiece.X = selectedTile.X;
            this.selectedPiece.Y = selectedTile.Y;

            // Update the origin and destination tile's player piece fields.
            Move move = UpdateTilesPieceFieldsAndStoreMove(
                this.selectedPiece, originTile, selectedTile);

            // Clear all of the highlighted tiles.
            Board.ClearAllHighlights();

            // Check if the team that made the last player move is in
            // possession of the ball.
            if (IsBallInPossessionOfCurrentTurn())
            {
                // Change the status of the game.
                GameStatus = GameStatus.WaitingBallMovement;
                // Highlight the valid destinations for the ball
                // for the next method call.
                CalculateBallMovesAndHighlightTiles();
            }
            else
            {
                // Change the status of the game.
                GameStatus = GameStatus.WaitingPlayerPieceSelection;
                // Switch the current turn.
                SwitchCurrentTurn();
            }

            // Return the move (to do a reactive board update).
            return move;
        }
        else if (
            GameStatus == GameStatus.WaitingBallMovement &&
            selectedTile.IsHighlighted)
        {
            // Get the origin tile.
            AbstractTile originTile = Board.GetTile(Board.Ball.X,Board.Ball.Y);

            // Update the piece's position fields.
            Board.Ball.X = selectedTile.X;
            Board.Ball.Y = selectedTile.Y;

            // Update both the origin and destination tile's ball fields.
            Move move = UpdateTilesPieceFieldsAndStoreMove(
                Board.Ball, originTile, selectedTile);

            // Clear all of the highlighted tiles.
            Board.ClearAllHighlights();

            // Check if a goal has been scored.
            Team? goalScored = CheckForGoalScored();
            if (goalScored != null)
            {
                // Mark that the latest move resulted in a goal.
                move.IsGoal = true;

                // Update the scores.
                if (goalScored == Team.White)
                {
                    whiteScore++;
                }
                else
                {
                    blackScore++;
                }
                // The game is over when someone scores two goals.
                if (this.whiteScore >= 2 || this.blackScore >= 2)
                {
                    GameStatus = GameStatus.GameOver;
                }
                else
                {
                    // After a goal, it's the opposite team's turn.
                    this.currentTurn = GetOppositeTeam(goalScored.Value);
                    // Change the status of the game.
                    GameStatus = GameStatus.WaitingPlayerPieceSelection;
                    // Reset the pieces to their initial position.
                    Board.ResetPieces();
                }
            }
            else
            {
                // Check if the player passed the ball.
                if (IsBallInPossessionOfCurrentTurn())
                {
                    this.passCount++;
                    // Highlight the valid destinations for the ball
                    // for the next method call.
                    CalculateBallMovesAndHighlightTiles();
                }
                else
                {
                    // Reset the pass count.
                    this.passCount = 0;
                    // Change the status of the game.
                    GameStatus = GameStatus.WaitingPlayerPieceSelection;
                    // Switch the current turn.
                    SwitchCurrentTurn();
                }
            }

            // Return the move (to do a reactive board update).
            return move;
        }

        // If an invalid selection is made (no branches apply), return null.
        return null;
    }

    // Check if the player piece is of the right team, highlight the
    // valid moves, change the game status and store the selected piece.
    private void SelectPlayerPiece(PlayerPiece piece, AbstractTile tile)
    {
        // Check if the current turn matches with the piece's color.
        if (piece.TeamColor == currentTurn)
        {
            // Highlight and store the tiles that indicate valid destinations.
            CalculatePlayerMovesAndHighlightTiles(tile, piece);
            // Change the status of the game.
            GameStatus = GameStatus.WaitingPlayerPieceMovement;
            // Store the selected piece in an instance field.
            this.selectedPiece = piece;
        }
    }

    // Overloaded method that updates the origin and destination tile's
    // PlayerPiece and Ball fields according to the type of Piece that
    // is moved, creates a new Move instance, stores the move and then
    // returns it. This method is used then the piece is an instance of
    // Player Piece.
    private Move UpdateTilesPieceFieldsAndStoreMove(
        PlayerPiece playerPiece, AbstractTile originTile,
        AbstractTile destinationTile)
    {
        // Set the origin tile's "PlayerPiece" field to null.
        originTile.PlayerPiece = null;
        // Set the destination tile's field to the correct reference.
        destinationTile.PlayerPiece = playerPiece;

        // Store the move.
        Move move = new Move(
            this.currentTurn, originTile, destinationTile, playerPiece);
        this.allMoves.Add(move);

        // Return the move (for the reactive board update).
        return move;
    }

    // Overloaded method that updates the origin and destination tile's
    // PlayerPiece and Ball fields according to the type of Piece that
    // is moved, creates a new Move instance, stores the move and then
    // returns it. This method is used then the piece is an instance of
    // Ball.
    private Move UpdateTilesPieceFieldsAndStoreMove(
        Ball ballPiece, AbstractTile originTile,
        AbstractTile destinationTile)
    {
        // Set the origin tile's "Ball" field to null.
        originTile.Ball = null;
        // Set the destination tile's field to the correct reference.
        destinationTile.Ball = ballPiece;

        // Store the move.
        Move move = new Move(
            this.currentTurn, originTile, destinationTile, ballPiece);
        this.allMoves.Add(move);

        // Return the move (for the reactive board update).
        return move;
    }

    // Calculates the valid moves for a player piece and highlights all
    // of the tiles in which this piece can be moved.
    private void CalculatePlayerMovesAndHighlightTiles(
        AbstractTile selectedTile, PlayerPiece selectedPiece)
    {
        // Get the selected tile's coordinates.
        int tileX = selectedTile.X;
        int tileY = selectedTile.Y;

        // Clamp the coordinates, so it doesn't go out of the board.
        int startX = Math.Max(tileX - playerPieceReach, 0);
        int startY = Math.Max(tileY - playerPieceReach, 0);
        int endX = Math.Min(tileX + playerPieceReach, 10);
        int endY = Math.Min(tileY + playerPieceReach, 14);

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
                    tileX, tileY, j, i, selectedPiece.TeamColor))
                {
                    // Highlight the tile.
                    Board.GetTile(j, i).IsHighlighted = true;
                }
            }
        }
    }

    // Check for these conditions:
    // 1) No placing pieces on their own corners;
    // 2) No jumping over the ball and other players;
    // 3) The destination tile is free (only one piece per tile allowed).
    // 4) Allow movement to just be over a single row, a single column or
    // diagonally.
    // 5) The tile is valid (is not next to either one of the goals).
    // 6) The piece cannot stay on its original tile.
    //
    // If all of those conditions are met, returns "true"; false otherwise.
    private bool CheckForValidPlayerMove(
        int x1, int y1, int x2, int y2, Team teamColor)
    {
        // Check all of the aforementioned conditions.
        bool notOwnCorner = !IsItsOwnCorner(x2, y2, teamColor); // 1
        bool notPieceInTheWay = !IsAnotherPieceInTheWay(
            x1, y1, x2, y2); // 2
        bool destinationTileFree = DoesTileContainAPiece(x2,y2); // 3
        bool validDirection = CheckForValidMovementDirections(
            x2-x1, y2-y1); // 4 & 6
        bool validTile = Board.GetTile(x2, y2).IsValid; // 5

        return (
            notOwnCorner && notPieceInTheWay && validDirection && validTile);

        /*
        return (
            !IsItsOwnCorner(x2, y2, teamColor) && // 1
            !IsAnotherPieceInTheWay(x1, y1, x2, y2) && // 2
            CheckForValidMovementDirections(x2-x1, y2-y1) && // 3 & 5
            Board.GetTile(x2, y2).IsValid // 4
        );
        */
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
            for (int i = minY+1; i < maxY; i++)
            {
                // Check if another piece is on the path from the origin
                // tile to the destination tile.
                if (DoesTileContainAPiece(x1,i))
                {
                    return true;
                }
            }
        }
        else if (y1 == y2)
        {
            // Traverse the board over a single row.
            for (int j = minX+1; j < maxX; j++)
            {
                if (DoesTileContainAPiece(j, y1))
                {
                    return true;
                }
            }
        }
        else
        {
            // Traverse the board diagonally.
            for (int k = minX+1; k < maxX; k++)
            {
                if (DoesTileContainAPiece(k,k))
                {
                    return true;
                }
            }
        }
        
        // If no pieces were found on the traversal, return false.
        return false;
    }

    // Takes the x and y coordinates of a tile and returns true if the
    // the tile contains a player piece or the ball, false otherwise.
    private bool DoesTileContainAPiece(int x, int y)
    {
        AbstractTile tile = Board.GetTile(x,y);
        return (tile.PlayerPiece != null || tile.Ball != null);
    }

    // Checks if the given coordinates are the coordinates of one of the
    // given team's corners (players can't move to their own corners).
    private bool IsItsOwnCorner(int x, int y, Team teamColor)
    {
        return (
            (x == 0 || x == 10) &&
            // The white team play on the top side of the board.
            ((teamColor == Team.White && y == 1 ) ||
            // The black team play on the bottom side of the board.
             (teamColor == Team.Black && y == 13)
            ));
    }

    // Checks to see if the given coordinates corresponds to the area
    // of the team whose turn it is (determined by the currentTurn field).
    //
    // The area of each team is defined as a 9 by 4 rectangle contiguous
    // to the team's goal.
    private bool IsItsOwnArea(int x, int y, Team teamColor)
    {
        return (
            // These are the columns that span the area.
            (x >= 1 && x <= 9) &&
             // The rows that span the area of each team.
            ((teamColor == Team.White && y >= 1  && y <= 4 ) ||
             (teamColor == Team.Black && y >= 10 && y <= 13))
            );
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
            (!(Math.Abs(x) == 1 && Math.Abs(y) > 1)) &&
            (!(Math.Abs(y) == 1 && Math.Abs(x) > 1))
            );
    }

    // Calculates all the possible moves for the ball from its current
    // position on the board and highlights the tiles on which the ball
    // can be moved to.
    private void CalculateBallMovesAndHighlightTiles()
    {
        Ball ball = Board.Ball;

        // Clamp the coordinates, so it doesn't go out of the board.
        int startX = Math.Max(ball.X - this.ballPieceReach, 0);
        int startY = Math.Max(ball.Y - this.ballPieceReach, 0);
        int endX = Math.Min(
            ball.X + this.ballPieceReach, Board.GetXLength() - 1);
        int endY = Math.Min(
            ball.Y + this.ballPieceReach, Board.GetYLength() - 1);

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
                if (CheckForValidBallMove(ball.X, ball.Y, j, i))
                {
                    // Highlight the tile.
                    Board.GetTile(j, i).IsHighlighted = true;
                }
            }
        }
    }

    // Check that a potential ball move satisfies these conditions.
    //
    // General conditions:
    // 1) Allow movement to just be over a single row, a single column or
    // diagonally.
    // 2) The piece cannot stay on its original tile.
    // 3) The ball must be moved to an unoccupied tile.
    // 4) The tile is valid (is not next to either one of the goals).
    // 5) The ball cannot be moved to a tile contiguous to all of the
    // player pieces. This is VERY a rare case.
    //
    // End-of-turn conditions:
    // 6) The ball cannot be in the possession of any team at the end of
    // the turn.
    // 7) The player cannot be in the team's own area at the end of the turn.
    // 8) The ball cannot be in the team's own corner at the end of the turn.
    //
    // If all of those conditions are met, returns "true"; false otherwise.
    private bool CheckForValidBallMove(int x1, int y1, int x2, int y2)
    {
        // Get the team whose current turn it is and the opposite team.
        Team teamColor = this.currentTurn;

        // Check all of the aforementioned conditions.
        //
        // General conditions.
        bool validDirection = CheckForValidMovementDirections(x2-x1, y2-y1); // 1 & 2
        bool tileIsFree = !DoesTileContainAPiece(x2,y2); // 3
        bool tileIsValid = Board.GetTile(x2, y2).IsValid; // 4
        bool tileNotCongiuousToAllPieces = (
            CountContiguousPieces(x2, y2, Team.White) != 2 ||
            CountContiguousPieces(x2, y2, Team.Black) != 2); // 5
        bool generalConditions = (
            validDirection && tileIsFree && tileIsValid &&
            tileNotCongiuousToAllPieces);

        // End-of-turn conditions.
        bool isEndOfTurn = passCount == 3 || IsTileFree(x2,y2);
        bool isDestinationTileFree = IsTileFree(x2,y2); // 6
        bool notOwnArea = !IsItsOwnArea(x2, y2, teamColor); // 7
        bool notOwnCorner = !IsItsOwnCorner(x2, y2, teamColor); // 8
        bool endOfTurnConditions = (
            !isEndOfTurn ||
            isDestinationTileFree && notOwnArea && notOwnCorner);

        // If it is not the end of the current turn, only the general
        // conditions must be met.
        if (!isEndOfTurn)
        {
            return generalConditions;
        }
        // If it is the end of the current turn, both the general
        // conditions and the end-of-turn conditions must be met.
        else
        {
            return generalConditions && endOfTurnConditions;
        }

        /*
        return (
            // General conditions.
            CheckForValidMovementDirections(x2-x1, y2-y1) && // 1 & 2
            !DoesTileContainAPiece(x2,y2) && // 3
            Board.GetTile(x2, y2).IsValid && // 4
            (CountContiguousPieces(x2, y2, Team.White) != 2 ||
             CountContiguousPieces(x2, y2, Team.Black) != 2) && // 5
            // End-of-turn conditions.
            !(
                // The turn ends when the pass count is 3 or when the
                // destination tile is free.
                (passCount == 3 || IsTileFree(x2,y2)) &&
                // If the turn ends, end of turn conditions must be met.
                (!IsTileFree(x2,y2) || // 6
                 IsItsOwnArea(x2, y2, teamColor) || // 7
                 IsItsOwnCorner(x2, y2, teamColor)) // 8
                 )
                 );
        */
    }

    // Check if the tile located on the given coordinates is in
    // "possession" of the given team.
    //
    // This method is useful to determine the validity of potential
    // ball moves.
    private bool IsTileInPossessionOfTeam(int x, int y, Team teamColor)
    {
        // Get the opposite team's color.
        Team oppositeColor = GetOppositeTeam(teamColor);

        // Count the pieces around the selected tile.
        int teamCount = CountContiguousPieces(x, y, teamColor);
        int oppositeCount = CountContiguousPieces(x, y, oppositeColor);

        return (
            (this.currentTurn == teamColor && teamCount > oppositeCount) ||
            (this.currentTurn == oppositeColor && oppositeCount > teamCount)
            );
    }

    // Checks if a tile given by the coordinates is free. A tile is free
    // when no team is in "possession" of it.
    private bool IsTileFree(int x, int y)
    {
        return (
            !IsTileInPossessionOfTeam(x,y,Team.White) &&
            !IsTileInPossessionOfTeam(x,y,Team.Black));
    }

    // Takes a team and returns the opposite team.
    private Team GetOppositeTeam(Team team)
    {
        return ((team == Team.White) ? Team.Black : Team.White);
    }

    // Checks if a goal has been scored based on the current position
    // of the ball.
    //
    // Returns the scoring team if a goal has been scored; null otherwise.
    private Team? CheckForGoalScored()
    {
        Ball ball = Board.Ball;

        // Look for the ball on the white team's goal.
        if (ball.Y == 0)
        {
            return Team.White;
        }
        // Look for the ball on the black team's goal.
        else if (ball.Y == 14)
        {
            return Team.Black;
        }

        // If no goal has been scored, return null.
        return null;
    }

    // Checks if a team is currently in possession of the ball and if
    // that team is the team whose turn it is, currently.
    //
    // If those conditions are met, the player can move the ball, either
    // to make it a pass or not.
    private bool IsBallInPossessionOfCurrentTurn()
    {
        // Check if the ball can be passed or if we must switch turns.
        Team? ballPossesion = GetBallPossession(Board.Ball);
        return (
            ballPossesion != null &&
            ballPossesion == this.currentTurn);
    }

    // Takes the ball piece and returns the team that is currently in
    // possession of the ball. Returns null if no team is in possession
    // of the ball.
    private Team? GetBallPossession(Ball ball)
    {
        // Count the pieces next to the ball.
        int whiteCount = CountContiguousPieces(
            ball.X, ball.Y, Team.White);
        int blackCount = CountContiguousPieces(
            ball.X, ball.Y, Team.Black);

        // Return the team color that is in possession of the ball.
        if (whiteCount > blackCount)
        {
            return Team.White;
        }
        else if (blackCount > whiteCount)
        {
            return Team.Black;
        }
        else
        {
            return null;
        }
    }

    // Takes the coordinates of a tile and a team color as argument and
    // counts the pieces of that color that are contiguous to the tile.
    private int CountContiguousPieces(int x, int y, Team teamColor)
    {
        // Set the limits for the nested iteration.
        int minX = Math.Max(0, x-1);
        int maxX = Math.Min(
            Board.GetXLength() - 1, x+1);
        int minY = Math.Max(0, y-1);
        int maxY = Math.Min(
            Board.GetYLength() - 1, y+1);
        
        int count = 0;

        // Check the tiles that are contiguous to the ball for player pieces.
        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j <= maxX; j++)
            {
                // Get the piece, if there is any.
                PlayerPiece? piece = Board.GetTile(j,i).PlayerPiece;

                // Count the pieces of the given color.
                if (
                    (j != x || i != y) &&
                    piece != null && piece.TeamColor == teamColor)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Switches the current turn from "White" to "Black", or viceversa.
    private void SwitchCurrentTurn()
    {
        if (this.currentTurn == Team.White)
        {
            this.currentTurn = Team.Black;
        }
        else
        {
            this.currentTurn = Team.White;
        }
    }
}
