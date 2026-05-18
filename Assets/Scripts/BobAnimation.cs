using UnityEngine;

/// <summary>
/// Simple up-down bob animation for pickup items (MathAmmo, PowerUps).
/// </summary>
public class BobAnimation : MonoBehaviour
{
    public float speed = 2f;
    public float height = 0.18f;

    private Vector3 startPos;

    void Start() => startPos = transform.position;

    void Update()
    {
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * speed) * height;
    }
}
