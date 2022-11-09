using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public enum Team
{
    White,
    Red,
}

public class ScriptController : MonoBehaviour
{
    // GameObjects for game control
    public Tilemap tilemapBoard;
    public Tilemap tilemapHighlight;
    public Tile tileHighlight;
    public BoxCollider2D boardBoxCollider;
    public GameObject[] playerChips;
    public GameObject ballChip;

    // Special tile coordinates
    // TODO: Look up list comprehension?
    private readonly Vector3[] whiteGoals =
    {
        new Vector3(-2, 7, 0),
        new Vector3(-1, 7, 0),
        new Vector3(0, 7, 0),
        new Vector3(1, 7, 0),
        new Vector3(2, 7, 0),
    };
    private readonly Vector3[] redGoals =
    {
        new Vector3(-2, -7, 0),
        new Vector3(-1, -7, 0),
        new Vector3(0, -7, 0),
        new Vector3(1, -7, 0),
        new Vector3(2, -7, 0),
    };
    private readonly Vector3[] redCorners =
    {
        new Vector3(-5, -6, 0),
        new Vector3(5, -6, 0),
    };
    private readonly Vector3[] whiteCorners =
    {
        new Vector3(-5, 6, 0),
        new Vector3(5, 6, 0),
    };
    private readonly Vector3[] whiteArea =
    {
        new Vector3(-4, 6, 0),
        new Vector3(-3, 6, 0),
        new Vector3(-2, 6, 0),
        new Vector3(-1, 6, 0),
        new Vector3(0, 6, 0),
        new Vector3(1, 6, 0),
        new Vector3(2, 6, 0),
        new Vector3(3, 6, 0),
        new Vector3(4, 6, 0),
        new Vector3(-4, 5, 0),
        new Vector3(-3, 5, 0),
        new Vector3(-2, 5, 0),
        new Vector3(-1, 5, 0),
        new Vector3(0, 5, 0),
        new Vector3(1, 5, 0),
        new Vector3(2, 5, 0),
        new Vector3(3, 5, 0),
        new Vector3(4, 5, 0),
        new Vector3(-4, 4, 0),
        new Vector3(-3, 4, 0),
        new Vector3(-2, 4, 0),
        new Vector3(-1, 4, 0),
        new Vector3(0, 4, 0),
        new Vector3(1, 4, 0),
        new Vector3(2, 4, 0),
        new Vector3(3, 4, 0),
        new Vector3(4, 4, 0),
        new Vector3(-4, 3, 0),
        new Vector3(-3, 3, 0),
        new Vector3(-2, 3, 0),
        new Vector3(-1, 3, 0),
        new Vector3(0, 3, 0),
        new Vector3(1, 3, 0),
        new Vector3(2, 3, 0),
        new Vector3(3, 3, 0),
        new Vector3(4, 3, 0),
    };
    private readonly Vector3[] redArea =
    {
        new Vector3(-4, -6, 0),
        new Vector3(-3, -6, 0),
        new Vector3(-2, -6, 0),
        new Vector3(-1, -6, 0),
        new Vector3(0, -6, 0),
        new Vector3(1, -6, 0),
        new Vector3(2, -6, 0),
        new Vector3(3, -6, 0),
        new Vector3(4, -6, 0),
        new Vector3(-4, -5, 0),
        new Vector3(-3, -5, 0),
        new Vector3(-2, -5, 0),
        new Vector3(-1, -5, 0),
        new Vector3(0, -5, 0),
        new Vector3(1, -5, 0),
        new Vector3(2, -5, 0),
        new Vector3(3, -5, 0),
        new Vector3(4, -5, 0),
        new Vector3(-4, -4, 0),
        new Vector3(-3, -4, 0),
        new Vector3(-2, -4, 0),
        new Vector3(-1, -4, 0),
        new Vector3(0, -4, 0),
        new Vector3(1, -4, 0),
        new Vector3(2, -4, 0),
        new Vector3(3, -4, 0),
        new Vector3(4, -4, 0),
        new Vector3(-4, -3, 0),
        new Vector3(-3, -3, 0),
        new Vector3(-2, -3, 0),
        new Vector3(-1, -3, 0),
        new Vector3(0, -3, 0),
        new Vector3(1, -3, 0),
        new Vector3(2, -3, 0),
        new Vector3(3, -3, 0),
        new Vector3(4, -3, 0),
    };
    private readonly Vector3Int[] playerDirections =
    {
        Vector3Int.up,
        Vector3Int.up + Vector3Int.right,
        Vector3Int.right,
        Vector3Int.right + Vector3Int.down,
        Vector3Int.down,
        Vector3Int.down + Vector3Int.left,
        Vector3Int.left,
        Vector3Int.left + Vector3Int.up,
    };
    
