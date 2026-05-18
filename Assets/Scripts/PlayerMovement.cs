using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Speed Boost")]
    public float boostedSpeed = 9f;
    private bool isSpeedBoosted = false;
    private float speedBoostTimer = 0f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Visual feedback for speed boost
    private SpriteRenderer spriteRenderer;
    private Color normalColor = Color.white;
    private Color boostedColor = new Color(0.5f, 1f, 0.5f); // light green tint

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;

        // Speed boost timer
        if (isSpeedBoosted)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0f)
            {
                isSpeedBoosted = false;
                if (spriteRenderer != null)
                    spriteRenderer.color = normalColor;
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        float currentSpeed = isSpeedBoosted ? boostedSpeed : moveSpeed;
        rb.velocity = moveInput * currentSpeed;
    }

    public void ActivateSpeedBoost(float duration)
    {
        isSpeedBoosted = true;
        speedBoostTimer = duration;
        if (spriteRenderer != null)
            spriteRenderer.color = boostedColor;
    }
}
