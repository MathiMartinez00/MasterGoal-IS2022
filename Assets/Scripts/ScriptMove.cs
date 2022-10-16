using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScriptMove : MonoBehaviour
{

    public GameObject[] playerChips;
    public Vector2[] defaultPositions;
    public GameObject ballChip;
    public Vector2 defaultBallPosition;
    public GameObject[] waypoints;
    public GameObject[] waypointsBallMovement;
    public GameObject[] waypointsBallPassing;
    public GameObject[] waypointsBallGoal;
    public GameObject[] waypointsBallPassingBetweenPlayers;
    public GameObject[] waypointsBallNeutralPositionBreak;
    public GameObject[] waypointsBallWrongMoveCorner;
    public GameObject[] waypointsBallWrongMovePosession;
    public GameObject[] waypointsBallWrongMoveOwnArea;
    public float speed = 1;
    Vector2 originalPosition;
    int waypointIterator = 0;
    public float distance = 0.375f;

    public GameObject titleTextObject, descriptionTextObject;
    public Button nextButton, previousButton;

    public GameObject cross;

    public Vector2 positionStartBallPassing = new Vector2(-0.375f, 1.875f);
    public Vector2 positionStartPlayerMovement = new Vector2(1.125f, 1.125f);
    public Vector2 positionStartBallGoal = new Vector2(-1.5f, -1.875f);

    public int stepTutorial = 0;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = playerChips[0].transform.position;
        nextButton.onClick.AddListener(NextStep);
        previousButton.onClick.AddListener(PreviousStep);
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        switch (stepTutorial)
        {
            case 0:
                ResetPositions();
                break;
            case 1:
                // Show player movement
                float step = speed * Time.deltaTime;
                playerChips[0].transform.position = Vector2.MoveTowards(playerChips[0].transform.position, waypoints[waypointIterator].transform.position, step);
                if (playerChips[0].transform.position == waypoints[waypointIterator].transform.position)
                {
                    playerChips[0].transform.position = originalPosition;
                    waypointIterator = waypointIterator == waypoints.Length - 1 ? 0 : waypointIterator + 1;
                    speed = waypointIterator < waypoints.Length / 2 ? 1 : 2;
                }
                break;
            case 2:
                // Show ball movement
                HandleMovement(ballChip, waypointsBallMovement, defaultBallPosition);
                break;
            case 3:
                // Show ball throw
                HandleMovement(ballChip, waypointsBallPassing, positionStartBallPassing);
                break;
            case 4:
                // Show goals
                HandleMovement(ballChip, waypointsBallGoal, positionStartBallGoal);
                break;
            case 5:
                // Show passing 
                HandleMovement(ballChip, waypointsBallPassingBetweenPlayers, positionStartBallPassing);
                break;
            case 6:
                // Show ball passing above a player
                // TODO: Look up this answer to wait a while and then continue
                // https://gamedev.stackexchange.com/questions/182912/moving-a-gameobject-to-a-position-waiting-and-then-moving-it-again
                HandleMovement(ballChip, waypointsBallPassingBetweenPlayers, positionStartBallPassing);
                break;
            case 8:
                // Show neutral position breaking
                HandleMovement(ballChip, waypointsBallNeutralPositionBreak, new Vector2(-1.5f, 0));
                break;
            case 9:
                // Show ball moving to corner
                HandleMovement(ballChip, waypointsBallWrongMoveCorner, new Vector2(-1.875f, 1.125f));
                break;
            case 10:
                // Show ball ownership at the end of turn
                HandleMovement(ballChip, waypointsBallWrongMovePosession, new Vector2(-0.375f, 1.125f));
                break;
            case 11:
                // Show ball in own player area
                HandleMovement(ballChip, waypointsBallWrongMoveOwnArea, new Vector2(-0.375f, 0));
                break;

        }
        previousButton.gameObject.SetActive(stepTutorial != 0);
        cross.SetActive(stepTutorial >= 9);
        nextButton.gameObject.SetActive(stepTutorial != 11);
    }

    void HandleMovement(GameObject chip, GameObject[] destinations, Vector2 originalPos, float time = 0.015f)
    {
        Vector2 velocity = Vector2.zero;
        chip.transform.position = Vector2.SmoothDamp(chip.transform.position, destinations[waypointIterator].transform.position, ref velocity, time);
        if (chip.transform.position == destinations[waypointIterator].transform.position)
        {
            chip.transform.position = originalPos;
            waypointIterator = waypointIterator == destinations.Length - 1 ? 0 : waypointIterator + 1;
        }
    }

    void ResetPositions()
    {
        for (int i = 0; i < playerChips.Length; i++)
        {
            playerChips[i].transform.position = defaultPositions[i];
        }
        ballChip.transform.position = defaultBallPosition;
    }

    void UpdateText()
    {
        TextMeshProUGUI titleText = titleTextObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = descriptionTextObject.GetComponent<TextMeshProUGUI>();
        switch (stepTutorial)
        {
            case 0:
                titleText.text = "Jugadores";
                descriptionText.text = "El juego está conformado por 2 equipos representados por las fichas rojas y blancas.";
                break;
            case 1:
                titleText.text = "Movimiento del jugador";
                descriptionText.text = "Los jugadores se pueden mover hasta 2 casillas.";
                break;
            case 2:
                titleText.text = "Movimiento de la pelota";
                descriptionText.text = "La pelota es representada por la ficha gris y se puede mover hasta 4 casillas.";
                break;
            case 3:
                titleText.text = "Movimiento de la pelota";
                descriptionText.text = "La pelota debe estar en un espacio adyacente a un jugador para moverla.";
                break;
            case 4:
                titleText.text = "Goles";
                descriptionText.text = "Los arcos son los espacios blancos. Los goles se pueden meter en todas las direcciones.\r\nEl juego termina al meter 2 goles.";
                break;
            case 5:
                titleText.text = "Pases";
                descriptionText.text = "La pelota se puede mover hasta 4 veces seguidas en un turno.\r\nLos pases son entre jugadores del mismo equipo.";
                break;
            case 6:
                titleText.text = "Pases";
                descriptionText.text = "La pelota puede pasar por casillas ocupadas por jugadores.";
                break;
            case 7:
                titleText.text = "Casilla neutra";
                descriptionText.text = "Cuando la pelota es compartida por un número igual de jugadores esta no pertenece a ninguno.";
                break;
            case 8:
                titleText.text = "Casilla neutra";
                descriptionText.text = "La neutralidad se rompe si se obtiene mayoría por parte de un equpo.";
                break;
            case 9:
                titleText.text = "Movimientos prohibidos";
                descriptionText.text = "La pelota no puede terminar en el corner propio.";
                break;
            case 10:
                titleText.text = "Movimientos prohibidos";
                descriptionText.text = "Tampoco puede terminar en posesión de un equipo al final del turno.";
                break;
            case 11:
                titleText.text = "Movimientos prohibidos";
                descriptionText.text = "Ni puede dejar la pelota dentro de su propia área.";
                break;
        }
    }

    void ResetState()
    {
        ResetPositions();
        UpdateText();
        waypointIterator = 0;
        switch (stepTutorial)
        {
            case 1:
                playerChips[0].transform.position = positionStartPlayerMovement;
                break;
            case 3:
                playerChips[0].transform.position = positionStartPlayerMovement;
                ballChip.transform.position = positionStartBallPassing;
                break;
            case 4:
                playerChips[0].transform.position = new Vector2(-1.875f, -1.5f);
                ballChip.transform.position = positionStartBallGoal;
                break;
            case 5:
                ballChip.transform.position = new Vector2(-0.375f, 1.875f);
                break;
            case 6:
                ballChip.transform.position = new Vector2(-0.375f, 1.875f);
                playerChips[2].transform.position = new Vector2(-0.375f, 0.375f);
                break;
            case 7:
                playerChips[1].transform.position = new Vector2(-1.125f, 0);
                playerChips[2].transform.position = new Vector2(-1.5f, -0.375f);
                ballChip.transform.position = new Vector2(-1.5f, 0);
                break;
            case 8:
                playerChips[1].transform.position = new Vector2(-1.125f, 0);
                playerChips[2].transform.position = new Vector2(-1.5f, -0.375f);
                playerChips[3].transform.position = new Vector2(-1.125f, -0.375f);
                ballChip.transform.position = new Vector2(-1.5f, 0);
                break;
            case 9:
                cross.transform.position = waypointsBallWrongMoveCorner[0].transform.position;
                playerChips[1].transform.position = new Vector2(-1.5f, 1.125f);
                ballChip.transform.position = new Vector2(-1.875f, 1.125f);
                break;
            case 10:
                cross.transform.position = waypointsBallWrongMovePosession[0].transform.position;
                playerChips[1].transform.position = new Vector2(-0.75f, 0);
                ballChip.transform.position = new Vector2(-0.375f, 1.125f);
                break;
            case 11:
                cross.transform.position = waypointsBallWrongMoveOwnArea[0].transform.position;
                playerChips[0].transform.position = new Vector2(-0.75f, 0);
                playerChips[1].transform.position = new Vector2(-0.75f, 1.125f);
                ballChip.transform.position = new Vector2(-0.375f, 0);
                break;
            default:
                break;
        }
    }

    void NextStep()
    {
        stepTutorial++;
        ResetState();
    }

    void PreviousStep()
    {
        stepTutorial--;
        ResetState();
    }
}
