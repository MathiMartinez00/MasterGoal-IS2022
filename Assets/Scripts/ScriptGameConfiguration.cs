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
    public Button buttonChipPlayer1, buttonChipPlayer2;
    public Image imageChipPlayer1, imageChipPlayer2;
    public Sprite[] chipSprites;
    public Toggle toggleModePlayerVsPlayer;
    public TMP_InputField inputFieldPlayer1, inputFieldPlayer2;
    public string player1Name, player2Name = "PC";

    // Start is called before the first frame update
    void Start()
    {
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
    }
    public void SeeToggle()
    {
        if (toggleModePlayerVsPlayer.isOn)
        {
            inputFieldPlayer2.GameObject().SetActive(false);
        }
        else
        {
            inputFieldPlayer2.GameObject().SetActive(true);
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
        PlayerPrefs.SetString("player1", player1Name);
        PlayerPrefs.SetString("player2", player2Name);
    }
}
