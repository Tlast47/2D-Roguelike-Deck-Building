using System.Collections.Generic;
using UnityEngine;

// Shares the same targeting rules as DamageEffect and BlockEffect.
public abstract class StatusCardEffect : CardEffect
{
    protected abstract int StackAmount { get; }
    protected abstract void ApplyStatus(CharacterData target, int amount);

    public override void Execute(CardEffectContext context)
    {
        if (context == null || StackAmount <= 0)
        {
            return;
        }

        switch (Target)
        {
            case TargetType.Player:
                ApplyTo(context.Player);
                break;
            case TargetType.SingleEnemy:
                ApplyTo(context.SelectedEnemy);
                break;
            case TargetType.AllEnemies:
                if (context.Enemies == null) return;
                foreach (EnemyData enemy in context.Enemies)
                {
                    ApplyTo(enemy);
                }
                break;
            case TargetType.RandomEnemy:
                if (context.Enemies == null) return;
                List<EnemyData> availableEnemies = new List<EnemyData>();
                foreach (EnemyData enemy in context.Enemies)
                {
                    if (enemy != null && !enemy.IsDead())
                    {
                        availableEnemies.Add(enemy);
                    }
                }
                if (availableEnemies.Count > 0)
                {
                    ApplyTo(availableEnemies[Random.Range(0, availableEnemies.Count)]);
                }
                break;
        }
    }

    private void ApplyTo(CharacterData target)
    {
        if (target != null && !target.IsDead())
        {
            ApplyStatus(target, StackAmount);
        }
    }
}
