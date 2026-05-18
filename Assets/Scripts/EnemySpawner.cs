using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnRadius = 12f;        // Distance from center to spawn
    public float minSpawnInterval = 0.5f;  // Fastest possible spawn rate
    public float startSpawnInterval = 3f;  // Starting spawn rate

    [Header("Difficulty Scaling")]
    public int scoreThresholdPerLevel = 50; // Every 50 score = 1 level up
    public float spawnIntervalReduction = 0.2f;
    public float enemySpeedIncrease = 0.3f;
    public float baseEnemySpeed = 2f;
    public int maxEnemiesOnScreen = 30;

    private float currentSpawnInterval;
    private float spawnTimer;
    private int currentDifficultyLevel = 0;
    private int enemiesOnScreen = 0;

    void Start()
    {
        currentSpawnInterval = startSpawnInterval;
        spawnTimer = currentSpawnInterval;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        // Update difficulty based on score
        UpdateDifficulty();

        // Spawn timer
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            if (enemiesOnScreen < maxEnemiesOnScreen)
                SpawnEnemy();
            spawnTimer = currentSpawnInterval;
        }
    }

    void UpdateDifficulty()
    {
        if (GameManager.Instance == null) return;

        int newLevel = GameManager.Instance.score / scoreThresholdPerLevel;
        if (newLevel != currentDifficultyLevel)
        {
            currentDifficultyLevel = newLevel;
            // Decrease spawn interval (more frequent), capped at minimum
            currentSpawnInterval = Mathf.Max(
                minSpawnInterval,
                startSpawnInterval - (currentDifficultyLevel * spawnIntervalReduction)
            );
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // Pick random angle around the player/center
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 spawnPos = new Vector2(
            Mathf.Cos(angle) * spawnRadius,
            Mathf.Sin(angle) * spawnRadius
        );

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        // Scale speed with difficulty
        if (enemy != null)
        {
            float scaledSpeed = baseEnemySpeed + (currentDifficultyLevel * enemySpeedIncrease);
            enemy.SetSpeed(scaledSpeed);
        }

        enemiesOnScreen++;

        // Track when enemy is destroyed
        EnemyTracker tracker = enemyObj.AddComponent<EnemyTracker>();
        tracker.spawner = this;
    }

    public void EnemyDestroyed()
    {
        enemiesOnScreen = Mathf.Max(0, enemiesOnScreen - 1);
    }

    // Returns current difficulty level for other scripts to read
    public int GetDifficultyLevel()
    {
        return currentDifficultyLevel;
    }
}

// Helper component to notify spawner when enemy is destroyed
public class EnemyTracker : MonoBehaviour
{
    public EnemySpawner spawner;

    void OnDestroy()
    {
        if (spawner != null)
            spawner.EnemyDestroyed();
    }
}
