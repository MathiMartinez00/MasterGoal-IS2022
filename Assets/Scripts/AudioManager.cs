using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] music;

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        PlayMusic("Hypnotic puzzle");
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
            Debug.LogWarning("Sound: " + name + " not found.");

        s.source.Play();
    }

    public void PlayMenuButtonSound()
    {
        PlaySound("Menu button");
    }

    public void PlayButtonSound()
    {
        PlaySound("Button press");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(music, s => s.name == name);

        if (s == null)
            Debug.LogWarning("Song: " + name + " not found.");

        s.source.Play();
    }

}
