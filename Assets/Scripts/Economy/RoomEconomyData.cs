using System.Collections.Generic;

// Cached by room in RunData, so reloading a scene does not reroll or duplicate purchases/rewards.
public class ShopCardOffer
{
    public CardData Card;
    public int Price;
    public bool Sold;
}

public class ShopRelicOffer
{
    public RelicData Relic;
    public int Price;
    public bool Sold;
}

public class ShopInventory
{
    public readonly List<ShopCardOffer> Cards = new List<ShopCardOffer>();
    public readonly List<ShopRelicOffer> Relics = new List<ShopRelicOffer>();
    public bool RemovalUsed;
}

public class BattleRewardData
{
    public readonly List<CardData> Cards = new List<CardData>();
    public int Gold;
    public bool GoldClaimed;
    public bool CardClaimed;
    public RelicData Relic;
    public bool RelicClaimed;
}
