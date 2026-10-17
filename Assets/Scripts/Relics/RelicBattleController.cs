using System.Collections.Generic;

// One controller per battle; opening bonuses survive the first resource reset exactly once.
public class RelicBattleController
{
    private readonly RelicCollection relics;
    private readonly PlayerData player;
    private readonly DeckManager deck;
    private readonly IReadOnlyList<EnemyData> enemies;
    private bool firstTurn = true;
    private bool victoryApplied;

    public RelicBattleController(RelicCollection relics, PlayerData player,
        DeckManager deck, IReadOnlyList<EnemyData> enemies)
    {
        this.relics = relics; this.player = player; this.deck = deck; this.enemies = enemies;
    }

    public void StartBattle()
    {
        if (relics == null) return;
        deck.DrawAdditionalCards(relics.Total(RelicEffect.BattleStartDraw));
        foreach (EnemyData enemy in enemies)
        {
            if (enemy == null || enemy.IsDead()) continue;
            enemy.AddBurn(relics.Total(RelicEffect.BattleStartBurn));
            enemy.AddPoison(relics.Total(RelicEffect.BattleStartPoison));
            enemy.TakeDamage(relics.Total(RelicEffect.BattleStartDamage));
        }
    }

    public void StartPlayerTurn()
    {
        if (relics == null) return;
        player.AddBlock(relics.Total(RelicEffect.TurnStartBlock));
        if (firstTurn)
        {
            player.AddBlock(relics.Total(RelicEffect.BattleStartBlock));
            player.GainEnergy(relics.Total(RelicEffect.BattleStartEnergy));
            firstTurn = false;
        }
    }

    public void Victory()
    {
        if (victoryApplied || relics == null) return;
        victoryApplied = true;
        player.Heal(relics.Total(RelicEffect.VictoryHeal));
    }
}
