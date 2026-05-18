using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    public int maxHealth = 5;
    public int currentHealth;
    public int currentAmmo = 30;
    public int score = 0;
    public int highScore = 0;

    [Header("UI References (auto-found if empty)")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverHighScoreText;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // Auto-find UI if not assigned in Inspector
        AutoFindUI();

        currentHealth = maxHealth;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        isGameOver = false;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        UpdateUI();
    }

    void AutoFindUI()
    {
        if (healthText == null)    healthText    = FindTMP("HealthText");
        if (ammoText == null)      ammoText      = FindTMP("AmmoText");
        if (scoreText == null)     scoreText     = FindTMP("ScoreText");
        if (highScoreText == null) highScoreText = FindTMP("HighScore");

        if (gameOverPanel == null)
        {
            var go = GameObject.Find("GameOverPanel");
            if (go != null) gameOverPanel = go;
        }
        if (gameOverScoreText == null)    gameOverScoreText    = FindTMP("GOScore");
        if (gameOverHighScoreText == null) gameOverHighScoreText = FindTMP("GOHigh");

        // Wire GameOver buttons
        var restart = GameObject.Find("RestartButton");
        if (restart != null)
        {
            var btn = restart.GetComponent<Button>();
            if (btn != null) { btn.onClick.RemoveAllListeners(); btn.onClick.AddListener(RestartGame); }
        }
        var quit = GameObject.Find("QuitButton");
        if (quit != null)
        {
            var btn = quit.GetComponent<Button>();
            if (btn != null) { btn.onClick.RemoveAllListeners(); btn.onClick.AddListener(QuitGame); }
        }
    }

    TextMeshProUGUI FindTMP(string objName)
    {
        var go = GameObject.Find(objName);
        return go != null ? go.GetComponent<TextMeshProUGUI>() : null;
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        UpdateUI();
    }

    public void AddAmmo(int amount)
    {
        if (isGameOver) return;
        currentAmmo = Mathf.Max(0, currentAmmo + amount); // tidak bisa negatif
        UpdateUI();
    }

    public void UseAmmo()
    {
        if (isGameOver) return;
        currentAmmo = Mathf.Max(0, currentAmmo - 1);
        UpdateUI();
    }

    public bool HasAmmo() => currentAmmo > 0;

    public void TakeDamage(int amount)
    {
        if (isGameOver) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateUI();
        if (currentHealth <= 0) TriggerGameOver();
    }

    public void Heal(int amount)
    {
        if (isGameOver) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthText != null)    healthText.text    = "HP: " + currentHealth + " / " + maxHealth;
        if (ammoText != null)      ammoText.text      = "Ammo: " + currentAmmo;
        if (scoreText != null)     scoreText.text     = "Score: " + score;
        if (highScoreText != null) highScoreText.text = "Best: " + highScore;
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverScoreText != null)     gameOverScoreText.text     = "Score: " + score;
        if (gameOverHighScoreText != null) gameOverHighScoreText.text = "High Score: " + highScore;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public bool IsGameOver() => isGameOver;
}
