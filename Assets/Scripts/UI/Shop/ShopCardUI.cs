using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    private CardData cardData;
    private ShopManager shopManager;


    public void Setup(CardData card, int price, ShopManager manager)
    {
        cardData = card;
        shopManager = manager;

        if (cardData == null)
        {
            Debug.LogError("ShopCardUI : CardData is null");
            return;
        }

        if (cardNameText != null)
        {
            cardNameText.text = cardData.CardName;
            cardNameText.color = cardData.IsUpgraded ? new Color(1f, 0.86f, 0.42f) : Color.white;
        }

        if (descriptionText != null)
            descriptionText.text = cardData.Description;

        if (priceText != null)
            priceText.text = price + " Gold";

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }

        CardVisualStyle.Apply((RectTransform)transform, cardData, cardNameText, descriptionText,
            priceText, price + " Gold", buyButton, background: CardVisualStyle.LightBackground);
        RefreshBuyButton();
    }


    public void RefreshBuyButton()
    {
        if (buyButton == null)
            return;

        if (shopManager == null || cardData == null)
        {
            buyButton.interactable = false;
            return;
        }

        buyButton.interactable = shopManager.CanBuyCard(cardData);
        TMP_Text buttonLabel = buyButton.GetComponentInChildren<TMP_Text>();
        if (buttonLabel != null) buttonLabel.text = shopManager.IsCardSold(cardData) ? "SOLD" : "BUY";
    }


    private void OnBuyButtonClicked()
    {
        if (shopManager == null || cardData == null)
            return;

        bool bought = shopManager.BuyCard(cardData);

        if (bought)
        {
            RefreshBuyButton();
        }
    }
}


