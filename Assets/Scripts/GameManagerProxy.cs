using UnityEngine;

/// <summary>
/// Proxy script untuk wiring persistent Button onClick ke GameManager.
/// Dipakai oleh SceneSetupTool agar Restart/Quit bisa di-persist di scene.
/// </summary>
public class GameManagerProxy : MonoBehaviour
{
    public void Restart()
    {
        if (GameManager.Instance != null) GameManager.Instance.RestartGame();
    }

    public void Quit()
    {
        if (GameManager.Instance != null) GameManager.Instance.QuitGame();
    }
}
