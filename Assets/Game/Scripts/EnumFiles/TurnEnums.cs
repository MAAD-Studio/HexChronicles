public class TurnEnums
{
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
        WorldTurn,
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
        None,
        Movement,
        BasicAttack,
        ActiveSkill
    }

    public enum CharacterType
    {
        Player,
        Enemy
    }
}
