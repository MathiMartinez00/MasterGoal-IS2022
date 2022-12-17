
// An instance of this class represents an abstract representation of the
// state of the game.
// This is a standalone C# class and doesn't inherit from Monobehavior,
// so it will not be attached to any Unity GameObject.
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
        WaitingBallMovement,
        GameOver
    }

    public enum Team
    {
        White,
        Black
    }

    private Board board;
    // GAME MODE NOT USED YET.
    private GameMode gameMode;
    private GameStatus gameStatus;
    private Team currentTurn;
    private List<Move> allMoves;
    private Nullable<PlayerPiece> selectedPiece;
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
        this.allMoves = new List<Move>();
        this.currentBallPossession = null;
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
    public Nullable<Move> UserInput(Position position)
    {
        // Conver the coordinates to x and y.
        int x = position.GetX();
        int y = position.GetY();

        // Get the selected tile.
        Nullable<Piece> selectedTile = this.board.GetTile(x,y);

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
                // Highlight and store the tiles that indicate valid destinations.
                CalculatePlayerMovesAndHighlightTiles(
                    selectedTile, selectedPiece);
                // Change the status of the game.
                this.gameStatus = WaitingPlayerPieceMovement;
                // Store the selected piece.
                this.selectedPiece = piece.Value;
            }

            // Because no moves were made on this conditional branch.
            return null;
        }
        else if (
            gameStatus == WaitingPlayerPieceMovement &&
            selectedTile.IsTileHighlighted())
        {
            // Move the piece and store the move.
            Move move = MovePiece(this.selectedPiece, this.selectedTile);
            // Clear all of the highlighted tiles.
            this.board.ClearAllHighlights();
            // Deselect the piece.
            this.selectedPiece = null;
            // Check if the team that made the last player move is in
            // possession of the ball.
            if (IsBallInPossessionOfCurrentTurn())
            {
                // Change the status of the game.
                this.gameStatus = WaitingBallMovement;
                // Highlight the valid destinations for the ball
                // for the next method call.
                CalculateBallMovesAndHighlightTiles();
            }
            else
            {
                // Change the status of the game.
                this.gameStatus = WaitingPlayerPieceSelection;
                // Switch the current turn.
                SwitchCurrentTurn();
            }

            // Return the move (to do a reactive board update).
            return move;
        }
        else if (
            gameStatus == WaitingPlayerBallMovement &&
            selectedTile.IsTileHighlighted())
        {
            // Move the ball and store the move.
            Move move = MovePiece(this.board.GetBall(), this.selectedTile);

            // Clear all of the highlighted tiles.
            this.board.ClearAllHighlights();

            // Check if a goal has been scored.
            Nullable<Team> goalScored = CheckForGoalScored();
            if (goalScored.HasValue)
            {
                // Mark that the latest move resulted in a goal.
                move.SetIsGoal(true);

                // Update the scores.
                if (goalScored.Value == White)
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
                    this.gameStatus = GameOver;
                }
                else
                {
                    // After a goal, it's the opposite team's turn.
                    this.currentTurn = GetOppositeTeam(goalScored.Value);
                    // Change the status of the game.
                    this.gameStatus = WaitingPlayerPieceSelection;
                    // Reset the pieces to their initial position.
                    this.board.ResetPieces();
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
                    this.gameStatus = WaitingPlayerPieceSelection;
                    // Switch the current turn.
                    SwitchCurrentTurn();
                }
            }

            // Return the move (to do a reactive board update).
            return move;
        }
    }

    // Takes a piece and a pair of coordinates and moves the piece
    // to the tile located at the given coordinates. In this process,
    // it also changes the x and y position of the Piece object and
    // sets the old tile's "piece" field to null.
    //
    // This method doesn't validate the move. Validation is made by
    // highlighting tiles.
    private Move MovePiece(Piece piece, AbstractTile destinationTile)
    {
        // Origin coordinates.
        int x1 = piece.GetX();
        int y1 = piece.GetY();
        // Destination coordinates.
        int x2 = destinationTile.GetX();
        int y2 = destinationTile.GetY();

        // Set the origin tile's "piece" field to null.
        AbstractTile originTile = this.board.GetTile(x1,y1);
        originTile.SetPiece(null);
        // Set the destination tile's field to the correct reference.
        destinationTile.SetPiece(piece);
        // Update the piece's fields.
        piece.SetX(x2);
        piece.SetY(y2);

        // Store the move.
        Move move = new Move(
            this.currentTurn, originTile, destinationTile, piece);
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

    // Check for these conditions:
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
            !IsItsOwnCorner(x2, y2) && // 1
            !IsAnotherPieceInTheWay(x1, y1, x2, y2) && // 2
            CheckForValidMovementDirections(x2-x1, y2-y1) && // 3 & 5
            this.board.GetTile(x2, y2).IsTileValid() // 4
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
        Team thisTeam     = this.currentTurn;
        Team oppositeTeam = GetOppositeTeam(this.currentTurn);

        return (
            // General conditions.
            CheckForValidMovementDirections(x2-x1, y2-y1) && // 1 & 2
            !this.board.GetTile(x2, y2).GetPiece().HasValue && // 3
            this.board.GetTile(x2, y2).IsTileValid() && // 4
            (CountContiguousPieces(x2, y2, White) != 2 ||
             CountContiguousPieces(x2, y2, Black) != 2) && // 5
            // End-of-turn conditions.
            !(
                // The turn ends when the pass count is 3 or when the
                // destination tile is free.
                (passCount == 3 || IsTileFree(x2,y2)) &&
                // If the turn ends, end of turn conditions must be met.
                (!IsTileFree(x2,y2) || // 6
                 IsItsOwnArea(x2, y2) || // 7
                 IsItsOwnCorner(x2, y2)) // 8
                 )
              )
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
            )
    }

    // Checks if a tile given by the coordinates is free. A tile is free
    // when no team is in "possession" of it.
    private bool IsTileFree(int x, int y)
    {
        return (
            !IsTileInPossessionOfTeam(x,y,White) &&
            !IsTileInPossessionOfTeam(x,y,Black))
    }

    // Takes a team and returns the opposite team.
    private Team GetOppositeTeam(Team team)
    {
        return ((team == White) ? Black : White)
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
    private bool IsBallInPossessionOfCurrentTurn()
    {
        // Check if the ball can be passed or if we must switch turns.
        Nullable<Team> ballPossesion =
            GetBallPossession(this.board.GetBall());
        return (
            ballPossesion.HasValue &&
            ballPossesion.Value == this.currentTurn)
    }

    // Takes the ball piece and returns the team that is currently in
    // possession of the ball. Returns null if no team is in possession
    // of the ball.
    private Nullable<Team> GetBallPossession(Ball ball)
    {
        // Count the pieces next to the ball.
        int whiteCount = CountContiguousPieces(
            ball.GetX(), ball.GetY(), White);
        int blackCount = CountContiguousPieces(
            ball.GetX(), ball.GetY(), Black);

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

    // Takes the coordinates of a tile and a team color as argument and
    // counts the pieces of that color that are contiguous to the tile.
    private int CountContiguousPieces(int x, int y, Team teamColor)
    {
        // Set the limits for the nested iteration.
        int minX = Math.Max(0, x-1);
        int maxX = Math.Min(
            this.board.GetXLength() - 1, x+1);
        int minY = Math.Max(0, y-1);
        int maxY = Math.Min(
            this.board.GetYLength() - 1, y+1);
        
        int count = 0;

        // Check the tiles that are contiguous to the ball for player pieces.
        for (int i = minY; i <= maxX; i++)
        {
            for (int j = minX; j <= maxY; j++)
            {
                // Get the piece, if there is any.
                Nullable<Piece> piece = this.board.GetTile(j,i).GetPiece();

                // Count the pieces of the given color.
                if (
                    (j != x && i != y) &&
                    piece.HasValue && piece.Value.teamColor == teamColor)
                {
                    count++;
                }
            }
        }

        return count;
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