    // Movement settings
    public float smoothDampTime = .02f;
    public float distanceToSnap = .05f;

    // Variables for game control used in movement validation
    [SerializeField] private int passCount = 0;
    List<Vector3> destinationsPlayer = new List<Vector3>();
    List<Vector3> destinationsBall = new List<Vector3>();
    [SerializeField] private GameObject selectedChip;
    public Team currentTurn;
    [SerializeField] private GameObject ballChipPasser;

    public enum PlayerStates
    {
        WaitingPlayerInputChip,
        WaitingPlayerInputChipDestination,
        WaitingPlayerInputBallDestination,
    }
    public PlayerStates currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentTurn = Team.White;
        ballChip.GetComponent<ScriptBall>().teamPossession = currentTurn;
        currentState = PlayerStates.WaitingPlayerInputChip;
        boardBoxCollider = tilemapBoard.gameObject.GetComponent<BoxCollider2D>();
    }

    IEnumerator MoveChipAndUpdateState(GameObject chip, Vector3 destination)
    {
        /// Moves the chip to the destination passed with SmoothDamp (destination MUST be in world units) and then
        /// updates the game state.
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
        // When move is done, check if more moves are available (passes)
        if (IsBallPassable())
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
        }
    }

    bool IsFieldValidForPlayerChip(Vector3 destinationCenter)
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
        return true;
    }

    void CalculateMovesPlayer(GameObject playerChip, Vector3Int point, List<Vector3> destinations)
    {
        /// Calculates all possible fields the player can move to and writes their positions to destinations.
        for (int i = 1; i <= 2; i++)
        {
            foreach(Vector3Int direction in playerDirections)
            {
                Vector3Int destination = point + direction * i;
                Vector3 destinationCenter = tilemapBoard.GetCellCenterWorld(destination);
                if (IsFieldValidForPlayerChip(destinationCenter))
                {
                    tilemapHighlight.SetTile(destination, tileHighlight);
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
        if (Math.Abs(point.y) == 7 && (Array.IndexOf(redGoals, point) == -1 || Array.IndexOf(whiteGoals, point) == -1))
        {
            return true;
        }
        return false;
    }

    private bool IsFieldAValidGoal(Vector3 point)
    {
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

    private bool IsBallPassable()
    {
        /// Checks if it's possible to pass the ball from its position by checking for player chips in adyacent
        /// fields, if found, counts them to check for neutral spaces.
        /// If no neutral spaces were found and a pass is possible for the team whose turn it is and the pass limit hasn't been passed, return true.
        int redCount = 0, whiteCount = 0;
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
                            ballChipPasser = chip;
                        }
                    }
                    else
                    {
                        redCount++;
                        if (currentTurn == Team.Red)
                        {
                            ballChipPasser = chip;
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
                            passerCandidate = chip;
                        }
                    }
                    else
                    {
                        redCount++;
                        if (currentTurn == Team.Red)
                        {
                            passerCandidate = chip;
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
            if (passerCandidate == ballChipPasser)
            {
                return false;
            }
            return true;
        }
        // Do the same for white chips.
        if (whiteCount > redCount && currentTurn == Team.White && passCount <= 3)
        {
            if (passerCandidate == ballChipPasser)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    private bool IsFieldValidForBallChip(Vector3 destinationCenter)
    {
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
        /// Calculates all possible fields the ball can move to and writes their positions to destinations.
        var point = tilemapBoard.WorldToCell(ballChip.transform.position);
        
        for (int i = 1; i <= 4; i++)
        {
            foreach (Vector3Int direction in playerDirections)
            {
                Vector3Int destination = point + direction * i;
                Vector3 destinationCenter = tilemapBoard.GetCellCenterWorld(destination);
                if (IsFieldValidForBallChip(destinationCenter))
                {
                    tilemapHighlight.SetTile(destination, tileHighlight);
                    destinations.Add(destinationCenter);
                }
            }
        }
        yield return null;
    }

    public void UpdateBoard(PointerEventData eventData)
    {
        /// Updates board based on player input given by the IPointerHandlerEvent on the tilemap board GameObject.
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        var point = tilemapBoard.WorldToCell(worldPoint);
        var tile = tilemapBoard.GetTile(point);

        // Check if tile clicked is not blank (like the spaces next to the goals)
        if (tile != null)
        {
            var pointCenter = tilemapBoard.GetCellCenterWorld(point);
            //Debug.Log(pointCenter);

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
        }
        //boxCollider.enabled = false;
    }
}
