using UnityEngine;

/// <summary>
/// Attach ke SubmitButton agar onClick bisa di-persist di scene.
/// Tidak perlu AddListener via code — assign lewat Inspector OnClick.
/// </summary>
public class SubmitButtonHandler : MonoBehaviour
{
    // Dipanggil oleh Button OnClick (persistent, di-assign di Inspector/Editor)
    public void Submit()
    {
        MathUIRefs.Instance?.SubmitFromButton();
    }
}
