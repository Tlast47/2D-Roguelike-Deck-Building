using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private CardData cardData;

    public CardData CardData => cardData;

    public string CardName => cardData.CardName;
    public string Description => cardData.Description;

    public CardType CardType => cardData.CardType;
    public CardRarity Rarity => cardData.Rarity;

    public int EnergyCost => cardData.EnergyCost;
    //public int Damage => cardData.Damage;
    //public int Block => cardData.Block;

    public void SetCardData(CardData newCardData)
    {
        cardData = newCardData;

        //Debug.Log("Set Card : " + cardData.CardName);

        RefreshUI();
    }

    private void RefreshUI()
    {

    }
}
