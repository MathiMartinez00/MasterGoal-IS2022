using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptGameConfiguration : MonoBehaviour
{
    public Button buttonChipPlayer1, buttonChipPlayer2, buttonPlay;
    public Image imageChipPlayer1, imageChipPlayer2;
    public Sprite[] chipSprites;
    public Toggle toggleModePlayerVsPlayer;
    public TMP_InputField inputFieldPlayer1, inputFieldPlayer2;
    public string player1Name = "Jugador1", player2Name = "PC";
    public int gameMode = 0; // 0 = player1 vs PC. 1 = player1 vs player2

    // Start is called before the first frame update
    void Start()
    {
        if (IsTheSameChipColor())
        {
            buttonPlay.image.color = Color.red;
            buttonPlay.interactable = false;
        }
        buttonChipPlayer1.onClick.AddListener(delegate{ ChangeImageChipPlayer(1); });
        buttonChipPlayer2.onClick.AddListener(delegate{ ChangeImageChipPlayer(2); });
        toggleModePlayerVsPlayer.onValueChanged.AddListener(delegate { SeeToggle(); });
        inputFieldPlayer1.onEndEdit.AddListener(delegate { NameSetted(1); });
        inputFieldPlayer2.onEndEdit.AddListener(delegate { NameSetted(2); });
    }
    public void ChangeImageChipPlayer(int player)
    {
        Image imageChip = player == 1 ? imageChipPlayer1 : imageChipPlayer2;
        for (int i = 0; i < chipSprites.Length; i++)
        {
            if (imageChip.sprite == chipSprites[i])
            {
                if (i < chipSprites.Length - 1)
                {
                    imageChip.sprite = chipSprites[i + 1];
                }
                else
                {
                    imageChip.sprite = chipSprites[0];
                }
                break;
            }
        }
        if (IsTheSameChipColor())
        {
            buttonPlay.image.color = Color.red;
            buttonPlay.interactable = false;
        }
        else
        {
            buttonPlay.image.color = buttonChipPlayer1.image.color;
            buttonPlay.interactable = true;
        }
    }
    public void SeeToggle()
    {
        if (toggleModePlayerVsPlayer.isOn)
        {
            inputFieldPlayer2.GameObject().SetActive(false);
            gameMode = 0;
        }
        else
        {
            inputFieldPlayer2.GameObject().SetActive(true);
            gameMode = 1;
        }
    }
    public void NameSetted(int player)
    {
        TMP_InputField inputField;
        if (player == 1)
        {
            inputField = inputFieldPlayer1;
            player1Name = inputField.GetComponentInChildren<TMP_InputField>().text;
        }
        else
        {
            inputField = inputFieldPlayer2;
            player2Name = inputField.GetComponentInChildren<TMP_InputField>().text;
        }
    }
    void OnDisable()
    {
        //Debug.Log("Hey?");
        PlayerPrefs.SetInt("gameMode", gameMode); // 0 = player1 vs PC. 1 = player1 vs player2
        PlayerPrefs.SetString("player1", player1Name);
        PlayerPrefs.SetString("player2", player2Name);
        PlayerPrefs.SetString("color1", imageChipPlayer1.sprite.ToString()); // color 1 for player 1
        PlayerPrefs.SetString("color2", imageChipPlayer1.sprite.ToString());
    }
    public bool IsTheSameChipColor()
    {
        if (imageChipPlayer1.sprite.ToString() == imageChipPlayer2.sprite.ToString())
        {
            return true;
        }
        return false;
    }
}
