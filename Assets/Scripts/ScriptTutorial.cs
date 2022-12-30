using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScriptTutorial : MonoBehaviour
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
    public int waypointIterator = 0;

    public GameObject titleTextObject, descriptionTextObject;
    public Button nextButton, previousButton;

    public GameObject cross;

    public Vector2 positionStartBallPassing = new Vector2(-1, 5);
    public Vector2 positionStartPlayerMovement = new Vector2(3, 3);
    public Vector2 positionStartBallGoal = new Vector2(-4, -5);

    public int stepTutorial = 0;
    public float distanceToSnap = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        nextButton.onClick.AddListener(NextStep);
        previousButton.onClick.AddListener(PreviousStep);
        UpdateText();
        ResetPositions();
    }

    // Update is called once per frame
    void Update()
    {
        previousButton.gameObject.SetActive(stepTutorial != 0);
        cross.SetActive(stepTutorial >= 9);
        nextButton.gameObject.SetActive(stepTutorial != 11);
    }

    IEnumerator MoveAndWait(GameObject chip, GameObject[] destinations, Vector2 originalPos, bool changeOriginalPos = false, float time = 0.1f)
    {
        // The whole changeOriginalPos thing is probably not the best idea but it is what it is
        while (chip.transform.position != destinations[waypointIterator].transform.position)
        {
            Vector2 velocity = Vector2.zero;
            chip.transform.position = Vector2.SmoothDamp(chip.transform.position, destinations[waypointIterator].transform.position, ref velocity, time);
            if (Math.Abs(Vector2.Distance(chip.transform.position, destinations[waypointIterator].transform.position)) < distanceToSnap)
            {
                chip.transform.position = destinations[waypointIterator].transform.position;
            }
            yield return null;
        }
        if (chip.transform.position == destinations[waypointIterator].transform.position)
        {
            if (!changeOriginalPos)
            {
                chip.transform.position = originalPos;
                waypointIterator = waypointIterator == destinations.Length - 1 ? 0 : waypointIterator + 1;
            }
            else
            {
                // All destionations (the array) have the start position as its last element, return there
                if (destinations.Length - 2 == waypointIterator)
                {
                    chip.transform.position = destinations[^1].transform.position;
                }
                waypointIterator = waypointIterator == destinations.Length - 2 ? 0 : waypointIterator + 1;
            }
            yield return new WaitForSeconds(1f);
        }
        if (changeOriginalPos)
        {
            yield return MoveAndWait(chip, destinations, destinations[waypointIterator].transform.position, true);
        }
        else
        {
            yield return MoveAndWait(chip, destinations, originalPos);
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

                descriptionText.text =
                "El juego está conformado por 2 equipos con 2 jugadores " +
                "por equipo.";

                break;
            case 1:
                titleText.text = "Movimiento del jugador";

                descriptionText.text =
                "Los jugadores se pueden mover hasta 2 casillas en una " +
                "sola acción. Los jugadores no pueden saltar sobre otros" +
                " jugadores o sobre la pelota.";

                break;
            case 2:
                titleText.text = "Movimiento de la pelota";

                descriptionText.text =
                "La pelota es representada por la ficha amarilla y se puede" +
                " mover hasta 4 casillas por acción.";

                break;
            case 3:
                titleText.text = "Movimiento de la pelota";

                descriptionText.text =
                "La pelota debe estar en una casilla adyacente al jugador" +
                " para que se pueda mover.";

                break;
            case 4:
                titleText.text = "Goles";

                descriptionText.text =
                "Los arcos se representan con las casillas en blanco. Los" +
                " goles se pueden meter en todas las direcciones.\r\nEl " +
                "juego termina cuando un jugador haya anotado 2 goles.";

                break;
            case 5:
                titleText.text = "Pases";

                descriptionText.text =
                "La pelota se puede mover hasta 4 veces seguidas en un " +
                "turno, 3 pases y 1 movimiento definitivo.\r\nLos pases" +
                " siempre son entre jugadores del mismo equipo.";

                break;
            case 6:
                titleText.text = "Movimiento de la pelota";

                descriptionText.text =
                "La pelota puede saltar sobre los jugadores.";

                break;
            case 7:
                titleText.text = "Casilla neutra";

                descriptionText.text =
                "Cuando la pelota está rodeada de un número igual de " +
                "jugadores, se dice que está en una casilla neutra, y " +
                "ningún jugador puede moverla.";

                break;
            case 8:
                titleText.text = "Casilla neutra";

                descriptionText.text =
                "La casilla deja de ser neutra si un equipo obtiene una " +
                "mayoría de jugadores alrededor de ella.";

                break;
            case 9:
                titleText.text = "Movimientos prohibidos";

                descriptionText.text =
                "La pelota no puede terminar en el corner del equipo de " +
                "turno al final del turno.";

                break;
            case 10:
                titleText.text = "Movimientos prohibidos";

                descriptionText.text =
                "La pelota no puede terminar en posesión de ningún equipo" +
                " al final del turno.";

                break;
            case 11:
                titleText.text = "Movimientos prohibidos";

                descriptionText.text =
                "La pelota no puede terminar dentro del área del equipo " +
                "de turno al final del turno.";

                break;
        }
    }

    void ResetState()
    {
        StopAllCoroutines();
        ResetPositions();
        UpdateText();
        waypointIterator = 0;
        switch (stepTutorial)
        {
            case 0:
                ResetPositions();
                break;
            case 1:
                playerChips[0].transform.position = positionStartPlayerMovement;
                StartCoroutine(MoveAndWait(playerChips[0], waypoints, new Vector2(3, 3)));
                break;
            case 2:
                StartCoroutine(MoveAndWait(ballChip, waypointsBallMovement, defaultBallPosition));
                break;
            case 3:
                playerChips[0].transform.position = positionStartPlayerMovement;
                ballChip.transform.position = positionStartBallPassing;
                StartCoroutine(MoveAndWait(ballChip, waypointsBallPassing, positionStartBallPassing));
                break;
            case 4:
                playerChips[0].transform.position = new Vector2(-5, -4f);
                ballChip.transform.position = positionStartBallGoal;
                StartCoroutine(MoveAndWait(ballChip, waypointsBallGoal, positionStartBallGoal));
                break;
            case 5:
                ballChip.transform.position = new Vector2(-1, 5);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallPassingBetweenPlayers, positionStartBallPassing, true));
                break;
            case 6:
                ballChip.transform.position = new Vector2(-1, 5);
                playerChips[2].transform.position = new Vector2(-1, 1);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallPassingBetweenPlayers, positionStartBallPassing, true));
                break;
            case 7:
                playerChips[1].transform.position = new Vector2(-3, 0);
                playerChips[2].transform.position = new Vector2(-4, -1);
                ballChip.transform.position = new Vector2(-4, 0);
                break;
            case 8:
                playerChips[1].transform.position = new Vector2(-3, 0);
                playerChips[2].transform.position = new Vector2(-4, -1);
                playerChips[3].transform.position = new Vector2(-3, -1);
                ballChip.transform.position = new Vector2(-4, 0);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallNeutralPositionBreak, new Vector2(-4, 0)));
                break;
            case 9:
                cross.transform.position = waypointsBallWrongMoveCorner[0].transform.position;
                playerChips[1].transform.position = new Vector2(-4, 3);
                ballChip.transform.position = new Vector2(-5, 3);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallWrongMoveCorner, new Vector2(-5, 3)));
                break;
            case 10:
                cross.transform.position = waypointsBallWrongMovePosession[1].transform.position;
                playerChips[1].transform.position = new Vector2(-2, 0);
                ballChip.transform.position = new Vector2(-1, 3);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallWrongMovePosession, new Vector2(-1, 3), true));
                break;
            case 11:
                cross.transform.position = waypointsBallWrongMoveOwnArea[0].transform.position;
                playerChips[0].transform.position = new Vector2(-2, 0);
                ballChip.transform.position = new Vector2(-1, 0);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallWrongMoveOwnArea, new Vector2(-1, 0)));
                break;
            default:
                break;
        }
    }

    void NextStep()
    {
        // Play the sound of the menu button.
        FindObjectOfType<AudioManager>().PlaySound("Menu button");
        stepTutorial++;
        ResetState();
    }

    void PreviousStep()
    {
        // Play the sound of the menu button.
        FindObjectOfType<AudioManager>().PlaySound("Menu button");
        stepTutorial--;
        ResetState();
    }
}
