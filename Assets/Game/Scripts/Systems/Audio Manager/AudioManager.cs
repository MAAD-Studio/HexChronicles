using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public float backgroundVolume = 1.0f;
    public float soundFXVolume = 1.0f;

    public System.Action<float> OnBackgroundVolumeChanged;
    public System.Action<float> OnSoundFXVolumeChanged;

    public void SetBackgroundVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        backgroundVolume = volume;
        if (OnBackgroundVolumeChanged != null)
        {
            OnBackgroundVolumeChanged(backgroundVolume);
        }
    }

    public void SetSoundFXVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        soundFXVolume = volume;
        if (OnSoundFXVolumeChanged != null)
        {
            OnSoundFXVolumeChanged(soundFXVolume);
        }
    }
}
