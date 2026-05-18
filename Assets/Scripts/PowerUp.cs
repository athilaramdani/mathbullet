using UnityEngine;
using TMPro;

public enum PowerUpType
{
    Heal,
    FreezeEnemies,
    Shield,
    SpeedBoost
}

public class PowerUp : MonoBehaviour
{
    [Header("PowerUp Type")]
    public PowerUpType powerUpType = PowerUpType.Heal;

    [Header("Effect Values")]
    public int healAmount = 1;
    public float freezeDuration = 4f;
    public float shieldDuration = 5f;
    public float speedBoostDuration = 5f;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    [Header("Floating Label")]
    public TextMeshProUGUI floatingLabel; // Optional screen-space label

    // Colors for each type
    private static Color healColor      = new Color(1f, 0.3f, 0.4f);   // Red/pink
    private static Color freezeColor    = new Color(0.3f, 0.8f, 1f);   // Cyan
    private static Color shieldColor    = new Color(1f, 0.85f, 0f);    // Gold
    private static Color speedColor     = new Color(0.5f, 1f, 0.2f);   // Lime

    void Start()
    {
        SetVisual();
    }


    void SetVisual()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        string label = "";
        Color color = Color.white;

        switch (powerUpType)
        {
            case PowerUpType.Heal:
                color = healColor;
                label = "❤ HEAL";
                break;
            case PowerUpType.FreezeEnemies:
                color = freezeColor;
                label = "❄ FREEZE";
                break;
            case PowerUpType.Shield:
                color = shieldColor;
                label = "🛡 SHIELD";
                break;
            case PowerUpType.SpeedBoost:
                color = speedColor;
                label = "⚡ SPEED";
                break;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = color;

        if (floatingLabel != null)
        {
            floatingLabel.text = label;
            floatingLabel.color = color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ApplyEffect(other.gameObject);
        Destroy(gameObject);
    }

    void ApplyEffect(GameObject player)
    {
        switch (powerUpType)
        {
            case PowerUpType.Heal:
                if (GameManager.Instance != null)
                    GameManager.Instance.Heal(healAmount);
                break;

            case PowerUpType.FreezeEnemies:
                FreezeAllEnemies();
                break;

            case PowerUpType.Shield:
                PlayerShooting shooting = player.GetComponent<PlayerShooting>();
                if (shooting != null)
                    shooting.ActivateShield(shieldDuration);
                break;

            case PowerUpType.SpeedBoost:
                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                    movement.ActivateSpeedBoost(speedBoostDuration);
                break;
        }
    }

    void FreezeAllEnemies()
    {
        SoundManager.Instance?.PlayFreeze();
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
            enemy.Freeze(freezeDuration);
    }
}
