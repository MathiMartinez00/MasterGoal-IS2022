using System;

// The instance of this class represents the abstract ball.
// There should only be a single instance of this class per game.
[Serializable]
public class Ball : Piece
{
    public Ball(int x, int y) : base(x,y)
    {
        // (Save the position with the parent class' constructor).
    }
}
