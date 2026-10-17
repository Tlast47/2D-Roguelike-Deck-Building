using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]

    [SerializeField] private string cardName;
    [SerializeField] private string description;

    [SerializeField] private CardType cardType;
    [SerializeField] private CardRarity rarity;
    [SerializeField] private bool availableInCardPools = true;

    [Header("Shop")]
    [Min(0)] [SerializeField] private int baseShopPrice;
    [SerializeField] private bool isUpgraded;
    public int BaseShopPrice => baseShopPrice;
    public bool IsUpgraded => isUpgraded;

    [Header("Card Stats")]
    [SerializeField] private int energyCost;
    
    [Header("Card Effects")]

    [SerializeField] private List<CardEffect> effects;

    [Header("Upgrade")]
    [SerializeField] private CardData upgradedCard;

    public string CardName => cardName;
    public string Description => description;
    
    public CardType CardType => cardType;
    public CardRarity Rarity => rarity;
    public bool AvailableInCardPools => availableInCardPools;
    
    public int EnergyCost => energyCost;

    public List<CardEffect> Effects => effects;

    public CardData UpgradedCard => upgradedCard;

}

