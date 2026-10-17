using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunInventoryHUD : MonoBehaviour
{
    private RunManager manager;
    private TMP_Text goldText;
    private Button relicButton;
    private RectTransform overlay;
    public bool IsRelicPanelOpen => overlay != null && overlay.gameObject.activeInHierarchy;
    private int previousGold = -1;
    private int previousRelics = -1;

    public static void Attach(RunManager runManager)
    {
        if (runManager == null || runManager.CurrentRun == null) return;
        foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (!canvas.isRootCanvas || canvas.renderMode == RenderMode.WorldSpace ||
                canvas.gameObject.scene.name == "MainMenu") continue;
            RunInventoryHUD existing = canvas.GetComponentInChildren<RunInventoryHUD>(true);
            if (existing != null) { existing.manager = runManager; return; }
            RectTransform root = EconomyUI.Rect("Run Inventory HUD", canvas.transform, 0f, 0f, 1f, 1f);
            RunInventoryHUD hud = root.gameObject.AddComponent<RunInventoryHUD>();
            hud.manager = runManager;
            bool battle = canvas.gameObject.scene.name == "BattleScene";
            bool shop = canvas.gameObject.scene.name == "ShopScene";
            hud.goldText = EconomyUI.Label("Gold", root, "", battle ? 0.13f : shop ? 0.14f : 0.04f,
                battle ? 0.92f : 0.95f, 0.35f, battle ? 0.97f : 0.995f, 22f);
            // The shop shows a larger balance beside its title.
            hud.goldText.gameObject.SetActive(!shop);
            hud.relicButton = EconomyUI.Button("Owned Relics", root, "", battle ? 0.70f : 0.76f,
                battle ? 0.92f : 0.95f, battle ? 0.87f : 0.96f, battle ? 0.97f : 0.995f, hud.OpenRelics);
            return;
        }
    }

    private void Update()
    {
        RunData run = manager != null ? manager.CurrentRun : null;
        if (run == null || goldText == null || relicButton == null) return;
        if (run.Gold == previousGold && run.Relics.Count == previousRelics) return;
        previousGold = run.Gold;
        previousRelics = run.Relics.Count;
        goldText.text = "Gold: " + run.Gold;
        EconomyUI.ButtonText(relicButton, "Relics: " + run.Relics.Count);
    }

    private void OpenRelics()
    {
        if (overlay != null || manager == null || manager.CurrentRun == null) return;
        overlay = EconomyUI.Modal(transform.parent, "Your relics", out RectTransform panel);
        RectTransform content = EconomyUI.Scroll(panel, 0.05f, 0.16f, 0.95f, 0.86f);
        foreach (RelicData relic in manager.CurrentRun.Relics.Owned)
        {
            RectTransform row = EconomyUI.ListRow(content, relic.RelicName);
            EconomyUI.Label("Name", row, relic.RelicName, 0.03f, 0.57f, 0.97f, 0.94f, 26f).color = new Color(1f, 0.85f, 0.40f);
            EconomyUI.Label("Description", row, relic.Description, 0.03f, 0.05f, 0.97f, 0.57f, 22f);
        }
        if (manager.CurrentRun.Relics.Count == 0)
        {
            RectTransform row = EconomyUI.ListRow(content, "Empty");
            EconomyUI.Label("Empty", row, "No relics yet. Find them after battles or buy them in shops.", 0.04f, 0.1f, 0.96f, 0.9f);
        }
        EconomyUI.Button("Close", panel, "Close", 0.35f, 0.04f, 0.65f, 0.12f, () =>
        {
            EconomyUI.Close(overlay);
            overlay = null;
        });
    }
}



