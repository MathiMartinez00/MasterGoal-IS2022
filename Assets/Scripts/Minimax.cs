using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Minimax
{
    public int minimax(Vector3 position, int depth, bool maximizingPlayer)
    {
        if ((depth == 0) || isGameOver())
        {
            return evaluateBoard();
        }

        (int,int)[] positions = getChildrenPositions();

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

    private (int,int)[] getChildrenPositions()
    {
        (int,int)[] positions = {(0,0), (0,0)};
        return positions;
    }

    private int evaluateBoard()
    {
        return 0;
    }

    public bool isGameOver()
    {
        return false;
    }
}
