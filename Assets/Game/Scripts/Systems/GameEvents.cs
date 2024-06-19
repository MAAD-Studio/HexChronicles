public struct OnNewLevelStart { }
public struct OnPlayerTurn { }
public struct OnEnemyTurn { }

public struct CharacterSelected
{
    public Character character;
}

public struct CharacterHasMadeDecision
{
    public Character character;
}


/// <summary>
/// Event to notify changes in audio settings like master volume, music volume, and SFX volume.
/// </summary>
public struct AudioSettingsChangedEvent
{
    public float MasterVolume; ///< The master volume level, typically between 0.0 (mute) and 1.0 (max volume).
    public float MusicVolume;  ///< The music volume level, typically between 0.0 (mute) and 1.0 (max volume).
    public float SfxVolume;    ///< The SFX (Sound Effects) volume level, typically between 0.0 (mute) and 1.0 (max volume).
}

/// <summary>
/// Event to notify changes in the game's resolution and fullscreen mode.
/// </summary>
public struct ResolutionChangedEvent
{
    public int Width;         ///< The width of the resolution.
    public int Height;        ///< The height of the resolution.
    public bool IsFullScreen; ///< Indicates whether the game should be in fullscreen mode.
}
