
public class Minimax
{
    public Minimax(Game game, int depth, bool maximizingPlayer)
    {
        if ((depth == 0) || IsGameOver())
        {
            return EvaluateBoard();
        }

        (int,int)[] positions = GetChildrenPositions();

        if (maximizingPlayer)
        {
            int maxEval = int.MaxValue;
            return maxEval;
        }
        else
        {
            int minEval = int.MinValue;
            return minEval;
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