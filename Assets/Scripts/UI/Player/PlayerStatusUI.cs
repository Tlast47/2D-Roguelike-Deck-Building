using TMPro;
using UnityEngine;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerData player;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI blockText;
    [SerializeField] private TextMeshProUGUI energyText;

    private void Start()
    {
        Refresh();
    }

    private void LateUpdate()
    {
        if (player != null) Refresh();
    }

    public void Refresh()
    {
        if (player == null)
        {
            Debug.LogError(
                "PlayerStatusUI : Player is null"
            );

            return;
        }

        if (hpText != null)
        {
            hpText.text =
                "HP: " +
                player.GetCurrentHP() +
                " / " +
                player.GetMaxHP();
        }

        CharacterStatusDisplay.Refresh(player, hpText, blockText, energyText);

        if (energyText != null)
        {
            energyText.text =
                "Energy: " +
                player.CurrentEnergy;
        }
    }
}