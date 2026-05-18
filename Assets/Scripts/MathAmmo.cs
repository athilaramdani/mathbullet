using UnityEngine;

public class MathAmmo : MonoBehaviour
{
    [Header("Ammo Rewards")]
    public int correctAnswerAmmo = 20;
    public int wrongAnswerPenalty = 5;  // dikurangi saat salah

    [Header("Interaction")]
    public float interactRadius = 2f;

    private int correctAnswer;
    private string questionString;
    private bool isPanelOpen = false;
    private bool isAnswered = false;
    private Transform playerTransform;

    private static MathAmmo currentOpenAmmo = null;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        int a = Random.Range(1, 20);
        int b = Random.Range(1, 20);
        int op = Random.Range(0, 3);

        switch (op)
        {
            case 0:
                correctAnswer = a + b;
                questionString = $"{a} + {b} = ?";
                break;
            case 1:
                if (a < b) { int t = a; a = b; b = t; }
                correctAnswer = a - b;
                questionString = $"{a} - {b} = ?";
                break;
            default:
                a = Random.Range(1, 10); b = Random.Range(1, 10);
                correctAnswer = a * b;
                questionString = $"{a} \u00d7 {b} = ?";
                break;
        }
    }

    void Update()
    {
        if (isAnswered) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            ClosePanel(); return;
        }
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist <= interactRadius && !isPanelOpen && currentOpenAmmo == null)
            OpenPanel();
        else if (dist > interactRadius * 1.5f && isPanelOpen)
            ClosePanel();
    }

    void OpenPanel()
    {
        isPanelOpen = true;
        currentOpenAmmo = this;

        // Show and unlock cursor so player can click/type
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        MathUIRefs.Instance?.OpenPanel(this, questionString);
    }

    void ClosePanel()
    {
        isPanelOpen = false;
        if (currentOpenAmmo == this) currentOpenAmmo = null;
        MathUIRefs.Instance?.ClosePanel();
    }

    public void SubmitAnswer()
    {
        if (isAnswered) return;

        string inputStr = MathUIRefs.Instance?.GetInputText() ?? "";
        if (!int.TryParse(inputStr.Trim(), out int playerAnswer))
        {
            MathUIRefs.Instance?.ShowFeedback("⚠ Masukkan angka yang valid!");
            return;
        }

        if (playerAnswer == correctAnswer)
        {
            MathUIRefs.Instance?.ShowFeedback($"✅ Benar! +{correctAnswerAmmo} peluru!");
            GameManager.Instance?.AddAmmo(correctAnswerAmmo);
            SoundManager.Instance?.PlayCorrect(); // play benar.wav
        }
        else
        {
            MathUIRefs.Instance?.ShowFeedback($"❌ Salah! Jawaban: {correctAnswer} | -{wrongAnswerPenalty} peluru");
            GameManager.Instance?.AddAmmo(-wrongAnswerPenalty); // kurangi 5 peluru
        }

        isAnswered = true;
        Invoke(nameof(DestroyAmmo), 1.5f);
    }

    void DestroyAmmo()
    {
        ClosePanel();
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
