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
<<<<<<< HEAD
    public Game Game { get; set; } = default!;
    public GameMode GameMode { get; private set; }
=======
    AbstractGameBoard gameState;
>>>>>>> a79abc05739dc0b8a020d117a71b2274c279f6ff

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
<<<<<<< HEAD
    public TextMeshProUGUI whiteScoreText = default!;
    public TextMeshProUGUI blackScoreText = default!;
    public TextMeshProUGUI whiteScoreName = default!;
    public TextMeshProUGUI blackScoreName = default!;
    public TextMeshProUGUI winnerName = default!;
    public string WhiteName { get; private set; } = default!;
    public string BlackName { get; private set; } = default!;
    //public int WhiteScore { get; set; } = default!;
    //public int BlackScore { get; set; } = default!;
    public GameObject whiteBanner = default!;
    public GameObject blackBanner = default!;
    public GameObject popUpBanner;
    public Color defaultBannerColor;
=======
    public TextMeshProUGUI whiteScoreText, redScoreText;
    public TextMeshProUGUI whiteScoreName, redScoreName;
    public TextMeshProUGUI winnerName;
    public GameObject whiteBanner, redBanner;
    public GameObject popupBanner;
    public Color defaultBannerColor;
    public string whiteName, redName;
    public int whiteScore = 0, redScore = 0;
>>>>>>> a79abc05739dc0b8a020d117a71b2274c279f6ff
    public GameObject spriteChip1Player1, spriteChip2Player1, spriteChip1Player2, spriteChip2Player2;
    public Image imageChipInScoreOfPlayer1, imageChipInScoreOfPlayer2;
    public Sprite[] chipSprites;

    // Configuration variables
    public bool isHighlightModeOn;
