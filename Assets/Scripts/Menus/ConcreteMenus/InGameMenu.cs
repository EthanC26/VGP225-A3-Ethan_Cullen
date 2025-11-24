using TMPro;

public class InGameMenu : BaseMenu
{
    public TMP_Text livesText;
    PlayerController player;

    public override void Init(MenuController contex)
    {
        base.Init(contex);
        state = MenuStates.InGame;

        if (player == null)
           player = FindFirstObjectByType<PlayerController>();

        // Subscribe to player's health changes
        player.OnHealthChanged += LifeValueChanged;

        // Initialize text
        UpdateLiveText();
    }

    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        if (player != null)
            player.OnHealthChanged -= LifeValueChanged;
    }

    private void LifeValueChanged(int value)
    {
        livesText.text = $"Lives: {value}";
    }

    private void UpdateLiveText()
    {
        livesText.text = "Lives: " + player.currentHealth.ToString();
    }
}
