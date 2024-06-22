public class TurnEnums
{
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
        WorldTurn,
        WeatherTurn
    }

    public enum WorldTurns
    {
        Outpost,
        RefugeeConvoy,
        Towers,
        NightSurvival
    }

    public enum PlayerAction
    {
        BasicAttack,
        ActiveSkill
    }

    public enum PlayerPhase
    {
        Movement,
        Attack,
        Execution
    }

    public enum CharacterType
    {
        Player,
        Enemy
    }
}
