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

    // GameObjects for game control
    public Tilemap tilemapBoard;
    public Tilemap tilemapHighlight;
    public Tile tileHighlight;
    public BoxCollider2D boardBoxCollider;
    public GameObject[] playerChips;
    public GameObject ballChip;
    
    // Movement settings
    public float smoothDampTime = .02f;
    public float distanceToSnap = .05f;

    // Variables for game control used in movement validation
    [SerializeField] private int passCount = 0;
    List<Vector3> destinationsPlayer = new List<Vector3>();
    List<Vector3> destinationsBall = new List<Vector3>();
    [SerializeField] private GameObject selectedChip;
    [SerializeField] private GameObject ballChipPasser;
    public Team currentTurn;
    public PlayerStates currentState;

    // UI variables
    public TextMeshProUGUI whiteScoreText, redScoreText;
    public TextMeshProUGUI whiteScoreName, redScoreName;
    public TextMeshProUGUI winnerName;
    public string whiteName, redName;
    public int whiteScore = 0, redScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        whiteName = PlayerPrefs.GetString("player1");
        redName = PlayerPrefs.GetString("player2");
        whiteScoreName.text = whiteName;
        redScoreName.text = redName;
        currentTurn = Team.Red;
        ballChip.GetComponent<ScriptBall>().teamPossession = currentTurn;
        currentState = PlayerStates.WaitingPlayerInputChip;
        boardBoxCollider = tilemapBoard.gameObject.GetComponent<BoxCollider2D>();
        ResetState();
        // Create a new abstract game instance.
        game = New Game(2Players, White);
    }

    IEnumerator MoveChipAndUpdateState(GameObject chip, Vector3 destination)
    {
        // Moves the chip to the destination passed with SmoothDamp (destination MUST be in world units) and then
        // updates the game state.
        while (chip.transform.position != destination)
        {
            Vector2 velocity = Vector2.zero;
            chip.transform.position = Vector2.SmoothDamp(chip.transform.position, destination, ref velocity, smoothDampTime);
            if (Math.Abs(Vector2.Distance(chip.transform.position, destination)) < distanceToSnap)
            {
                chip.transform.position = destination;
            }
            yield return null;
        }
        if (IsBallInGoal())
        {
            // Update score UI and reset variables.
            if (currentTurn == Team.White)
            {
                whiteScore++;
                whiteScoreText.text = whiteScore.ToString();
            }
            else
            {
                redScore++;
                redScoreText.text = redScore.ToString();
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
          }
        }
    }
  

    public void UpdateBoard(PointerEventData eventData)
    {
        // Updates board based on player input given by the
        // IPointerHandlerEvent on the tilemap board GameObject.
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3Int point = tilemapBoard.WorldToCell(worldPoint);

        // Convert the Vector3Int using a Position object.
        Position position = new Position(point);
        Nullable<Move> move = this.game.UserInput(position);
        RenderChanges(move, this.game.board);
    }

    private IEnumerator RenderMoves(Nullable<Move> move, Board board)
    {
        if (move.HasValue)
        {}

        // Render the 
        RenderHighlightingChanges(board);
    }

    private void RenderHighlightingChanges(Board board)
    {
        // Clear all of the highlights on the tilemap.
        tilemapHighlight.ClearAllTiles();

        // Iterate through the abstract tiles and highlight the concrete ones.
        foreach(Tile tile in board.GetIterativeTiles())
        {
            tilemapHighlight.SetTile()
        }
    }
}