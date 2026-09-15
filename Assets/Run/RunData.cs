public class RunData
{
    public int Gold { get; private set; }

    public MapData CurrentMap { get; private set; }

    public MapNode CurrentNode { get; private set; }

    public bool HasStarted => CurrentNode != null;

    public RunState State { get; private set; }

    public RunData()
    {
        Gold = 0;
        CurrentMap = null;
        CurrentNode = null;
        State = RunState.NotStarted;
    }

    public void SetMap(MapData map)
    {
        if (map == null)
        {
            return;
        }

        CurrentMap = map;
    }

    public void SetCurrentNode(MapNode node)
    {
        if (node == null)
        {
            return;
        }

        CurrentNode = node;
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
        if (amount <= 0)
        {
            return;
        }

        Gold += amount;
    }
}