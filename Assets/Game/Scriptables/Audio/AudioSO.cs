using UnityEngine;

/// <summary>
/// Defines an audio asset for use in the game, including clip, volume, pitch, and looping settings.
/// </summary>
[CreateAssetMenu(fileName = "AudioAsset", menuName = "Audio Asset")]
public class AudioSO : ScriptableObject
{
    [Tooltip("The name of the audio asset.")]
    public new string name;

    [Tooltip("The audio clip to be played.")]
    public AudioClip clip;

    [Tooltip("Volume level of the audio clip. Range: 0.0 to 1.0.")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("Pitch level of the audio clip. Range: 0.1 to 3.0.")]
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Tooltip("Determines whether the audio clip should loop.")]
    public bool loop = false;
}
