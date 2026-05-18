using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public int maxHealth = 1;
    public int scoreValue = 10;
    public int damageToPlayer = 1;

    private int currentHealth;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isFrozen = false;
    private float freezeTimer = 0f;

    // Visual
    private SpriteRenderer spriteRenderer;
    private Color normalColor;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        if (spriteRenderer != null)
            normalColor = spriteRenderer.color;
    }

    void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        // Freeze timer
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                isFrozen = false;
                if (spriteRenderer != null)
                    spriteRenderer.color = normalColor;
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;
        if (isFrozen) return;
        if (playerTransform == null) return;

        // Move toward player
        Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Flash white on hit
            if (spriteRenderer != null)
                StartCoroutine(FlashWhite());
        }
    }

    void Die()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        SoundManager.Instance?.PlayZombieDead(); // random zombiedead1-4.ogg

        Destroy(gameObject);
    }

    System.Collections.IEnumerator FlashWhite()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerShooting shooting = collision.gameObject.GetComponent<PlayerShooting>();

            // Check if player has shield
            if (shooting != null && shooting.IsShielded())
            {
                // Bounce enemy away instead
                Vector2 bounceDir = (transform.position - collision.transform.position).normalized;
                rb.velocity = bounceDir * 5f;
                return;
            }

            if (GameManager.Instance != null)
                GameManager.Instance.TakeDamage(damageToPlayer);
        }
    }

    public void Freeze(float duration)
    {
        isFrozen = true;
        freezeTimer = duration;
        rb.velocity = Vector2.zero;
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.5f, 0.8f, 1f); // ice blue
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
