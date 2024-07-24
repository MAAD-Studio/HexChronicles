using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

/// <summary>
/// Manages all audio playback for the game, including sound effects and music.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixer audioMixer; // Reference to the game's main AudioMixer

    private AudioSource musicSource; // Dedicated AudioSource for music playback
    private AudioSource sfxSource; // Dedicated AudioSource for sound effects

    [SerializeField] private List<AudioSO> audios = new List<AudioSO>(); // List of Sound Effect Audio ScriptableObjects
    [SerializeField] private List<AudioSO> musics = new List<AudioSO>(); // List of Music Audio ScriptableObjects

    private Dictionary<string, AudioSO> audioDictionary = new(); // Maps audio names to their AudioSO

    #region Unity Methods

    public override void Awake()
    {
        base.Awake();
        InitializeAudioSources();
        LoadAudioObjects();
    }

    private void Start()
    {
        PlayMusic("MainTheme");
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes and configures the AudioSource components.
    /// </summary>
    private void InitializeAudioSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        if (audioMixer == null)
        {
            audioMixer = Resources.Load<AudioMixer>("Audio/AudioMixer");
        }
        Debug.Assert(audioMixer != null, "AudioManager can't find AudioMixer");

        musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
    }

    /// <summary>
    /// Loads AudioSO objects into the audioDictionary for quick access.
    /// </summary>
    private void LoadAudioObjects()
    {
        foreach (AudioSO audio in audios)
        {
            if (!audioDictionary.ContainsKey(audio.name))
            {
                audioDictionary.Add(audio.name, audio);
            }
        }

        foreach (AudioSO music in musics)
        {
            if (!audioDictionary.ContainsKey(music.name))
            {
                audioDictionary.Add(music.name, music);
            }
        }
    }

    #endregion

    #region Audio Playback Methods

    /// <summary>
    /// Plays a sound effect identified by name.
    /// </summary>
    /// <param name="soundName">The name of the sound effect to play.</param>
    public void PlaySound(string soundName)
    {
        if (audioDictionary.TryGetValue(soundName, out AudioSO audio))
        {
            sfxSource.PlayOneShot(audio.clip, audio.volume);
            sfxSource.pitch = audio.pitch;
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found!");
        }
    }

    /// <summary>
    /// Plays a sound effect from an AudioSO. Add it into the dictionary if it is not there
    /// </summary>
    /// <param name="audio">The scriptable object.</param>
    public void PlaySound(AudioSO audio)
    {
        if (audio.clip != null)
        {
            sfxSource.PlayOneShot(audio.clip, audio.volume);
            sfxSource.pitch = audio.pitch;
            if (!audioDictionary.ContainsKey(audio.name))
            {
                audioDictionary.Add(audio.name, audio);
            }
        }
        else
        {
            Debug.LogWarning($"Sound {audio.name} not found!");
        }
    }

    /// <summary>
    /// Plays a music track identified by name, with optional looping.
    /// </summary>
    /// <param name="musicName">The name of the music track to play.</param>
    public void PlayMusic(string musicName)
    {
        if (audioDictionary.TryGetValue(musicName, out AudioSO music))
        {
            musicSource.clip = music.clip;
            musicSource.volume = music.volume;
            musicSource.pitch = music.pitch;
            musicSource.loop = music.loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music {musicName} not found!");
        }
    }

    /// <summary>
    /// Plays a music track from a scriptable object.
    /// </summary>
    /// <param name="music">The scriptable object.</param>
    public void PlayMusic(AudioSO music)
    {
        if (music.clip != null)
        {
            musicSource.clip = music.clip;
            musicSource.volume = music.volume;
            musicSource.pitch = music.pitch;
            musicSource.loop = music.loop;
            musicSource.Play();
            if (!audioDictionary.ContainsKey(music.name))
            {
                audioDictionary.Add(music.name, music);
            }
        }
        else
        {
            Debug.LogWarning($"Music {music.name} not found!");
        }
    }

    /// <summary>
    /// Stops all currently playing sound effects.
    /// </summary>
    public void StopSound()
    {
        sfxSource.Stop();
    }

    /// <summary>
    /// Stops the currently playing music track.
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    #endregion

    #region Volume and Mute Control

    /// <summary>
    /// Sets the volume for sound effects.
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0).</param>
    public void SetSoundVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    /// <summary>
    /// Sets the volume for music.
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0).</param>
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    /// <summary>
    /// Mutes or unmutes all audio.
    /// </summary>
    /// <param name="mute">Whether to mute (true) or unmute (false) all audio.</param>
    public void MuteAll(bool mute)
    {
        musicSource.mute = mute;
        sfxSource.mute = mute;
    }

    #endregion
}