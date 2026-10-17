using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    [Header("Reward UI")]
    [SerializeField] private TMP_Text goldRewardText;
    [Header("Card Reward")]
    [SerializeField] private CardData[] rewardCardPool;
    [SerializeField] private int rewardCardCount = 3;
    [Header("Card Reward UI")]
    [SerializeField] private GameObject rewardCardPrefab;
    [SerializeField] private Transform cardRewardContainer;
    private RunManager runManager;
    private BattleRewardData reward;
    private Button claimRelicButton;
    private RunData Run => runManager != null ? runManager.CurrentRun : null;

    private void Start()
    {
        runManager = FindFirstObjectByType<RunManager>();
        if (Run == null || Run.CurrentNode == null)
        {
            Debug.LogWarning("RewardManager: start a run and win a battle before opening rewards.");
            return;
        }
        MapNodeType type = Run.CurrentNode.NodeType;
        if (type != MapNodeType.NormalBattle && type != MapNodeType.Elite && type != MapNodeType.Boss) return;
        reward = Run.GetReward(Run.CurrentNode);
        if (reward == null)
        {
            reward = GenerateReward(type);
            Run.SetReward(Run.CurrentNode, reward);
        }
        if (!reward.GoldClaimed)
        {
            reward.GoldClaimed = true;
            Run.AddGold(reward.Gold);
        }
        if (goldRewardText != null) goldRewardText.text = "+" + reward.Gold + " Gold";
        BuildCardRewardUI();
        BuildRelicRewardUI();
    }

    private BattleRewardData GenerateReward(MapNodeType type)
    {
        BattleRewardData result = new BattleRewardData();
        result.Gold = EconomyBalance.BattleGold(type) + Run.Relics.Total(RelicEffect.VictoryGold);
        List<CardData> available = new List<CardData>();
        if (rewardCardPool != null)
            foreach (CardData card in rewardCardPool)
                if (card != null && card.AvailableInCardPools && !card.IsUpgraded && !available.Contains(card)) available.Add(card);
        int count = Mathf.Min(Mathf.Max(0, rewardCardCount), available.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, available.Count);
            result.Cards.Add(available[index]);
            available.RemoveAt(index);
        }
        List<RelicData> relics = RelicCatalog.Available(Run.Relics);
        float dropChance = EconomyBalance.RelicDropChance(type);
        if (relics.Count > 0 && (dropChance >= 1f || Random.value < dropChance))
            result.Relic = relics[Random.Range(0, relics.Count)];
        return result;
    }

    private void BuildCardRewardUI()
    {
        if (rewardCardPrefab == null || cardRewardContainer == null)
        {
            Debug.LogError("RewardManager: card reward prefab/container is missing.");
            return;
        }
        foreach (Transform child in cardRewardContainer)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        for (int i = 0; i < reward.Cards.Count; i++)
        {
            RewardCardUI view = Instantiate(rewardCardPrefab, cardRewardContainer).GetComponent<RewardCardUI>();
            if (view == null) continue;
            view.Setup(reward.Cards[i], i, this);
            view.SetInteractable(!reward.CardClaimed);
        }
    }

    public void SelectCardReward(int index)
    {
        if (Run == null || reward == null || reward.CardClaimed || index < 0 || index >= reward.Cards.Count) return;
        CardData card = reward.Cards[index];
        if (card == null) return;
        reward.CardClaimed = true;
        Run.Deck.AddCard(card);
        if (cardRewardContainer != null)
            foreach (RewardCardUI view in cardRewardContainer.GetComponentsInChildren<RewardCardUI>()) view.SetInteractable(false);
    }

    private void BuildRelicRewardUI()
    {
        Canvas sceneCanvas = cardRewardContainer != null ? cardRewardContainer.GetComponentInParent<Canvas>() : null;
        if (sceneCanvas == null) return;
        Transform canvas = sceneCanvas.rootCanvas.transform;
        if (reward.Relic == null)
        {
            TMP_Text noRelic = EconomyUI.Label("No Relic Reward", canvas,
                Run.Relics.Count >= RelicCatalog.Available(null).Count ? "All relics collected." : "No relic found this battle.",
                0.2f, 0.15f, 0.8f, 0.20f, 20f);
            noRelic.alignment = TextAlignmentOptions.Center;
            return;
        }
        EconomyUI.SetRect(cardRewardContainer as RectTransform, 0.10f, 0.34f, 0.90f, 0.72f);
        RectTransform panel = EconomyUI.Panel("Relic Reward", canvas, 0.20f, 0.15f, 0.80f, 0.30f, EconomyUI.PanelColor);
        EconomyUI.Label("Name", panel, "RELIC: " + reward.Relic.RelicName, 0.04f, 0.60f, 0.70f, 0.94f, 25f).color =
            new Color(1f, 0.85f, 0.40f);
        EconomyUI.Label("Description", panel, reward.Relic.Description, 0.04f, 0.08f, 0.70f, 0.59f, 22f);
        claimRelicButton = EconomyUI.Button("Claim Relic", panel, "Claim relic", 0.74f, 0.24f, 0.96f, 0.76f, ClaimRelic);
        RefreshRelicButton();
    }

    private void ClaimRelic()
    {
        if (Run == null || reward == null || reward.Relic == null || reward.RelicClaimed) return;
        if (!Run.Relics.TryAdd(reward.Relic)) return;
        reward.RelicClaimed = true;
        RefreshRelicButton();
    }

    private void RefreshRelicButton()
    {
        bool owned = Run.Relics.Contains(reward.Relic);
        claimRelicButton.interactable = !reward.RelicClaimed && !owned;
        EconomyUI.ButtonText(claimRelicButton, reward.RelicClaimed ? "Collected" : owned ? "Owned" : "Claim relic");
    }

    public void LeaveReward() { if (Run != null) runManager.ReturnToMap(); }
}

