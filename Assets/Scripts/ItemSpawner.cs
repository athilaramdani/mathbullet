using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject mathAmmoPrefab;
    public GameObject[] powerUpPrefabs; // Each prefab has PowerUp component with different type

    [Header("Spawn Area")]
    public float spawnAreaHalfWidth = 8f;
    public float spawnAreaHalfHeight = 5f;

    [Header("Math Ammo Spawning")]
    public float mathAmmoSpawnInterval = 10f;
    public int maxMathAmmoOnScreen = 3;

    [Header("PowerUp Spawning")]
    public float powerUpSpawnInterval = 15f;
    public int maxPowerUpsOnScreen = 2;

    private float mathAmmoTimer;
    private float powerUpTimer;
    private int mathAmmoCount = 0;
    private int powerUpCount = 0;

    void Start()
    {
        mathAmmoTimer = mathAmmoSpawnInterval * 0.5f; // Spawn first ammo sooner
        powerUpTimer = powerUpSpawnInterval;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        // Math Ammo spawn
        mathAmmoTimer -= Time.deltaTime;
        if (mathAmmoTimer <= 0f)
        {
            if (mathAmmoCount < maxMathAmmoOnScreen)
                SpawnMathAmmo();
            mathAmmoTimer = mathAmmoSpawnInterval;
        }

        // PowerUp spawn
        powerUpTimer -= Time.deltaTime;
        if (powerUpTimer <= 0f)
        {
            if (powerUpCount < maxPowerUpsOnScreen)
                SpawnPowerUp();
            powerUpTimer = powerUpSpawnInterval;
        }
    }

    void SpawnMathAmmo()
    {
        if (mathAmmoPrefab == null) return;
        Vector2 pos = GetRandomPosition();
        GameObject obj = Instantiate(mathAmmoPrefab, pos, Quaternion.identity);
        mathAmmoCount++;

        // Track when destroyed
        ItemTracker tracker = obj.AddComponent<ItemTracker>();
        tracker.onDestroyed = () => mathAmmoCount = Mathf.Max(0, mathAmmoCount - 1);
    }

    void SpawnPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        // Pick random powerup prefab
        int idx = Random.Range(0, powerUpPrefabs.Length);
        GameObject prefab = powerUpPrefabs[idx];
        if (prefab == null) return;

        Vector2 pos = GetRandomPosition();
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        powerUpCount++;

        ItemTracker tracker = obj.AddComponent<ItemTracker>();
        tracker.onDestroyed = () => powerUpCount = Mathf.Max(0, powerUpCount - 1);
    }

    Vector2 GetRandomPosition()
    {
        float x = Random.Range(-spawnAreaHalfWidth, spawnAreaHalfWidth);
        float y = Random.Range(-spawnAreaHalfHeight, spawnAreaHalfHeight);
        return new Vector2(x, y);
    }
}

// Lightweight tracker using Action callback
public class ItemTracker : MonoBehaviour
{
    public System.Action onDestroyed;

    void OnDestroy()
    {
        onDestroyed?.Invoke();
    }
}
