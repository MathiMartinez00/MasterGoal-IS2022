using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

#nullable enable

public class ScriptController : MonoBehaviour
{
    // Used to get information to and from the AI algorithm.
    public Game Game { get; set; } = default!;
    public GameMode GameMode { get; private set; }

    // GameObjects for game control.
    //
    public Tilemap TilemapBoard = default!;
    public Tilemap TilemapHighlight = default!;
    // A PNG file that signals the highlighting.
    public Tile TileHighlight = default!;
    public BoxCollider2D BoardBoxCollider = default!;
    // The five game pieces.
    public GameObject WhitePiece1 = default!;
    public GameObject WhitePiece2 = default!;
    public GameObject BlackPiece1 = default!;
    public GameObject BlackPiece2 = default!;
    public GameObject Ball = default!;

    // UI variables
    public TextMeshProUGUI whiteScoreText = default!;
    public TextMeshProUGUI redScoreText = default!;
    public TextMeshProUGUI whiteScoreName = default!;
    public TextMeshProUGUI redScoreName = default!;
    public TextMeshProUGUI winnerName = default!;
    public string WhiteName { get; private set; } = default!;
    public string RedName { get; private set; } = default!;
    public int WhiteScore { get; set; } = default!;
    public int RedScore { get; set; } = default!;

    // Start is called before the first frame update.
    void Start()
    {
        WhiteScore = 0;
        RedScore = 0;
        WhiteName = PlayerPrefs.GetString("player1");
        RedName = PlayerPrefs.GetString("player2");
        this.whiteScoreName.text = WhiteName;
        this.redScoreName.text = RedName;
        BoardBoxCollider = TilemapBoard.gameObject.GetComponent<BoxCollider2D>();
        // Create a new abstract game instance.
        Game = new Game(Team.White);
        GameMode = GameMode.TwoPlayers;
    }

    // This method is called whenever the user taps on the screen.
    public void UpdateBoard(PointerEventData eventData)
    {
        // Updates board based on player input given by the
        // IPointerHandlerEvent on the tilemap board GameObject.
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(
            eventData.position);
        // Get the position of the screen tap in the Vector3Int type.
        Vector3Int point = TilemapBoard.WorldToCell(worldPoint);

        // Convert the Vector3Int to matrix coordinates using
        // the Position object.
        Position position = new Position(point);

        // Pass the input to the Game instance and get the Move instance
        // that resulted from the input.
        Move? move = Game.Input(position);

        // Render the changes that resulted from this interaction with
        // the user.
        RenderChanges(move);

        // If the game mode is single player, then the computer has
        // to make a move now.
        if (GameMode == GameMode.OnePlayer && Game.CurrentTurn == Team.Black)
        {
            // Create a new AIModule instance. This class encapsulates
            // the recommended moves in an instance field.
            AIModule aiModule = new AIModule(Game);
            // Get the recommended moves (the moves with the highest
            // utility score) and render them.
            RenderChanges(aiModule.Moves);
            // Get the child game state and replace the old one.
            Game = aiModule.ChildGame;
        }
    }

    // Takes a series of moves (or just one move, or no move) that were
    // made on the game and moves the corresponding concrete piece.
    private void RenderChanges(List<Move> moves)
    {
        foreach (Move move in moves)
        {
            RenderChanges(move);
        }
    }

    // Takes a move (or no move) that was made on the abstract game
    // representation and moves the corresponding concrete piece.
    private void RenderChanges(Move? move)
    {
        if (move != null)
        {
            // Check for any piece movement that didn't result in a goal.
            if (!move.IsGoal)
            {
                // Check what kind of piece was recently moved and move
                // it on the concrete board.
                if (move.PlayerPiece != null)
                {
                    MoveConcretePlayer(move.PlayerPiece);
                }
                else if (move.BallMoved != null)
                {
                    MoveConcreteBall(move.BallMoved);
                }
            }
            // If a goal has been made, all of the pieces need to be reset.
            else if (move.IsGoal)
            {
                MoveConcretePlayer(Game.Board.WhitePiece1);
                MoveConcretePlayer(Game.Board.WhitePiece2);
                MoveConcretePlayer(Game.Board.BlackPiece1);
                MoveConcretePlayer(Game.Board.BlackPiece2);
                MoveConcreteBall(Game.Board.Ball);
            }
        }

        // Render the new highlights, if there are any.
        RenderHighlights(Game.Board);
    }

    // Takes the abstract board as argument and checks for the tiles that
    // are highlighted. It "translates" those hightlights to the concrete
    // game board.
    private void RenderHighlights(Board board)
    {
        // Clear all of the highlights on the tilemap.
        TilemapHighlight.ClearAllTiles();

        // Iterate through the abstract tiles and highlight the concrete ones.
        foreach(AbstractTile tile in board.GetIterativeTiles())
        {
            // Check if the tile is valid and if it's highlighted.
            if (tile.IsValid && tile.IsHighlighted)
            {
                // Create a new position object (to make a unit conversion).
                Position position = new Position(tile.X, tile.Y);

                // Highlight the concrete tile.
                TilemapHighlight.SetTile(
                    position.Vector3Int, TileHighlight);
            }
        }
    }

    // Takes an abstract player piece and returns the concrete GameObject
    // that it is supossed to represent.
    private GameObject AbstractPlayerToConcrete(PlayerPiece piece)
    {
        GameObject result = WhitePiece1;
        if (
            piece.TeamColor == Team.White &&
            piece.PieceNumber == PieceNumber.One)
        {
            result = WhitePiece1;
        }
        else if (
            piece.TeamColor == Team.White &&
            piece.PieceNumber == PieceNumber.Two)
        {
            result = WhitePiece2;
        }
        else if (
            piece.TeamColor == Team.Black &&
            piece.PieceNumber == PieceNumber.One)
        {
            result = BlackPiece1;
        }
        else if (
            piece.TeamColor == Team.Black &&
            piece.PieceNumber == PieceNumber.Two)
        {
            result = BlackPiece2;
        }

        return result;
    }

    // Takes an abstract Player Piece (that was moved), finds the real
    // piece that it represents and updates its position (of the real one).
    private void MoveConcretePlayer(PlayerPiece playerPiece)
    {
        // Translate from the abstract player piece to the concrete one.
        GameObject realPiece = AbstractPlayerToConcrete(playerPiece);
        // Create a new position object (to make a unit conversion).
        Position position = new Position(playerPiece.X, playerPiece.Y);
        // Set the new coordinates for the piece.
        realPiece.transform.position = TilemapBoard.GetCellCenterWorld(
            position.Vector3Int);
    }

    // Takes an abstract Ball and based on its current position, updates
    // the position of the real one.
    private void MoveConcreteBall(Ball ball)
    {
        // Create a new position object (to make a unit conversion).
        Position position = new Position(ball.X, ball.Y);
        // Set the new coordinates for the ball.
        Ball.transform.position = TilemapBoard.GetCellCenterWorld(
            position.Vector3Int);
    }
}