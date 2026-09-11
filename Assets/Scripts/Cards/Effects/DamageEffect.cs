using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Cards/Effects/Damage")]
public class DamageEffect : CardEffect
{
    [SerializeField] private int damageAmount;

    public int DamageAmount => damageAmount;

    public override void Execute(CardEffectContext context)
    {
        if (Target == TargetType.Enemy)
        {
            context.Enemy.TakeDamage(damageAmount);
            //test
            Debug.Log("Enemy HP: " + context.Enemy.GetCurrentHP());
        }

        if (Target == TargetType.Player)
        {
            context.Player.TakeDamage(damageAmount);
        }
    }
}