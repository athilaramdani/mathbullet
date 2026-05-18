using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MathUIRefs : MonoBehaviour
{
    public static MathUIRefs Instance;

    [Header("Math Panel References")]
    public GameObject mathPanel;
    public TextMeshProUGUI questionText;
    public TMP_InputField answerInput;
    public TextMeshProUGUI feedbackText;

    private MathAmmo currentAmmo;
    private bool panelOpen = false;

    void Awake()
    {
        Instance = this;
        if (mathPanel != null) mathPanel.SetActive(false);

        // Auto-create EventSystem if missing (needed for UI input!)
        if (FindObjectOfType<EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
            Debug.Log("[MathUIRefs] EventSystem auto-created.");
        }
    }

    public void OpenPanel(MathAmmo ammo, string question)
    {
        currentAmmo = ammo;
        panelOpen = true;

        if (questionText != null) questionText.text = question;
        if (feedbackText != null) feedbackText.text = "Jawab untuk mendapat peluru!";

        if (answerInput != null)
        {
            answerInput.text = "";
            answerInput.interactable = true;
        }

        if (mathPanel != null) mathPanel.SetActive(true);

        // Focus input field after one frame
        StartCoroutine(FocusInput());
    }

    System.Collections.IEnumerator FocusInput()
    {
        yield return null; // wait 1 frame
        if (answerInput != null)
        {
            answerInput.Select();
            answerInput.ActivateInputField();
        }
    }

    public void ClosePanel()
    {
        currentAmmo = null;
        panelOpen = false;
        if (mathPanel != null) mathPanel.SetActive(false);
    }

    public bool IsPanelOpen() => panelOpen;

    public void SubmitFromButton()
    {
        if (currentAmmo != null) currentAmmo.SubmitAnswer();
    }

    public void ShowFeedback(string msg)
    {
        if (feedbackText != null) feedbackText.text = msg;
    }

    public string GetInputText()
    {
        return answerInput != null ? answerInput.text : "";
    }
}
