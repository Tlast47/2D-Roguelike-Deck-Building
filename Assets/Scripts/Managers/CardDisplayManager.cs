using UnityEngine;

public class CardDisplayManager : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Transform handArea;

    // private void Start()
    //     {
    //         RefreshHand();
    //     }

    public void RefreshHand()
    {

        //Debug.Log("Refresh Hand");

        //Debug.Log("Hand Count : " + deckManager.Hand.Count);

        foreach (Transform child in handArea)
        {
            Destroy(child.gameObject);
        }

        foreach (CardData cardData in deckManager.Hand)
        {
            //Debug.Log("Loop : " + cardData.CardName);

            Card card = Instantiate(cardPrefab, handArea);

            card.SetCardData(cardData);
        }

    }
}