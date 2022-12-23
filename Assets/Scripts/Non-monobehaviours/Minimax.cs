
public class Minimax
{
    private int EvalScore;

    public Minimax(Game game, int depth, bool maximizingPlayer)
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

    private (int,int)[] GetChildrenPositions()
    {
        (int,int)[] positions = {(0,0), (0,0)};
        return positions;
    }

    private int EvaluateBoard()
    {
        return 0;
    }

    public bool IsGameOver()
    {
        return false;
    }
}