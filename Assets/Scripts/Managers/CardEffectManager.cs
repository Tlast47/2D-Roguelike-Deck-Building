using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    public void Resolve(CardData card, PlayerData player, EnemyData enemy)
    {
        CardEffectContext context = new CardEffectContext(player, enemy);

        foreach (CardEffect effect in card.Effects)
        {
            ExecuteEffect(effect, context);
        }
    }

    private void ExecuteEffect(CardEffect effect,CardEffectContext context)
    {
        effect.Execute(context);
    }
}