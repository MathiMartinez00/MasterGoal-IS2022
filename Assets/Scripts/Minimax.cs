using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Minimax
{
    public void minimax(Vector3 position, int depth, bool maximizingPlayer)
    {
        if ((depth == 0) || isGameOver())
        {
            return evaluateBoard();
        }

        List<Vector3> positions = getChildrenPositions();

        if (maximizingPlayer)
        {
            maxEval = int.MaxValue;
            return maxEval;
        }
        else
        {
            minEval = int.MinValue;
            return minEval;
        }
    }

    private List<Vector3> getChildrenPositions()
    {
        foreach (var chip in playerChips)
        {
            Debug.Log("chip: " + chip.transform.position);
            if (chip.transform.position == point)
            {
                return chip;
            }
        }
    }

    private int evaluateBoard()
    {}

    public void UpdateBoard(PointerEventData eventData)
    {}
}
