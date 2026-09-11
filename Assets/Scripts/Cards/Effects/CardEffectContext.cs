public class CardEffectContext
{
    public PlayerData Player;
    public EnemyData Enemy;

    public CardEffectContext(PlayerData player, EnemyData enemy)
    {
        Player = player;
        Enemy = enemy;
    }
}