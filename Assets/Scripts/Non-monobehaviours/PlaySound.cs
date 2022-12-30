using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource buttonSound;

    public void PlayButtonSound()
    {
        buttonSound.Stop();
        buttonSound.Play();
    }
}
