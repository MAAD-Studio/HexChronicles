public struct PauseGame { }
public struct OnNewLevelStart { }
public struct OnPlayerTurn { }
public struct OnEnemyTurn { }
public struct OnWeatherSpawn { }
public struct OnWeatherEnded { }

public struct CharacterSelected
{
    public Character character;
}

public struct UpdateCharacterDecision
{
    public Character character;
    public bool hasMadeDecision;
}

public struct OnRestoreCharacterData
{
    public Character character;
}

#region Setting changed events
public struct AudioSettingsChangedEvent
{
    public float MasterVolume; ///< The master volume level, typically between 0.0 (mute) and 1.0 (max volume).
    public float MusicVolume;  ///< The music volume level, typically between 0.0 (mute) and 1.0 (max volume).
    public float SfxVolume;    ///< The SFX (Sound Effects) volume level, typically between 0.0 (mute) and 1.0 (max volume).
}

public struct ResolutionChangedEvent
{
    public int Width;         ///< The width of the resolution.
    public int Height;        ///< The height of the resolution.
    public bool IsFullScreen; ///< Indicates whether the game should be in fullscreen mode.
}
#endregion
