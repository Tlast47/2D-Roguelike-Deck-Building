using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestManager : MonoBehaviour
{
    [Header("Rest UI")]
    [SerializeField] private Button healButton;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private GameObject restPanel;
    [Header("Upgrade UI")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button upgradeButton;
    private RunManager runManager;
    private RestUpgradeUI upgradeUI;
    private bool restActionUsed;
    private RunData Run => runManager != null ? runManager.CurrentRun : null;

    private void Start()
    {
        runManager = FindFirstObjectByType<RunManager>();
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (restPanel != null) restPanel.SetActive(true);
        RefreshHPText();
    }

    public void HealPlayer()
    {
        if (restActionUsed || Run?.Player == null) return;
        Run.Player.Heal(Run.Player.MaxHP / 3);
        restActionUsed = true;
        RefreshHPText();
        DisableRestActions();
    }

    private void RefreshHPText()
    {
        if (hpText != null && Run?.Player != null)
            hpText.text = "HP: " + Run.Player.CurrentHP + " / " + Run.Player.MaxHP;
    }

    public void OpenUpgradePanel()
    {
        if (restActionUsed || Run?.Deck == null) return;
        if (upgradeUI == null) upgradeUI = gameObject.AddComponent<RestUpgradeUI>();
        Canvas canvas = restPanel != null ? restPanel.GetComponentInParent<Canvas>() : null;
        if (canvas == null) return;
        upgradeUI.Open(this, Run.Deck, canvas.rootCanvas.transform);
    }

    // Keep the original UI entry point compatible with existing card prefabs.
    public void SelectCardForUpgrade(int index)
    {
        TryUpgradeCard(index, Run?.Deck.GetCard(index));
    }

    public bool TryUpgradeCard(int index, CardData expectedCard)
    {
        if (restActionUsed || Run?.Deck == null || expectedCard == null ||
            Run.Deck.GetCard(index) != expectedCard || expectedCard.UpgradedCard == null) return false;
        if (!Run.Deck.UpgradeCard(index)) return false;
        restActionUsed = true;
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (restPanel != null) restPanel.SetActive(true);
        DisableRestActions();
        return true;
    }

    private void DisableRestActions()
    {
        if (healButton != null) healButton.interactable = false;
        if (upgradeButton != null) upgradeButton.interactable = false;
    }

    public void LeaveRest() { if (Run != null) runManager.ReturnToMap(); }
}
