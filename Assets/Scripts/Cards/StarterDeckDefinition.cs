using UnityEngine;

[CreateAssetMenu(fileName = "StarterDeck", menuName = "Cards/Starter Deck Definition")]
public class StarterDeckDefinition : ScriptableObject
{
    [SerializeField] private CardData strikeCard;
    [SerializeField] private CardData heavySlashCard;
    [SerializeField] private CardData defendCard;

    public CardData StrikeCard => strikeCard;
    public CardData HeavySlashCard => heavySlashCard;
    public CardData DefendCard => defendCard;
}
