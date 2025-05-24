using UnityEngine;

public class Infantry_Enemy : MonoBehaviour
{
    [Header("Enemey Stats")]
    [SerializeField] private int health = 20;
    public float moveSpeed = 3.5f;

    [Header("Combat Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private Enemy_Weapon weapon;

    [Header("Visual Effects")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color hitColor = Color.white; // FF8080 will be assigned in Unity

    [Header("Gizmo OY Offset")]
    [SerializeField] private float gizmosYOffset = 0.75f;

    private SpriteRenderer sr;
    private Color originalColor;
    private Collider2D col;
    private Rigidbody2D enemyBoby;
    private Animator animator;
    private GameObject targetAllied;
    [HideInInspector] public int moveDirection = 1; // +1 = right, -1 = left

    private void Start()
    {
        enemyBoby = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        col = GetComponent<Collider2D>();
    }
    private void Update()
    {
        targetAllied = FindNearestEnemy();

        if (targetAllied != null)
        {
            animator.SetBool("isRunning", false); // Stop
            animator.SetTrigger("isShooting");    // Shoot animation

            enemyBoby.linearVelocity = Vector2.zero;
        }
        else
        {
            animator.ResetTrigger("isShooting");
            animator.SetBool("isRunning", true);  // Move
            enemyBoby.linearVelocity = new Vector2(moveSpeed * moveDirection, enemyBoby.linearVelocity.y);
        }
    }

    public void Fire()
    {
        if (targetAllied != null)
        {
            StartCoroutine(weapon.Shoot());
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(FlashWhite());

        if (health <= 0)
        {
            Die();
        }
    }

    GameObject FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Allied"))
            {
                return hit.gameObject;
            }
        }
        return null;
    }

    private void Die()
    {
        if (col != null) col.enabled = false;
        StartCoroutine(FadeAndDestroy());
    }

    // Visual effects on Die()
    private System.Collections.IEnumerator FlashWhite()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    private System.Collections.IEnumerator FadeAndDestroy()
    {
        float timer = 0f;
        Color startColor = sr.color;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            sr.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, t));
            timer += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        Destroy(gameObject);
    }

    // Shooting range gizmos 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 posYOffset = new Vector3(0f, gizmosYOffset, 0f);
        Gizmos.DrawWireSphere(transform.position + posYOffset, detectionRadius);
    }
}
