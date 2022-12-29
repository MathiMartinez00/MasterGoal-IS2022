using System;
using System.Collections.Generic;
using ExtensionMethods;

// This is a class that encapsulates the functionality for the IA.
// For every computer move a new instance of this class should be
// instantiated.
public class AIModule
{
    // We need a list of moves because this game allows passes between
    // players, and a single pass is represented as a single move.
    public List<Move> Moves { get; private set; }
    // The child game state with the best utility score will be
    // stored here.
    public Game ChildGame { get; private set; }

    // The recursion depth to be used by the Minimax algorithm.
    private readonly int recursionDepth = 0;
    // Maximum possible score. This is the utility score of a move
    // that ends the game.
    private readonly int maxScore = 100000;
    // This is the utility score of a single goal.
    private readonly int goalScore = 10000;
    // Utility score of every increment in the "y" coordinate of the ball.
    private readonly int positionScore = 100;

    // This class constructor takes a game, calculates the recommended
    // moves and saves them on the "Moves" instance attribute.
    public AIModule(Game game)
    {
        // Get all of the children states of the current game state.
        List<Game> childrenStates = GetChildrenStates(game);
        // Get the current turn in the form of a bool.
        bool isMaximizingPlayer = game.CurrentTurn == Team.White;
        // To store the child state with the highest calculated utility.
        Game bestChildState = default;

        // Find the child state with the highest utility score.
        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (Game childState in childrenStates)
            {
                int currentEval = Minimax(
                    childState, recursionDepth, int.MinValue, int.MaxValue, false);
                if (currentEval > maxEval)
                {
                    bestChildState = childState;
                    maxEval = currentEval;
                }
            }
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (Game childState in childrenStates)
            {
                int currentEval = Minimax(
                    childState, recursionDepth, int.MinValue, int.MaxValue, true);
                if (currentEval < minEval)
                {
                    bestChildState = childState;
                    minEval = currentEval;
                }
            }
        }
        
