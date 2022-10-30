using JetBrains.Annotations;
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

    public Vector2 positionStartBallPassing = new Vector2(-0.375f, 1.875f);
    public Vector2 positionStartPlayerMovement = new Vector2(1.125f, 1.125f);
    public Vector2 positionStartBallGoal = new Vector2(-1.5f, -1.875f);

    public int stepTutorial = 0;

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

    IEnumerator MoveAndWait(GameObject chip, GameObject[] destinations, Vector2 originalPos, bool changeOriginalPos = false, float time = 0.05f)
    {
        // The whole changeOriginalPos thing is probably not the best idea but it is what it is
        while (chip.transform.position != destinations[waypointIterator].transform.position)
        {
            Vector2 velocity = Vector2.zero;
            chip.transform.position = Vector2.SmoothDamp(chip.transform.position, destinations[waypointIterator].transform.position, ref velocity, time);
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

    void HandleMovement(GameObject chip, GameObject[] destinations, Vector2 originalPos, float time = 0.02f)
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
                StartCoroutine(MoveAndWait(playerChips[0], waypoints, new Vector2(1.125f, 1.125f)));
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
                playerChips[0].transform.position = new Vector2(-1.875f, -1.5f);
                ballChip.transform.position = positionStartBallGoal;
                StartCoroutine(MoveAndWait(ballChip, waypointsBallGoal, positionStartBallGoal));
                break;
            case 5:
                ballChip.transform.position = new Vector2(-0.375f, 1.875f);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallPassingBetweenPlayers, positionStartBallPassing, true));
                break;
            case 6:
                ballChip.transform.position = new Vector2(-0.375f, 1.875f);
                playerChips[2].transform.position = new Vector2(-0.375f, 0.375f);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallPassingBetweenPlayers, positionStartBallPassing, true));
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
                StartCoroutine(MoveAndWait(ballChip, waypointsBallNeutralPositionBreak, new Vector2(-1.5f, 0)));
                break;
            case 9:
                cross.transform.position = waypointsBallWrongMoveCorner[0].transform.position;
                playerChips[1].transform.position = new Vector2(-1.5f, 1.125f);
                ballChip.transform.position = new Vector2(-1.875f, 1.125f);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallWrongMoveCorner, new Vector2(-1.875f, 1.125f)));
                break;
            case 10:
                cross.transform.position = waypointsBallWrongMovePosession[1].transform.position;
                playerChips[1].transform.position = new Vector2(-0.75f, 0);
                ballChip.transform.position = new Vector2(-0.375f, 1.125f);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallWrongMovePosession, new Vector2(-0.375f, 1.125f), true));
                break;
            case 11:
                cross.transform.position = waypointsBallWrongMoveOwnArea[0].transform.position;
                playerChips[0].transform.position = new Vector2(-0.75f, 0);
                playerChips[1].transform.position = new Vector2(-0.75f, 1.125f);
                ballChip.transform.position = new Vector2(-0.375f, 0);
                StartCoroutine(MoveAndWait(ballChip, waypointsBallWrongMoveOwnArea, new Vector2(-0.375f, 0)));
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
