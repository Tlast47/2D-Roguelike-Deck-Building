using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardTileView : MonoBehaviour
{
    private Outline border;
    private Button actionButton;
    private string normalAction;
    public void Initialize(CardData card, string action, UnityAction onSelect)
    {
        RectTransform rect = transform as RectTransform;
        Image background = gameObject.AddComponent<Image>();
        border = gameObject.AddComponent<Outline>();
        border.effectDistance = new Vector2(2f, -2f);
        TMP_Text title = EconomyUI.Label("Name", rect, card.CardName, 0.08f, 0.75f, 0.92f, 0.96f, 24f);
        TMP_Text description = EconomyUI.Label("Description", rect, card.Description, 0.08f, 0.31f, 0.92f, 0.71f, 22f);
        TMP_Text energy = EconomyUI.Label("Energy", rect, "", 0.08f, 0.18f, 0.92f, 0.28f, 24f);
        normalAction = action;
        if (onSelect != null)
        {
            Button body = gameObject.AddComponent<Button>();
            body.targetGraphic = background;
            body.onClick.AddListener(onSelect);
            actionButton = EconomyUI.Button("Select", rect, action, 0.08f, 0.04f, 0.92f, 0.16f, onSelect);
        }
        CardVisualStyle.Apply(rect, card, title, description, energy, "Energy: " + card.EnergyCost, actionButton);
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (border != null)
        {
            border.enabled = selected;
            border.effectColor = new Color(1f, 0.82f, 0.26f);
        }
        if (actionButton != null) EconomyUI.ButtonText(actionButton, selected ? "Selected" : normalAction);
    }
}
