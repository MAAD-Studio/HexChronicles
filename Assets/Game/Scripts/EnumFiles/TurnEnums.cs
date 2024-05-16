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

    public enum PathfinderTypes
    {
        Movement,
        BasicAttack,
        EnemyBasicAttack,
        EnemyMovement,
        ActiveSkill
    }

    public enum CharacterType
    {
        Player,
        Enemy
    }
}