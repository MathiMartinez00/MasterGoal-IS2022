
// This is a class that encapsulates the functionality for the IA.
// For every computer move a new instance of this class should be
// instantiated.
public class AIModule
{
    // We need a list of moves because this game allows passes between
    // players, and a single pass is represented as a single move.
    public Move[] Moves { get; private set; }

    // The recursion depth to be used by the Minimax algorithm.
    private readonly int recursionDepth = 4;
    // Maximum possible score. This score should be used when a move
    // results in a goal.
    private readonly int maxScore = 9999;

    // This class constructor takes a game, calculates the recommended
    // moves and saves them on the "Moves" instance attribute.
    public AIModule(Game game)
    {
        // Get all of the children states of the current game state.
        Game[] childrenStates = GetChildrenStates(game);
        // Get the current turn in the form of a bool.
        bool isMaximizingPlayer = game.CurrentTurn == Team.White;
        // To store the child state with the highest calculated utility.
        Game bestChildState;

        // Find the child state with the highest utility score.
        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (Game childState in childrenStates)
            {
                int currentEval = Minimax(childState, recursionDepth, false);
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
                int currentEval = Minimax(childState, recursionDepth, true);
                if (currentEval < minEval)
                {
                    bestChildState = childState;
                    minEval = currentEval;
                }
            }
        }
        
        // Store the moves that were made in order to get from the parent
        // state to the child state.
        Moves = GetMovesFromTheLatestTurn(game, bestChildState);
    }

    private Game[] GetChildrenStates(Game game)
    {

    }

    // Standard Minimax function. Takes a game state, a recursion depth
    // and a bool (that defines the current turn).
    public int Minimax(Game game, int depth, bool isMaximizingPlayer)
    {
        // If we have reached maximum depth in the game tree, return
        // the static evaluation of the current state of the game.
        if (depth == 0 || IsGameOver(game))
        {
            return EvaluateBoard(game);
        }

        // Get all of the children states of the current game state.
        Game[] childrenStates = GetChildrenStates(game);

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (Game childState in childrenStates)
            {
                int currentEval = Minimax(childState, depth-1, false)
                maxEval = Math.Max(maxEval, currentEval);
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (Game childState in childrenStates)
            {
                int currentEval = Minimax(childState, depth-1, true)
                minEval = Math.Min(minEval, currentEval);
            }
            return minEval;
        }
    }

    // Heuristic evaluation function. Given a board, it returns an integer
    // representing the utility score of the given state of the board.
    // The heuristic evaluation is made based on just the "y" coordinate
    // of the ball. If the ball is close to the black team's goal, the
    // utility score will be positive and high.
    // A state that favors the white team will get a positive score and a
    // state that favor the black team will get a negative score.
    private int EvaluateBoard(Game game)
    {
        int utilityScore = 0;
        // The evalution will be made based on the ball's "y" coordinate.
        int ballY = game.Board.BallY;

        // If a goal is scored, assign the maximum (or minimum) score.
        if (game.CheckForGoalScored() == Team.White)
        {
            // White is positive.
            utilityScore = maxScore;
        }
        else if (game.CheckForGoalScored() == Team.Black)
        {
            // Black is negative.
            utilityScore = -maxScore;
        }
        else
        {
            utilityScore = (ballY - 7) * 100;
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
    private Move[] GetMovesFromTheLatestTurn(Game parentState, Game childState)
    {
        List<Move> parentList = parentState.AllMoves;
        List<Move> childList = childState.AllMoves;

        List<Move> newMoves;

        for (int i = parentList.Count; i < childList.Count; i++)
        {
            newMoves.Add(childList[i]);
        }

        return newMoves;
    }
}