using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class ScriptController : MonoBehaviour, IPointerDownHandler
{
    public Tilemap tilemapBoard;
    public Tilemap tilemapHighlight;
    public Tile tileHighlight;
    public BoxCollider2D boardBoxCollider;
    // TODO: Probably make this an array
    public GameObject playerChip;
    public GameObject[] playerChips;
    public GameObject ballChip;
    // TODO: Look up list comprehension?
    private readonly Vector3[] goalWorldPoints = { 
        new Vector3(-2, -7, 0),
        new Vector3(-1, -7, 0),
        new Vector3(0, -7, 0),
        new Vector3(1, -7, 0),
        new Vector3(2, -7, 0),
        new Vector3(-2, 7, 0),
        new Vector3(-1, 7, 0),
        new Vector3(0, 7, 0),
        new Vector3(1, 7, 0),
        new Vector3(2, 7, 0),
    };
    private readonly Vector3[] corners =
    {
        new Vector3(-5, 6, 0),
        new Vector3(5, 6, 0),
        new Vector3(-5, -6, 0),
        new Vector3(5, -6, 0),
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
    public float smoothDampTime = .02f;
    public float distanceToSnap = .05f;

    public enum PlayerStates
    {
        WaitingPlayerInputChip,
        WaitingPlayerInputChipDestination,
        WaitingPlayerInputBallDestination,
    }
    public PlayerStates currentState;
    private GameObject selectedChip;

    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerStates.WaitingPlayerInputChip;
        boardBoxCollider = tilemapBoard.gameObject.GetComponent<BoxCollider2D>();
    }

    IEnumerator MoveChip(GameObject chip, Vector3 destination)
    {
        /// Moves the chip to the destination passed with SmoothDamp. Destination MUST be in world units.
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
    }

    bool IsFieldValid(Vector3 destinationCenter)
    {
        /// Checks and returns if a field is valid for player (chip) movement.
        // Is there a chip in destinationCenter?
        foreach (var chip in playerChips)
        {
            if (chip.transform.position == destinationCenter)
            {
                return false;
            }
        }
        // Is field out of bounds?
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
                if (IsFieldValid(destinationCenter))
                {
                    tilemapHighlight.SetTile(destination, tileHighlight);
                    destinations.Add(destinationCenter);
                }
            }
        }
    }

    List<Vector3> destinationsPlayer = new List<Vector3>();

    public void OnPointerDown(PointerEventData eventData)
    {
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
                    if (pointCenter == playerChip.transform.position)
                    {
                        selectedChip = playerChip;
                        currentState = PlayerStates.WaitingPlayerInputChipDestination;
                        CalculateMovesPlayer(selectedChip, point, destinationsPlayer);
                    }
                    break;

                case PlayerStates.WaitingPlayerInputChipDestination:
                    // Did the player choose a valid destination?
                    if (destinationsPlayer.Contains(pointCenter)) 
                    {
                        StartCoroutine(MoveChip(selectedChip, pointCenter));
                        //selectedChip.transform.position = pointCenter;
                        currentState = PlayerStates.WaitingPlayerInputChip;
                        tilemapHighlight.ClearAllTiles();
                        destinationsPlayer.Clear();
                    }
                    break;

                case PlayerStates.WaitingPlayerInputBallDestination:

                    break;
            }

            if (Array.IndexOf(goalWorldPoints, pointCenter) > -1) {
                Debug.Log("Clicked a goal!");
            }
        }
        //boxCollider.enabled = false;
    }
}
