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
    Game game;

    // GameObjects for game control.
    //
    public Tilemap tilemapBoard;
    public Tilemap tilemapHighlight;
    // A PNG file that signals the highlighting.
    public Tile tileHighlight;
    public BoxCollider2D boardBoxCollider;
    // The five game pieces.
    public GameObject whitePiece1;
    public GameObject whitePiece2;
    public GameObject blackPiece1;
    public GameObject blackPiece2;
    public GameObject ball;
    
    // Movement settings
    //public float smoothDampTime = .02f;
    //public float distanceToSnap = .05f;

    // UI variables
    public TextMeshProUGUI whiteScoreText, redScoreText;
    public TextMeshProUGUI whiteScoreName, redScoreName;
    public TextMeshProUGUI winnerName;
    public string whiteName, redName;
    public int whiteScore = 0, redScore = 0;

    // Start is called before the first frame update.
    void Start()
    {
        this.whiteName = PlayerPrefs.GetString("player1");
        this.redName = PlayerPrefs.GetString("player2");
        this.whiteScoreName.text = whiteName;
        this.redScoreName.text = redName;
        this.boardBoxCollider = tilemapBoard.gameObject.GetComponent<BoxCollider2D>();
        // Create a new abstract game instance.
        this.game = new Game(TwoPlayers, White);
    }

    // This method is called whenever the user taps on the screen.
    public void UpdateBoard(PointerEventData eventData)
    {
        // Updates board based on player input given by the
        // IPointerHandlerEvent on the tilemap board GameObject.
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3Int point = tilemapBoard.WorldToCell(worldPoint);

        // Convert the Vector3Int to matrix coordinates using
        // the Position object.
        Position position = new Position(point);
        Move? move = this.game.UserInput(position);

        // Render the changes that resulted from this interaction with
        // the user.
        RenderChanges(move, this.game.board);
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
                MoveConcretePiece(this.game.board.whitePiece1);
                MoveConcretePiece(this.game.board.whitePiece2);
                MoveConcretePiece(this.game.board.blackPiece1);
                MoveConcretePiece(this.game.board.blackPiece2);
                MoveConcretePiece(this.game.board.ball);
            }
        }

        // Render the new highlights, if there are any.
        RenderHighlights(this.game.board);
    }

    // Takes the abstract board as argument and checks for the tiles that
    // are highlighted. It "translates" those hightlights to the concrete
    // game board.
    private void RenderHighlights(Board board)
    {
        // Clear all of the highlights on the tilemap.
        tilemapHighlight.ClearAllTiles();

        // Iterate through the abstract tiles and highlight the concrete ones.
        foreach(Tile tile in board.GetIterativeTiles())
        {
            // Check if the tile is valid and if it's highlighted.
            if (tile.IsTileValid() && tile.isHighlighted())
            {
                // Create a new position object (to make a unit conversion).
                Position position = new Position(tile.GetX(), tile.GetY());

                // Highlight the concrete tile.
                tilemapHighlight.SetTile(
                    position.GetVector3Int(), this.tileHighlight);
            }
        }
    }

    // Takes an abstract piece and returns the concrete GameObject that
    // it is supossed to represent.
    private GameObject AbstractPieceToConcrete(PlayerPiece piece)
    {
        if (piece.teamColor == White && piece.pieceNumber == One)
        {
            return whitePiece1;
        }
        else if (piece.teamColor == White && piece.pieceNumber == Two)
        {
            return whitePiece2;
        }
        else if (piece.teamColor == Black && piece.pieceNumber == One)
        {
            return blackPiece1;
        }
        else if (piece.teamColor == Black && piece.pieceNumber == Two)
        {
            return blackPiece2;
        }
    }

    // Takes an abstract piece and returns the concrete GameObject that
    // it is supossed to represent.
    private GameObject AbstractPieceToConcrete(Ball ball)
    {
        return ballPiece;
    }

    // Takes an abstract piece (that was recently moved), finds the real
    // piece that it represents and updates its position (of the real one).
    private void MoveConcretePiece(Piece piece)
    {
        // Translate from the abstract piece to the concrete one.
        GameObject realPiece = AbstractPieceToConcrete(piece);
        // Create a new position object (to make a unit conversion).
        Position position = new Position(piece.GetX(), piece.GetY());
        // Set the new coordinates for the piece.
        realPiece.transform.position = tilemapBoard.GetCellCenterWorld(
            position.GetVector3Int());
    }
}