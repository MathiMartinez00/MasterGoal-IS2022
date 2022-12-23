using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;


public class ScriptController : MonoBehaviour
{
    // Used to get information to and from the AI algorithm.
    public Game Game { get; set; }

    // GameObjects for game control.
    //
    public Tilemap TilemapBoard;
    public Tilemap TilemapHighlight;
    // A PNG file that signals the highlighting.
    public Tile TileHighlight { get; set; }
    public BoxCollider2D BoardBoxCollider { get; set; }
    // The five game pieces.
    public GameObject WhitePiece1 { get; set; }
    public GameObject WhitePiece2 { get; set; }
    public GameObject BlackPiece1 { get; set; }
    public GameObject BlackPiece2 { get; set; }
    public GameObject Ball { get; set; }
    
    // Movement settings
    //public float smoothDampTime = .02f;
    //public float distanceToSnap = .05f;

    // UI variables
    public TextMeshProUGUI whiteScoreText, redScoreText;
    public TextMeshProUGUI whiteScoreName, redScoreName;
    public TextMeshProUGUI winnerName;
    public string WhiteName { get; private set; }
    public string RedName { get; private set; }
    public int WhiteScore { get; set; }
    public int RedScore { get; set; }

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
        Game = new Game(GameMode.TwoPlayers, Team.White);
    }

    // This method is called whenever the user taps on the screen.
    public void UpdateBoard(PointerEventData eventData)
    {
        // Updates board based on player input given by the
        // IPointerHandlerEvent on the tilemap board GameObject.
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3Int point = TilemapBoard.WorldToCell(worldPoint);

        // Convert the Vector3Int to matrix coordinates using
        // the Position object.
        Position position = new Position(point);
        Move? move = Game.UserInput(position);

        // Render the changes that resulted from this interaction with
        // the user.
        RenderChanges(move);
    }

    // Takes the last move that was made on the game and moves the
    // corresponding concrete piece.
    private void RenderChanges(Move? move)
    {
        if (move != null)
        {
            // Check for any piece movement that didn't result in a goal.
            if (!move.GetIsGoal())
            {
                // Get the piece that was recently moved on the abstract
                // board and move it on the concrete board.
                MoveConcretePiece(move.GetPieceMoved());
            }
            // If a goal has been made, all of the pieces need to be reset.
            else if (move.GetIsGoal())
            {
                MoveConcretePiece(Game.Board.WhitePiece1);
                MoveConcretePiece(Game.Board.WhitePiece2);
                MoveConcretePiece(Game.Board.BlackPiece1);
                MoveConcretePiece(Game.Board.BlackPiece2);
                MoveConcretePiece(Game.Board.Ball);
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
            if (tile.IsTileValid && tile.IsHighlighted)
            {
                // Create a new position object (to make a unit conversion).
                Position position = new Position(tile.X, tile.Y);

                // Highlight the concrete tile.
                TilemapHighlight.SetTile(
                    position.GetVector3Int(), TileHighlight);
            }
        }
    }

    // Takes an abstract piece and returns the concrete GameObject that
    // it is supossed to represent.
    private GameObject AbstractPieceToConcrete(PlayerPiece piece)
    {
        if (
            piece.TeamColor == Team.White &&
            piece.PieceNumber == PieceNumber.One)
        {
            return WhitePiece1;
        }
        else if (
            piece.TeamColor == Team.White &&
            piece.PieceNumber == PieceNumber.Two)
        {
            return WhitePiece2;
        }
        else if (
            piece.TeamColor == Team.Black &&
            piece.PieceNumber == PieceNumber.One)
        {
            return BlackPiece1;
        }
        else if (
            piece.TeamColor == Team.Black &&
            piece.PieceNumber == PieceNumber.Two)
        {
            return BlackPiece2;
        }
    }

    // Takes an abstract piece and returns the concrete GameObject that
    // it is supossed to represent.
    private GameObject AbstractPieceToConcrete(Ball ball)
    {
        return Ball;
    }

    // Takes an abstract piece (that was recently moved), finds the real
    // piece that it represents and updates its position (of the real one).
    private void MoveConcretePiece(Piece piece)
    {
        // Translate from the abstract piece to the concrete one.
        GameObject realPiece = AbstractPieceToConcrete(piece);
        // Create a new position object (to make a unit conversion).
        Position position = new Position(piece.X, piece.Y);
        // Set the new coordinates for the piece.
        realPiece.transform.position = tilemapBoard.GetCellCenterWorld(
            position.GetVector3Int());
    }
}