<<<<<<< HEAD

    // Start is called before the first frame update
    void Start()
    {
        //WhiteScore = 0;
        //BlackScore = 0;
        WhiteName = PlayerPrefs.GetString("player1");
        BlackName = PlayerPrefs.GetString("player2");

        PutChipImage();

        this.whiteScoreName.text = WhiteName;
        this.blackScoreName.text = BlackName;

        BoardBoxCollider =
        TilemapBoard.gameObject.GetComponent<BoxCollider2D>();

        isHighlightModeOn = PlayerPrefs.GetInt("ayuda") == 1;

        // Create a new abstract game instance.
        Game = new Game(Team.White);
        GameMode = GameMode.TwoPlayers;

        whiteBanner.GetComponent<Image>().color = Game.CurrentTurn == Team.White ? Color.white : Color.black;

        blackBanner.GetComponent<Image>().color = Game.CurrentTurn == Team.Black ? Color.black : Color.white;
=======
    /*
    void Awake()
    {
        // Getting the GameState script from the AbstractGameBoard GameObject.
        gameState = abstractGameBoard.GetComponent<AbstractGameBoard>();
    }*/
    
    // Start is called before the first frame update
    void Start()
    {
        whiteName = PlayerPrefs.GetString("player2");
        redName = PlayerPrefs.GetString("player1");
        putChipImage();
        whiteScoreName.text = whiteName;
        redScoreName.text = redName;
        currentTurn = Team.White;
        ballChip.GetComponent<ScriptBall>().teamPossession = currentTurn;
        currentState = PlayerStates.WaitingPlayerInputChip;
        boardBoxCollider = tilemapBoard.gameObject.GetComponent<BoxCollider2D>();
        ResetState();
        whiteBanner.GetComponent<Image>().color = currentTurn == Team.White ? Color.white : defaultBannerColor;
        redBanner.GetComponent<Image>().color = currentTurn == Team.Red ? Color.white : defaultBannerColor;
        isHighlightModeOn = PlayerPrefs.GetInt("ayuda") == 1;
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
>>>>>>> a79abc05739dc0b8a020d117a71b2274c279f6ff
    }

    public void PutChipImage()
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
<<<<<<< HEAD
                spriteChip1Player2.GetComponent<SpriteRenderer>().sprite = chipSprites[i];
                spriteChip2Player2.GetComponent<SpriteRenderer>().sprite = chipSprites[i];
                imageChipInScoreOfPlayer2.sprite = chipSprites[i];
=======
                whiteScore++;
                whiteScoreText.text = whiteScore.ToString();
                yield return MakePopup("Gol de " + whiteName);
            }
            else
            {
                redScore++;
                redScoreText.text = redScore.ToString();
                yield return MakePopup("Gol de " + redName);
            }
            if (redScore >= 2 || whiteScore >= 2)
            {
                string winner = currentTurn == Team.White ? whiteName : redName;
                winnerName.text = "Gano " + winner;
                boardBoxCollider.enabled = false;
            }
            ResetState();
            yield return null;
        }
        else
        {
            // When move is done, check if more moves are available (passes)
            if (IsBallPassable(chip))
            {
                passCount++;
                currentState = PlayerStates.WaitingPlayerInputBallDestination;
                yield return StartCoroutine(CalculateMovesBallChip(destinationsBall));
            }
            else
            {
                passCount = 0;
                currentTurn = currentTurn == Team.White ? Team.Red : Team.White;
                ballChip.GetComponent<ScriptBall>().teamPossession = currentTurn;
                currentState = PlayerStates.WaitingPlayerInputChip;
                UpdateBannerColors();
>>>>>>> a79abc05739dc0b8a020d117a71b2274c279f6ff
            }
        }
    }

    // Disables the visual highlighting when a player piece is selected
    // and when the ball needs to be moved. The highlighting on the
    // abstract game will always occur.
    public void SetHighlightMode(bool highlightMode)
    {
<<<<<<< HEAD
        isHighlightModeOn = highlightMode;
=======
        /// Resets chips positions, changes turn and resets pass count. Used when someone scores a goal.
        currentTurn = currentTurn == Team.White ? Team.Red : Team.White;
        currentState = PlayerStates.WaitingPlayerInputChip;
        ballChip.transform.position = new Vector3(0, 0, 0);
        playerChips[0].transform.position = new Vector3(0, -5, 0);
        playerChips[1].transform.position = new Vector3(0, -3, 0);
        playerChips[2].transform.position = new Vector3(0, 5, 0);
        playerChips[3].transform.position = new Vector3(0, 3, 0);
        passCount = 0;
        UpdateBannerColors();
    }

    private IEnumerator MakePopup(string text)
    {
        /// Creates a pop up with text as text.
        /// While the popup is active, players can't pick chips in board.
        boardBoxCollider.enabled = false;
        popupBanner.GetComponentInChildren<TextMeshProUGUI>().text = text;
        popupBanner.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        popupBanner.SetActive(false);
        boardBoxCollider.enabled = true;
    }

    private void UpdateBannerColors()
    {
        /// Updates the banners colors to indicate whose turn is it.
        whiteBanner.GetComponent<Image>().color = currentTurn == Team.White ? Color.white : defaultBannerColor;
        redBanner.GetComponent<Image>().color = currentTurn == Team.Red ? Color.white : defaultBannerColor;
    }

    private bool IsBallInGoal()
    {
        return Array.IndexOf(whiteGoals, ballChip.transform.position) > -1 || Array.IndexOf(redGoals, ballChip.transform.position) > -1;
    }

    private bool IsFieldValidForPlayerChip(GameObject playerChip, Vector3 destinationCenter)
    {
        /// Checks and returns if field at destinationCenter is valid for player chip movement.
        // TODO: This probably can be refactored using IsPlayerChipInField and IsBallChipInField
        // Is there a chip in destinationCenter?
        foreach (var chip in playerChips)
        {
            if (chip.transform.position == destinationCenter)
            {
                return false;
            }
        }
        // Is the ball at the destination?
        if (destinationCenter == ballChip.transform.position)
        {
            return false;
        }
        // Is the field out of bounds?
        if (Math.Abs(destinationCenter.x) > 5 || Math.Abs(destinationCenter.y) > 6)
        {
            return false;
        }
        if (!IsFieldAdyacentToNeutralFieldValid(playerChip, destinationCenter))
        {
            return false;
        }
        return true;
    }

    private bool IsFieldAdyacentToNeutralFieldValid(GameObject playerChip, Vector3 point)
    {
        /// Checks if field at point is a neutral space and gets all the chips for that
        /// space, if the field is adyacent to a neutral field and playerChip is part of it, return true.
        /// Else, return false.
        int whiteCount = 0, redCount = 0;
        List<GameObject> chipsInNeutralField = new List<GameObject>();
        List<Vector3> pointsInNeutralField = new List<Vector3>();
        foreach (var dir in playerDirections)
        {
            pointsInNeutralField.Add(ballChip.transform.position + dir);
            foreach (var chip in playerChips)
            {
                if (ballChip.transform.position + dir == chip.transform.position)
                {
                    if (chip.GetComponent<ScriptChip>().team == Team.White)
                    {
                        whiteCount++;
                    }
                    else
                    {
                        redCount++;
                    }
                    chipsInNeutralField.Add(chip);
                    break;
                }
            }
        }
        // Ball is in a neutral field but field isn't, therefore field is not valid.
        if (chipsInNeutralField.Contains(playerChip) && !pointsInNeutralField.Contains(point))
        {
            return false;
        }
        // Ball is in a neutral field but field is, therefore field is valid.
        if (chipsInNeutralField.Contains(playerChip) && pointsInNeutralField.Contains(point))
        {
            return true;
        }
        return true;
    }

    private void CalculateMovesPlayer(GameObject playerChip, Vector3Int point, List<Vector3> destinations)
    {
        /// Calculates all possible fields the player can move to and writes the positions to destinations.
        for (int i = 1; i <= 2; i++)
        {
            foreach(Vector3Int direction in playerDirections)
            {
                Vector3Int destination = point + direction * i;
                Vector3 destinationCenter = tilemapBoard.GetCellCenterWorld(destination);
                if (IsFieldValidForPlayerChip(playerChip, destinationCenter))
                {
                    if (isHighlightModeOn)
                    {
                        tilemapHighlight.SetTile(destination, tileHighlight);
                    }
                    destinations.Add(destinationCenter);
                }
            }
        }
    }

    private GameObject GetChipInField(Vector3 point)
    {
        foreach (var chip in playerChips)
        {
            Debug.Log("chip: " + chip.transform.position);
            if (chip.transform.position == point)
            {
                return chip;
            }
        }
        return null;
    }

    // Checker functions, these are mostly used in CalculateMovesBallChip()
    private bool IsPlayerChipInField(Vector3 point)
    {
        /// Gets a point and returns true if there is a chip in that space (player or ball), else returns false.
        foreach(var chip in playerChips)
        {
            if (chip.transform.position == point)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsBallChipInField(Vector3 point)
    {
        /// Gets a point and returns true if the ball is at that point.
        if (point == ballChip.transform.position)
        {
            return true;
        }
        return false;
    }

    private bool IsFieldOutOfBounds(Vector3 point)
    {
        /// Gets a point and returns true if the point is out of the bounds of the board.
        if (Math.Abs(point.x) >= 6)
        {
            return true;
        }
        if (Math.Abs(point.y) >= 8)
        {
            return true;
        }
        if (Math.Abs(point.y) == 7 && Math.Abs(point.x) >= 3)
        {
            return true;
        }
        return false;
    }

    private bool IsFieldAValidGoal(Vector3 point)
    {
        /// Gets a point and returns true if it's a valid goal for the current player playing.
        if (currentTurn == Team.White && Array.IndexOf(redGoals, point) > -1)
        {
            return true;
        }
        if (currentTurn == Team.Red && Array.IndexOf(whiteGoals, point) > -1)
        {
            return true;
        }
        return false;
    }

    private bool IsFieldAValidCorner(Vector3 point)
    {
        /// Gets a point and returns true if it's a valid corner for the current player playing.
        if (currentTurn == Team.White && Array.IndexOf(redCorners, point) > -1)
        {
            return true;
        }
        if (currentTurn == Team.Red && Array.IndexOf(whiteCorners, point) > -1)
        {
            return true;
        }
        return false;
    }

    private bool IsBallPassable(GameObject potentialPasser = null)
    {
        /// Checks if it's possible to pass the ball from its position by checking for player chips in adyacent
        /// fields, if found, counts them to check for neutral spaces.
        /// If no neutral spaces were found and a pass is possible for the team whose turn it is and the pass limit hasn't been passed, return true.
        int redCount = 0, whiteCount = 0;
        bool passerTagged = false;
        foreach (var dir in playerDirections)
        {
            foreach (var chip in playerChips)
            {
                if (ballChip.transform.position + dir == chip.transform.position)
                {
                    if (chip.GetComponent<ScriptChip>().team == Team.White)
                    {
                        whiteCount++;
                        if (currentTurn == Team.White)
                        {
                            if (potentialPasser == chip)
                            {
                                ballChipPasser = chip;
                            }
                            else if (potentialPasser == ballChip && ballChipPasser != chip && !passerTagged)
                            {
                                passerTagged = true;
                                ballChipPasser = chip;
                            }
                        }
                    }
                    else
                    {
                        redCount++;
                        if (currentTurn == Team.Red)
                        {
                            if (potentialPasser == chip)
                            {
                                ballChipPasser = chip;
                            }
                            else if (potentialPasser == ballChip && ballChipPasser != chip && !passerTagged)
                            {
                                passerTagged = true;
                                ballChipPasser = chip;
                            }
                        }
                    }
                    break;
                }
            }
        }
        return redCount != whiteCount && 
            ((redCount > whiteCount && currentTurn == Team.Red) || (whiteCount > redCount && currentTurn == Team.White)) &&
            passCount < 4;
    }

    private bool AreAdyacentFieldsValid(Vector3 point)
    {
        /// Checks if a field at point is a valid destination for the ball by checking the fields around it.
        /// If so, returns true.
        // TODO: Test the fuck out of this because the rules make me want to kms.
        GameObject passerCandidate = null;
        int redCount = 0, whiteCount = 0;
        foreach (var dir in playerDirections)
        {
            foreach (var chip in playerChips)
            {
                if (point + dir == chip.transform.position)
                {
                    if (chip.GetComponent<ScriptChip>().team == Team.White)
                    {
                        whiteCount++;
                        if (currentTurn == Team.White)
                        {
                            if (ballChipPasser != chip)
                            {
                                passerCandidate = chip;
                            }
                        }
                    }
                    else
                    {
                        redCount++;
                        if (currentTurn == Team.Red)
                        {
                            if (ballChipPasser != chip)
                            {
                                passerCandidate = chip;
                            }
                        }
                    }
                    break;
                }
            }
        }
        // Players can kick to neutral spaces, but the players around it can only move around that field.
        if (redCount == whiteCount)
        {
            // Players can only kick the ball to their area if there's a player there to kick elsewhere.
            if (currentTurn == Team.White && Array.IndexOf(whiteArea, point) > -1)
            {
                return false;
            }
            if (currentTurn == Team.Red && Array.IndexOf(redArea, point) > -1)
            {
                return false;
            }
            return true;
        }
        // If it's red's turn and there's more red chips than white ones, check for passer because a chip can't pass to itself.
        if (redCount > whiteCount && currentTurn == Team.Red && passCount <= 3)
        {
            if (passerCandidate != null)
            {
                return true;
            }
            return false;
        }
        // Do the same for white chips.
        if (whiteCount > redCount && currentTurn == Team.White && passCount <= 3)
        {
            if (passerCandidate != null)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    private bool IsFieldValidForBallChip(Vector3 destinationCenter)
    {
        /// Makes all the rule checks for the field at destinationCenter, returns true if the field is a valid destination.
        // TODO: Literally add the rest of the rules, corner checks, player checks, player area checks.
        if (IsPlayerChipInField(destinationCenter))
        {
            return false;
        }
        if (IsBallChipInField(destinationCenter))
        {
            return false;
        }
        if (IsFieldOutOfBounds(destinationCenter))
        {
            return false;
        }
        if (Array.IndexOf(whiteCorners, destinationCenter) > -1 || Array.IndexOf(redCorners, destinationCenter) > -1)
        {
            return IsFieldAValidCorner(destinationCenter);
        }
        if (Array.IndexOf(whiteGoals, destinationCenter) > -1 || Array.IndexOf(redGoals, destinationCenter) > -1)
        {
            return IsFieldAValidGoal(destinationCenter);
        }
        if (!AreAdyacentFieldsValid(destinationCenter))
        {
            return false;
        }
        return true;
    }

    IEnumerator CalculateMovesBallChip(List<Vector3> destinations)
    {
        /// Calculates all possible fields the ball can move to and writes the positions to destinations.
        var point = tilemapBoard.WorldToCell(ballChip.transform.position);
        
        for (int i = 1; i <= 4; i++)
        {
            foreach (Vector3Int direction in playerDirections)
            {
                Vector3Int destination = point + direction * i;
                Vector3 destinationCenter = tilemapBoard.GetCellCenterWorld(destination);
                if (IsFieldValidForBallChip(destinationCenter))
                {
                    if (isHighlightModeOn)
                    {
                        tilemapHighlight.SetTile(destination, tileHighlight);
                    }
                    destinations.Add(destinationCenter);
                }
            }
        }
        yield return null;
>>>>>>> a79abc05739dc0b8a020d117a71b2274c279f6ff
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
<<<<<<< HEAD
            // Create a new AIModule instance. This class encapsulates
            // the recommended moves in an instance field.
            AIModule aiModule = new AIModule(Game);
            // Get the recommended moves (the moves with the highest
            // utility score) and render them in a coroutine.
            StartCoroutine(RenderChanges(aiModule.Moves));
            // Get the child game state and replace the old one.
            Game = aiModule.ChildGame;
=======
            var pointCenter = tilemapBoard.GetCellCenterWorld(point);

            switch (currentState) {
                case PlayerStates.WaitingPlayerInputChip:
                    // Did the player choose a field with a chip?
                    if (IsPlayerChipInField(pointCenter))
                    {
                        selectedChip = GetChipInField(pointCenter);
                        if (selectedChip.GetComponent<ScriptChip>().team == currentTurn)
                        {
                            currentState = PlayerStates.WaitingPlayerInputChipDestination;
                            CalculateMovesPlayer(selectedChip, point, destinationsPlayer);
                        }
                    }
                    break;

                case PlayerStates.WaitingPlayerInputChipDestination:
                    // Did the player choose a valid destination?
                    if (destinationsPlayer.Contains(pointCenter)) 
                    {
                        StartCoroutine(MoveChipAndUpdateState(selectedChip, pointCenter));
                        tilemapHighlight.ClearAllTiles();
                        destinationsPlayer.Clear();
                    }
                    // If the player chose the same chip, undo the selection and let them tap chips again.
                    else if (selectedChip == GetChipInField(pointCenter))
                    {
                        destinationsPlayer.Clear();
                        tilemapHighlight.ClearAllTiles();
                        currentState = PlayerStates.WaitingPlayerInputChip;
                    }
                    break;

                case PlayerStates.WaitingPlayerInputBallDestination:
                    // Did the player choose a valid destination?
                    if (destinationsBall.Contains(pointCenter))
                    {
                        StartCoroutine(MoveChipAndUpdateState(ballChip, pointCenter));
                        tilemapHighlight.ClearAllTiles();
                        destinationsBall.Clear();
                    }
                    break;
            }
>>>>>>> a79abc05739dc0b8a020d117a71b2274c279f6ff
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
            // Update the scores.
            UpdateScores();

            // Render a pop-up with a message announcing the goal.
            if (move.GetGoal() == Team.White)
                yield return MakePopup("Gol de " + WhiteName);
            else
                yield return MakePopup("Gol de " + BlackName);

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
        whiteScoreText.text = Game.WhiteScore.ToString();
        blackScoreText.text = Game.BlackScore.ToString();
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