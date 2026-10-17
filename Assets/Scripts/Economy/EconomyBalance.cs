using UnityEngine;

public static class EconomyBalance
{
    public const int RemovalBasePrice = 50;
    public const int RemovalPriceIncrease = 20;
    public const int RelicsPerShop = 2;

    public static int CardPrice(CardData card)
    {
        if (card == null) return 0;
        int price = card.BaseShopPrice > 0 ? card.BaseShopPrice :
            35 + (int)card.Rarity * 30 + Mathf.Max(0, card.EnergyCost) * 5 + (card.IsUpgraded ? 25 : 0);
        // Stable once stored in the room's offer; nearby shops can charge slightly different prices.
        return Mathf.Max(10, Mathf.RoundToInt(price * Random.Range(0.9f, 1.1001f) / 5f) * 5);
    }

    public static int BattleGold(MapNodeType type)
    {
        return type == MapNodeType.Boss ? 80 : type == MapNodeType.Elite ? 45 : 25;
    }

    public static float RelicDropChance(MapNodeType type)
    {
        return type == MapNodeType.Boss ? 1f : type == MapNodeType.Elite ? 0.75f : 0.25f;
    }
}
