using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.25f;

    [Header("Gun Visual")]
    public Transform gunPivot;

    private float nextFireTime = 0f;
    private Camera mainCamera;

    // Shield state
    private bool isShielded = false;
    private float shieldTimer = 0f;
    private GameObject shieldVisual;

    // Prevent shooting while math panel is open
    private bool mathPanelBlocking => MathUIRefs.Instance != null &&
                                      MathUIRefs.Instance.IsPanelOpen();

    void Awake()
    {
        mainCamera = Camera.main;

        // Fix sorting: make sure player sprite renders in front
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = 10;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        AimAtMouse();

        // Shield timer
        if (isShielded)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                isShielded = false;
                if (shieldVisual != null) shieldVisual.SetActive(false);
            }
        }

        // Don't shoot if math panel is open
        if (mathPanelBlocking) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (GameManager.Instance != null && GameManager.Instance.HasAmmo())
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void AimAtMouse()
    {
        if (mainCamera == null || gunPivot == null) return;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 dir = (mouseWorldPos - gunPivot.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;
        GameManager.Instance.UseAmmo();
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        SoundManager.Instance?.PlayShoot();
    }

    public void ActivateShield(float duration)
    {
        isShielded = true;
        shieldTimer = duration;
        if (shieldVisual == null)
        {
            Transform st = transform.Find("ShieldVisual");
            if (st != null) shieldVisual = st.gameObject;
        }
        if (shieldVisual != null) shieldVisual.SetActive(true);
    }

    public bool IsShielded() => isShielded;
}
