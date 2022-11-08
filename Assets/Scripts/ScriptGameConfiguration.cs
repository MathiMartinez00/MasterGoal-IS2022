using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptGameConfiguration : MonoBehaviour
{
    public Button buttonChipPlayer1, buttonChipPlayer2;
    public Image imageChipPlayer1, imageChipPlayer2;
    public Sprite[] chipSprites;

    // Start is called before the first frame update
    void Start()
    {
        buttonChipPlayer1.onClick.AddListener(delegate{ ChangeImageChipPlayer(1); });
        buttonChipPlayer2.onClick.AddListener(delegate{ ChangeImageChipPlayer(2); });
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
