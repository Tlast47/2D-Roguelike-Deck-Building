using System.Collections.Generic;

public class RunData
{
    public int Gold { get; private set; }
    public RelicCollection Relics { get; private set; }
    public int CardsRemoved { get; private set; }
    private readonly Dictionary<MapNode, ShopInventory> shops = new Dictionary<MapNode, ShopInventory>();
    private readonly Dictionary<MapNode, BattleRewardData> rewards = new Dictionary<MapNode, BattleRewardData>();
    public ShopInventory GetShop(MapNode node) => node != null && shops.TryGetValue(node, out var shop) ? shop : null;
    public void SetShop(MapNode node, ShopInventory shop) { if (node != null) shops[node] = shop; }
    public BattleRewardData GetReward(MapNode node) => node != null && rewards.TryGetValue(node, out var reward) ? reward : null;
    public void SetReward(MapNode node, BattleRewardData reward) { if (node != null) rewards[node] = reward; }
    public void RecordCardRemoval() { CardsRemoved++; }

    public MapNode CurrentNode { get; private set; }

    public bool HasStarted => CurrentNode != null;

    public RunState State { get; private set; }

    public MapData CurrentMap { get; private set; }

    public PlayerDeck Deck { get; private set; }

    public PlayerRunData Player { get; private set; }

    public RunData()
    {
        Gold = 0;
        CurrentNode = null;
        State = RunState.NotStarted;
        CurrentMap = null;

        Deck = new PlayerDeck();
        Player = new PlayerRunData();
        Relics = new RelicCollection(Player);
    }


    public void SetCurrentNode(MapNode node)
    {
        if (node == null) return;

        CurrentNode = node;
    }


    public void SetMap(MapData map)
    {
        if (map == null) return;

        CurrentMap = map;
    }


    public void StartRun()
    {
        State = RunState.Playing;
    }


    public void CompleteRun()
    {
        State = RunState.Completed;
    }


    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        Gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0)
            return false;
    
        if (Gold < amount)
            return false;
    
        Gold -= amount;
    
        return true;
    }
}
