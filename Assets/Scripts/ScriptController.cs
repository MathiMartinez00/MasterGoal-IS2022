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
    private Difficulty Difficulty { get; set; }

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
    public TextMeshProUGUI winnerText = default!;
    public string WhiteName { get; private set; } = default!;
    public string BlackName { get; private set; } = default!;
    public GameObject whiteBanner = default!;
    public GameObject blackBanner = default!;
    public GameObject popUpBanner = default!;
    public Color defaultBannerColor = default!;
    public GameObject spriteChip1Player1 = default!;
    public GameObject spriteChip2Player1 = default!;
    public GameObject spriteChip1Player2 = default!;
    public GameObject spriteChip2Player2 = default!;
    public Image imageChipInScoreOfPlayer1 = default!;
    public Image imageChipInScoreOfPlayer2 = default!;
    public Sprite[] chipSprites = default!;

    // Configuration variables
    public bool isHighlightModeOn = true;

    // Start is called before the first frame update
    void Start()
    {
        // Assign the names to the players.
        WhiteName = PlayerPrefs.GetString("player1");
        BlackName = PlayerPrefs.GetString("player2");
        this.whiteScoreName.text = WhiteName;
        this.blackScoreName.text = BlackName;

        // Assign the images to the pieces.
        PutChipImage();

        BoardBoxCollider =
        TilemapBoard.gameObject.GetComponent<BoxCollider2D>();

        // Set the tile highlights on or off.
        isHighlightModeOn = PlayerPrefs.GetInt("ayuda") == 1;

        // Set the game mode that was selected by the user.
        // "modo" == 0 -> PC vs player ; "modo" == 1 -> player vs player.
        GameMode = PlayerPrefs.GetInt("gameMode") == 0 ?
        GameMode.OnePlayer : GameMode.TwoPlayers;

        // There is no way to change the difficulty yet.
        Difficulty = Difficulty.Easy;

        // Create a new abstract game instance.
        Game = new Game(Team.White);

        whiteBanner.GetComponent<Image>().color =
        Game.CurrentTurn == Team.White ? Color.white : defaultBannerColor;
        blackBanner.GetComponent<Image>().color =
        Game.CurrentTurn == Team.Black ? Color.white : defaultBannerColor;
    }

    // Assign the correct image to the player pieces, according to the
    // selection of the user in the previous menu.
    public void PutChipImage()
    {
        for (int i = 0; i < chipSprites.Length; i++)
        {
            if (PlayerPrefs.GetString("color1") == chipSprites[i].ToString())
            {
                spriteChip1Player1.GetComponent<SpriteRenderer>().sprite =
                chipSprites[i];
                spriteChip2Player1.GetComponent<SpriteRenderer>().sprite =
                chipSprites[i];
                imageChipInScoreOfPlayer1.sprite = chipSprites[i];
            }
            if (PlayerPrefs.GetString("color2") == chipSprites[i].ToString())
            {
                spriteChip1Player2.GetComponent<SpriteRenderer>().sprite =
                chipSprites[i];
                spriteChip2Player2.GetComponent<SpriteRenderer>().sprite =
                chipSprites[i];
                imageChipInScoreOfPlayer2.sprite = chipSprites[i];
            }
        }
    }

    // Turns recommendations on or off.
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

        // If the game is over, no more input is accepted.
        if (Game.GameStatus != GameStatus.GameOver)
            // Do the user's move and the computer's moves, if necessary.
            StartCoroutine(MakeHumanAndComputerMove(position));
    }

    // Concatenates the coroutines of the user and the computer's move.
    private IEnumerator MakeHumanAndComputerMove(Position position)
    {
        // User input.
        Move? move = Game.Input(position);

        yield return StartCoroutine(RenderChanges(move));

        // Check if a goal has been scored.
        if (move != null && move.IsGoal)
        {
            // Reset the pieces after a goal.
            yield return StartCoroutine(ResetAbstractAndRealGame());

            // If the player scored a goal, the computer has to player next.
            yield return StartCoroutine(MakeComputerMove());
        }
        // Computer's turn, if relevant.
        else if (
            GameMode == GameMode.OnePlayer &&
            // The computer is always black.
            Game.CurrentTurn == Team.Black &&
            // If the ball is inside of the goal, don't take input.
            Game.CheckForGoalScored() == null)
        {
            yield return StartCoroutine(MakeComputerMove());
        }
    }

    // Resets the real game board and the abstract game board.
    private IEnumerator ResetAbstractAndRealGame()
    {
        // Give the signal to reset to the abstract game representation.
        Game.ResetGame();
        // Reset the real board, after the abstract board has been reset.
        ResetConcreteBoard();

        yield return null;
    }

    // Performs the moves of the computer.
    private IEnumerator MakeComputerMove()
    {
        // Create a new AIModule instance. This class encapsulates
        // the recommended moves in an instance field.
        AIModule aiModule = new AIModule(Game, Difficulty);

        // Get the child game state and replace the old one.
        Game = aiModule.ChildGame;

        // Get the recommended moves (the moves with the highest
        // utility score) and render them in a coroutine.
        yield return StartCoroutine(RenderChanges(aiModule.Moves));

        // If the computer scored a goal and it's not game over, then
        // reset the board.
        if (
            Game.CheckForGoalScored() != null &&
            Game.GameStatus != GameStatus.GameOver)
        {
            yield return StartCoroutine(ResetAbstractAndRealGame());
        }
    }

    // Coroutine that takes a single move and changes the position of the
    // GameObjects accordingly. If the move resulted in a goal, first it
    // waits little bit, and then resets the game board.
    private IEnumerator RenderChanges(Move? move)
    {
        // Disable user input while the pieces are moving.
        BoardBoxCollider.enabled = false;

        // Move the piece (not a coroutine).
        MakeMove(move);

        // Check if a goal has been scored.
        if (move != null && move.IsGoal)
        {
            // Update the scores.
            UpdateScores();

            // Checks if the game is over to render to pop-up.
            if (Game.GameStatus == GameStatus.GameOver)
            {
                // Render the pop-up announcing the winner of the match.
                if (move.GetGoal() == Team.White)
                    yield return PopUpGameOver("¡Ganó " + WhiteName + "!");
                else
                    yield return PopUpGameOver("¡Ganó " + BlackName + "!");
            }
            else
            {
                // Render a pop-up with a message announcing the goal.
                if (move.GetGoal() == Team.White)
                    yield return PopUpWithTimer(
                        "¡Gol a favor de " + WhiteName + "!", 2.5f);
                else
                    yield return PopUpWithTimer(
                        "¡Gol a favor de " + BlackName + "!", 2.5f);

                // Wait for the player to see the goal before resetting the game.
                yield return new WaitForSeconds(3.0f);
            }
        }

        // Enable user input.
        BoardBoxCollider.enabled = true;
    }

    // Update the real scores in relation to the scores in the abstract game.
    private void UpdateScores()
    {
        whiteScoreText.text = Game.WhiteScore.ToString();
        blackScoreText.text = Game.BlackScore.ToString();
    }

    // Coroutine that takes a series of moves (or just one move, or no move)
    // that were made on the game and moves the corresponding concrete piece.
    private IEnumerator RenderChanges(List<Move> moves)
    {
        foreach (Move move in moves)
        {
            // Disable user input while the pieces are moving.
            BoardBoxCollider.enabled = false;

            // Wait a little bit before rendering the next action.
            yield return new WaitForSeconds(1.2f);

            // Render this single move (overloaded method).
            yield return RenderChanges(move);
        }

        // Enable user input.
        BoardBoxCollider.enabled = true;
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
            {
                MoveConcretePlayer(move.PlayerPiece, move.Destination);

                // Play the player move sound.
                FindObjectOfType<AudioManager>().PlaySound("Player move");
            }
            else if (move.BallMoved != null)
            {
                MoveConcreteBall(move.Destination);

                // Play the ball move sound.
                FindObjectOfType<AudioManager>().PlaySound("Ball move");
            }
        }

        // If highlight mode is on, we render the highlights.
        if (isHighlightModeOn)
            RenderHighlights(Game.Board);
    }

    // Resets the real game board by moving the GameObjects to the same
    // position in which their abstract counterparts are, which should
    // be their initial positions.
    private void ResetConcreteBoard()
    {
        // Get the abstract pieces.
        PlayerPiece white1 = Game.Board.WhitePiece1;
        PlayerPiece white2 = Game.Board.WhitePiece2;
        PlayerPiece black1 = Game.Board.BlackPiece1;
        PlayerPiece black2 = Game.Board.BlackPiece2;

        // Get the coordinates of the initial position of every piece.
        int white1Y = Game.Board.white1Y;
        int white2Y = Game.Board.white2Y;
        int black1Y = Game.Board.black1Y;
        int black2Y = Game.Board.black2Y;
        int ballY = Game.Board.ballY;
        int piecesX = Game.Board.piecesX;

        // Move the real pieces (the GameObjects).
        MoveConcretePlayer(white1, Game.Board.GetTile(piecesX, white1Y));
        MoveConcretePlayer(white2, Game.Board.GetTile(piecesX, white2Y));
        MoveConcretePlayer(black1, Game.Board.GetTile(piecesX, black1Y));
        MoveConcretePlayer(black2, Game.Board.GetTile(piecesX, black2Y));
        MoveConcreteBall(Game.Board.GetTile(piecesX, ballY));
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
    // piece that it represents and updates its position based on the
    // position of the tile passed as argument.
    private void MoveConcretePlayer(
        PlayerPiece playerPiece, AbstractTile tile)
    {
        // Translate from the abstract player piece to the concrete one.
        GameObject realPiece = AbstractPlayerToConcrete(playerPiece);

        // Create a new position object (to make a unit conversion).
        Position position = new Position(tile.X, tile.Y);

        // Set the new coordinates for the piece.
        realPiece.transform.position = TilemapBoard.GetCellCenterWorld(
            position.Vector3Int);
    }

    // Moves the GameObject ball to the position of the tile that is
    // passed as the argument.
    private void MoveConcreteBall(AbstractTile tile)
    {
        // Create a new position object (to make a unit conversion).
        Position position = new Position(tile.X, tile.Y);

        // Set the new coordinates for the ball.
        Ball.transform.position = TilemapBoard.GetCellCenterWorld(
            position.Vector3Int);
    }

    // Creates a pop-up. While the popup is active, players can't select
    // pieces in the board.
    private IEnumerator PopUpWithTimer(string text, float time)
    {
        // Disable user input while the pop-up is active.
        BoardBoxCollider.enabled = false;

        popUpBanner.GetComponentInChildren<TextMeshProUGUI>().text = text;
        popUpBanner.SetActive(true);
        yield return new WaitForSeconds(time);
        popUpBanner.SetActive(false);

        // Enable user input.
        BoardBoxCollider.enabled = true;
    }

    // Creates a pop-up message for when the game is over. This pop-up
    // will remain active, but it will let you tap on the screen.
    private IEnumerator PopUpGameOver(string text)
    {
        popUpBanner.GetComponentInChildren<TextMeshProUGUI>().text = text;
        popUpBanner.SetActive(true);
        yield return new WaitForSeconds(float.MaxValue);
        popUpBanner.SetActive(false);
    }
}