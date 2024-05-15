public class TurnEnums
{
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
        WorldTurn
    }

    public enum WorldTurns
    {
        Outpost,
        RefugeeConvoy,
        Towers,
        NightSurvival
    }

    public enum PathfinderTypes
    {
        Movement,
        Attack,
        Skill
    }

    public enum CharacterType
    {
        Player,
        Enemy
    }
}
