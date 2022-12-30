using UnityEngine;
using UnityEngine.Audio;
using System;

// This class is useful to manage a large amount of audio tracks across
// multiple scenes in Unity.
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] music;
    // The audio track that will be playing when this object initializes.
    private int currentMusicTrack = 4;
    private bool sfxOn = true;
    private bool musicOn = true;

    public static AudioManager instance;

    void Awake()
    {
        // Allow only one AudioManager instance to exist at a time.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // This allows the current instance to persist across scenes.
        DontDestroyOnLoad(gameObject);

        // Add an AudioSource to each sound in the sounds array.
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        // Add an AudioSource to each music track in the music array.
        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    // When the object initializes, start playing the first track.
    void Start()
    {
        // Play the music only if the music is activated.
        if (musicOn)
        {
            // Check if the first track index is whithin range.
            if (currentMusicTrack >= music.Length)
                ChangeMusicTrack();
            else
                music[currentMusicTrack].source.Play();
        }
    }

    // Useful for cycling between all the music tracks.
    public void ChangeMusicTrack()
    {
        // Play the music only if it's activated.
        if (musicOn)
        {
            // Stop whatever was playing before.
            music[currentMusicTrack].source.Stop();

            // Check if we have to restart the counter.
            if (currentMusicTrack >= music.Length - 1)
                currentMusicTrack = 0;
            else
                currentMusicTrack++;

            // Switch to the next track.
            music[currentMusicTrack].source.Play();
        }
    }

    // Play a sound by passing in its name.
    public void PlaySound(string name)
    {
        // Play sound only if SFX are activated.
        if (sfxOn)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null)
                Debug.LogWarning("Sound: " + name + " not found.");

            s.source.Play();
        }
    }

    // Play a music track by passing in its name.
    public void PlayMusic(string name)
    {
        // Play sound only if SFX are activated.
        if (sfxOn)
        {
            Sound s = Array.Find(music, s => s.name == name);

            if (s == null)
                Debug.LogWarning("Song: " + name + " not found.");

            s.source.Play();
        }
    }

    public void PlayMenuButtonSound()
    {
        // Play sound only if SFX are activated.
        if (sfxOn)
            PlaySound("Menu button");
    }

    public void PlayButtonSound()
    {
        // Play sound only if SFX are activated.
        if (sfxOn)
            PlaySound("Button press");
    }

    public void ToggleMusic()
    {
        if (musicOn)
        {
            music[currentMusicTrack].source.Stop();
            musicOn = false;
        }
        else
        {
            music[currentMusicTrack].source.Play();
            musicOn = true;
        }
    }

    public void ToggleSfx()
    {
        if (sfxOn)
            sfxOn = false;
        else
            sfxOn = true;
    }
}