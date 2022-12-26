
public class AIModule
{
    // We need a list of moves because this game allows passes between
    // players, and a single pass is represented as a single move.
    public Move[] moves { get; private set; }

    private readonly int maxScore = 9999;

    public AIModule(Game game)
    {

    }

    public int Minimax(Game game, int depth, bool maximizingPlayer)
    {
        if ((depth == 0) || IsGameOver())
        {
            this.EvalScore = EvaluateBoard();
        }

        (int,int)[] positions = GetChildrenPositions();

        if (maximizingPlayer)
        {
            int maxEval = int.MaxValue;
            this.EvalScore = maxEval;
        }
        else
        {
            int minEval = int.MinValue;
            this.EvalScore = minEval;
        }
    }

    private Game[] GetChildrenPositions(Game game)
    {
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

    public bool IsGameOver(Game game)
    {
        return (game.GameStatus == GameStatus.GameOver);
    }
}