using UnityEngine;

public class AudioConfigurator : MonoBehaviour
{
    public void ToggleMusic()
    {
        FindObjectOfType<AudioManager>().ToggleMusic();
    }

    public void ToggleSfx()
    {
        FindObjectOfType<AudioManager>().ToggleSfx();
    }

    public void ChangeMusicTrack()
    {
        FindObjectOfType<AudioManager>().ChangeMusicTrack();
    }
}
