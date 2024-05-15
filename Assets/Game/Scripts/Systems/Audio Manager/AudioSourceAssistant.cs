using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceAssistant : MonoBehaviour
{
    public enum AudioType
    {
        SoundFX,
        BackgroundMusic
    };
    public AudioType audioType;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioType == AudioType.SoundFX)
        {
            AudioManager.Instance.OnSoundFXVolumeChanged += OnVolumeChanged;
            audioSource.volume = AudioManager.Instance.soundFXVolume;
        }
        else
        {
            AudioManager.Instance.OnBackgroundVolumeChanged += OnVolumeChanged;
            audioSource.volume = AudioManager.Instance.backgroundVolume;
        }
    }

    private void OnVolumeChanged(float volume)
    {
        audioSource.volume = volume;
    }
}
