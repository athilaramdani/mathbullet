using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainPanel;
    public TextMeshProUGUI highScoreText;
    public Button playButton;
    public Button quitButton;

    [Header("Animation")]
    public float titleBobSpeed = 1.5f;
    public Transform titleTransform;

    private Vector3 titleStartPos;

    void Start()
    {
        // Show saved high score
        int hs = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = hs > 0 ? "High Score: " + hs : "";

        // Wire buttons
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (titleTransform != null)
            titleStartPos = titleTransform.localPosition;

        Time.timeScale = 1f;
    }

    void Update()
    {
        // Gentle bob animation for title
        if (titleTransform != null)
        {
            float yOffset = Mathf.Sin(Time.time * titleBobSpeed) * 8f;
            titleTransform.localPosition = titleStartPos + Vector3.up * yOffset;
        }

        // Press any key / Enter to start
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            PlayGame();
    }

    public void PlayGame()
    {
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        // Small delay for feel
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