        // Store the child game state with the best utility score.
        ChildGame = bestChildState;
        // Store the moves that were made in order to get from the parent
        // state to the child game state.
        Moves = GetMovesFromTheLatestTurn(game, bestChildState);
    }

    // Given a parent game state it returns all of its direct
    // descendants (or children).
    private List<Game> GetChildrenStates(Game game)
    {
        List<Game> childGameStates = new List<Game>();

        // Get the two player piece of the current team.
        foreach (PlayerPiece piece in GetPlayerPiecesOfCurrentTurn(game))
        {
            // Make a deep game copy.
            Game gameCopy = game.DeepClone();

            // Select a piece in the game copy. Player piece selection
            // is always the first action in a turn.
            Position position = new Position(piece.X, piece.Y);
            gameCopy.Input(position);

            // Get all of the possible games starting from a game state
            // where a player piece is selected and store them.
            childGameStates.AddRange(GetAllGamesOnPlayerSelected(gameCopy));
        }

        // Return all the children of the game states where each one of
        // the two player pieces of the current team gets selected.
        return childGameStates;
    }

    // Takes a game state where a player is already selected and returns
    // all of the possible direct descendant game states of it.
    private List<Game> GetAllGamesOnPlayerSelected(Game game)
    {
        List<Game> childGameStates = new List<Game>();

        // Iterate through the highlighted tiles.
        foreach(AbstractTile tile in game.Board.GetHighlightedTilesIterative())
        {
            // Make a deep game copy.
            Game gameCopy = game.DeepClone();

            // Move the piece in the game copy. This is always the second
            // action in a turn (and can be the last action of a turn).
            Position position = new Position(tile.X, tile.Y);
            gameCopy.Input(position);

            // Get all of the possible games starting from a game state
            // where a player piece is moved to a highlighted tile.
            childGameStates.AddRange(GetAllGamesOnPlayerMoved(gameCopy));
        }

        // Return all the children of the game states in which the player
        // is moved to each one of the highlighted tiles.
        return childGameStates;
    }

    // Recursive method that takes a game state where a player piece or the
    // ball was recently moved and returns all of the possible direct
    // descendant game states of it. This function is recursive because a
    // turn can end with a player move or with a ball move (and maybe after
    // up to three ball passses). The base case of the recursion is when
    // the ball can't be moved further and the turn ends. The recursive case
    // is when the last move didn't end the turn and the ball can be moved
    // further.
    private List<Game> GetAllGamesOnPlayerMoved(Game game)
    {
        List<Game> childGameStates = new List<Game>();

        // Base case. The last move ended the turn and the ball can't
        // be moved further.
        if (game.GameStatus != GameStatus.WaitingBallMovement)
        {
            // Just return a list with the current game state.
            childGameStates.Add(game);
            return childGameStates;
        }
        // Recursive case. The last move that was made didn't end the turn
        // and the ball can still be moved.
        else
        {
            // Iterate through the highlighted tiles.
            foreach(
                AbstractTile tile in game.Board.GetHighlightedTilesIterative())
            {
                // Make a deep game copy.
                Game gameCopy = game.DeepClone();

                // Move the ball in the game copy. This move doesn't
                // necessarily end the turn.
                Position position = new Position(tile.X, tile.Y);
                gameCopy.Input(position);

                // Get all of the possible games that result from moving the
                // ball to each one of the highlighted tiles.
                childGameStates.AddRange(GetAllGamesOnPlayerMoved(gameCopy));
            }
        }

        return childGameStates;
    }

    // Takes a game and a returns a list with the two player pieces of
    // the team whose turn it is.
    private List<PlayerPiece> GetPlayerPiecesOfCurrentTurn(Game game)
    {
        List<PlayerPiece> playerPieces = new List<PlayerPiece>();

        if (game.CurrentTurn == Team.White)
        {
            playerPieces.Add(game.Board.WhitePiece1);
            playerPieces.Add(game.Board.WhitePiece2);
        }
        else
        {
            playerPieces.Add(game.Board.BlackPiece1);
            playerPieces.Add(game.Board.BlackPiece2);
        }

        return playerPieces;
    }

    // Standard Minimax function with alpha-beta pruning. Takes a game state,
    // a recursion depth, the alpha and beta parameters and a bool (that
    // defines the current turn).
    public int Minimax(
        Game game, int depth, int alpha, int beta, bool isMaximizingPlayer)
    {
        // If we have reached maximum depth in the game tree, return
        // the static evaluation of the current state of the game.
        if (depth == 0 || IsGameOver(game))
            return EvaluateGame(game);

        // Get all of the children states of the current game state.
        List<Game> childrenStates = GetChildrenStates(game);

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (Game childState in childrenStates)
            {
                int currentEval = Minimax(
                    childState, depth-1, alpha, beta, false);
                maxEval = Math.Max(maxEval, currentEval);
                // Check for the pruning condition.
                alpha = Math.Max(alpha, currentEval);
                if (beta <= alpha)
                    break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (Game childState in childrenStates)
            {
                int currentEval = Minimax(
                    childState, depth-1, alpha, beta, true);
                minEval = Math.Min(minEval, currentEval);
                // Check for the pruning condition.
                beta = Math.Min(beta, currentEval);
                if (beta <= alpha)
                    break;
            }
            return minEval;
        }
    }

    // Heuristic evaluation function. Given a game, it returns an integer
    // representing the utility score of the given state of the game.
    // The heuristic evaluation is made based on the game state (whether
    // the game is still going), the number of goals and the current position
    // of the ball.
    // A state that favors the white team will get a positive score and a
    // state that favor the black team will get a negative score.
    private int EvaluateGame(Game game)
    {
        int utilityScore = 0;
        // Part of the evalution will be made based on the "y" coordinate.
        int ballY = game.Board.Ball.Y;

        // If the game is over, return the maximum (or minimum) possible
        // score. This factor has the highest weight on the utility score.
        if (IsGameOver(game))
        {
            utilityScore =
            (game.WhiteScore > game.BlackScore) ? maxScore : -maxScore;
        }
        else
        {
            // This is the score derived from the amount of goals that each
            // team made. This score has a high weight.
            int numberOfGoalsScore =
            (game.WhiteScore * goalScore) - (game.BlackScore * goalScore);

            // This is the score derived from the current position of the
            // ball in the game board. This score has a low weight.
            int ballPositionScore = (ballY - 7) * positionScore;

            // Add the two scores to get the total utility score.
            utilityScore = numberOfGoalsScore + ballPositionScore;
        }

        return utilityScore;
    }

    // Checks if the game is over.
    public bool IsGameOver(Game game)
    {
        return (game.GameStatus == GameStatus.GameOver);
    }

    // Takes a parent game state and a child game state —a game state that
    // is a direct descendant of the first— and returns the latest moves
    // that were made in order to get from the parent game to the child game.
    private List<Move> GetMovesFromTheLatestTurn(Game parentState, Game childState)
    {
        List<Move> parentList = parentState.AllMoves;
        List<Move> childList = childState.AllMoves;

        List<Move> newMoves = new List<Move>();

        for (int i = parentList.Count; i < childList.Count; i++)
            newMoves.Add(childList[i]);

        return newMoves;
    }
}