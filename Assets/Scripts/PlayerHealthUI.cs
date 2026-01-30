using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMP_Text hpText;

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.AddListener(UpdateHP);
            UpdateHP(playerHealth.CurrentHP, playerHealth.MaxHP);
        }
    }

    private void UpdateHP(int current, int max)
    {
        if (hpText != null)
            hpText.text = $"HP: {current}";
    }
}
