using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    public TextMeshProUGUI blackScoreText = default!;
    public TextMeshProUGUI whiteScoreName = default!;
    public TextMeshProUGUI blackScoreName = default!;
    public TextMeshProUGUI winnerName = default!;
    public string WhiteName { get; private set; } = default!;
    public string BlackName { get; private set; } = default!;
    public int WhiteScore { get; set; } = default!;
    public int BlackScore { get; set; } = default!;
    public GameObject whiteBanner;
    public GameObject blackBanner;
    public GameObject popUpBanner;
    public Color defaultBannerColor;
    public GameObject spriteChip1Player1, spriteChip2Player1, spriteChip1Player2, spriteChip2Player2;
    public Image imageChipInScoreOfPlayer1, imageChipInScoreOfPlayer2;
    public Sprite[] chipSprites;

    // Configuration variables
    public bool isHighlightModeOn;

    // Start is called before the first frame update
    void Start()
    {
        WhiteScore = 0;
        BlackScore = 0;
        WhiteName = PlayerPrefs.GetString("player1");
        BlackName = PlayerPrefs.GetString("player2");
        putChipImage();
        this.whiteScoreName.text = WhiteName;
        this.blackScoreName.text = BlackName;

        BoardBoxCollider =
        TilemapBoard.gameObject.GetComponent<BoxCollider2D>();
        isHighlightModeOn = PlayerPrefs.GetInt("ayuda") == 1;
        // Create a new abstract game instance.
        Game = new Game(Team.White);
        GameMode = GameMode.OnePlayer;

        whiteBanner.GetComponent<Image>().color =
        Game.CurrentTurn == Team.White ? Color.white : defaultBannerColor;
        blackBanner.GetComponent<Image>().color =
        Game.CurrentTurn == Team.Black ? Color.white : defaultBannerColor;
    }

    public void putChipImage()
    {
        for (int i = 0; i < chipSprites.Length; i++)
        {
            if (PlayerPrefs.GetString("color1") == chipSprites[i].ToString())
            {
                spriteChip1Player1.GetComponent<SpriteRenderer>().sprite = chipSprites[i];
                spriteChip2Player1.GetComponent<SpriteRenderer>().sprite = chipSprites[i];
                imageChipInScoreOfPlayer1.sprite = chipSprites[i];
            }
            if (PlayerPrefs.GetString("color2") == chipSprites[i].ToString())
            {
                spriteChip1Player2.GetComponent<SpriteRenderer>().sprite = chipSprites[i];
                spriteChip2Player2.GetComponent<SpriteRenderer>().sprite = chipSprites[i];
                imageChipInScoreOfPlayer2.sprite = chipSprites[i];
            }
        }
    }

    public void SetHighlightMode(bool highlightMode)
    {
        isHighlightModeOn = highlightMode;
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
        StartCoroutine(RenderChanges(move));

        // If the game mode is single player, then the computer has
        // to make a move now.
        if (GameMode == GameMode.OnePlayer && Game.CurrentTurn == Team.Black)
        {
            // Create a new AIModule instance. This class encapsulates
            // the recommended moves in an instance field.
            AIModule aiModule = new AIModule(Game);
            // Get the recommended moves (the moves with the highest
            // utility score) and render them in a coroutine.
            StartCoroutine(RenderChanges(aiModule.Moves));
            // Get the child game state and replace the old one.
            Game = aiModule.ChildGame;
        }
    }

    // Coroutine that takes a single move and changes the position of the
    // GameObjects accordingly. If the move resulted in a goal, first it
    // waits little bit, and then resets the game board.
    private IEnumerator RenderChanges(Move? move)
    {
        // Move the piece (not a coroutine).
        MakeMove(move);

        // Check if a goal has been scored.
        if (move != null && move.GetGoal() != null)
        {
            // Render a pop-up with a message announcing the goal.
            if (move.GetGoal() == Team.White)
                yield return MakePopup("Gol de " + WhiteName);
            else
                yield return MakePopup("Gol de " + BlackName);

            // Update the scores.
            UpdateScores();
            // Wait for the player to see the goal before resetting the game.
            yield return new WaitForSeconds(3.0f);
            // Give the signal to reset the abstract game representation.
            Game.ResetGame();
            // Reset the real board, after the abstract board has been reset.
            ResetConcreteBoard();
        }
    }

    // Update the real scores in relation to the scores in the abstract game.
    private void UpdateScores()
    {
        WhiteScore = Game.WhiteScore;
        BlackScore = Game.BlackScore;
    }

    // Coroutine that takes a series of moves (or just one move, or no move)
    // that were made on the game and moves the corresponding concrete piece.
    private IEnumerator RenderChanges(List<Move> moves)
    {
        foreach (Move move in moves)
        {
            // Wait a little bit before rendering the next action.
            yield return new WaitForSeconds(1.0f);
            RenderChanges(move);
        }
    }

    // Takes a move (or no move) that was made on the abstract game
    // representation and moves the corresponding concrete piece.
    private void MakeMove(Move? move)
    {
        if (move != null)
        {
            // Check what kind of piece was recently moved and move
            // it on the concrete board.
            if (move.PlayerPiece != null)
                MoveConcretePlayer(move.PlayerPiece);
            else if (move.BallMoved != null)
                MoveConcreteBall(move.BallMoved);
        }

        // Render the new highlights, if there are any.
        RenderHighlights(Game.Board);
    }

    // Resets the real game board by moving the GameObjects to the same
    // position in which their abstract counterparts are, which should
    // be their initial positions.
    private void ResetConcreteBoard()
    {
        MoveConcretePlayer(Game.Board.WhitePiece1);
        MoveConcretePlayer(Game.Board.WhitePiece2);
        MoveConcretePlayer(Game.Board.BlackPiece1);
        MoveConcretePlayer(Game.Board.BlackPiece2);
        MoveConcreteBall(Game.Board.Ball);
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

    // Creates a pop-up.
    // While the popup is active, players can't pick chips in board.
    private IEnumerator MakePopup(string text)
    {
        BoardBoxCollider.enabled = false;
        popUpBanner.GetComponentInChildren<TextMeshProUGUI>().text = text;
        popUpBanner.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        popUpBanner.SetActive(false);
        BoardBoxCollider.enabled = true;
    }
}