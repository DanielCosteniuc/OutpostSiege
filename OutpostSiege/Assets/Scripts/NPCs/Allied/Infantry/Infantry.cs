using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Infantry : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private int health = 20;
    [SerializeField] private Weapon weapon;

    [Header("Gizmo OY Offset")]
    [SerializeField] private float gizmosYOffset = 1f;

    [Header("Visual Effects")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color hitColor = Color.white; // FF8080 will be assigned in Unity

    private Animator animator;
    private float lastAttackTime;
    private GameObject targetEnemy;
    private bool facingRight = true;
    
    // Visual effects
    private SpriteRenderer sr;
    private Color originalColor;
    private Collider2D col;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastAttackTime = -attackInterval;
        col = GetComponent<Collider2D>();

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;    
    }

    void Update()
    {
        targetEnemy = FindNearestEnemy();

        if (targetEnemy != null)
        {
            Flip(targetEnemy.transform.position); // Face the enemy

            if (Time.time >= lastAttackTime + attackInterval)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("shooting");
            }
        }
        else
        {
            animator.ResetTrigger("shooting");
        }
    }

    GameObject FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                return hit.gameObject;
            }
        }
        return null;
    }

    // This is called from an Animation Event at the right shooting frame
    public void Fire()
    {
        if (targetEnemy != null)
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

    private System.Collections.IEnumerator FlashWhite()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    private void Die()
    {
        if (col != null) col.enabled = false;
        StartCoroutine(FadeAndDestroy());
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

    private void Flip(Vector3 targetPosition)
    {
        if (targetPosition.x < transform.position.x && facingRight)
        {
            // Enemy is on the left, but we're facing right — rotate to face left
            transform.Rotate(0f, 180f, 0f);
            facingRight = false;
        }
        else if (targetPosition.x > transform.position.x && !facingRight)
        {
            // Enemy is on the right, but we're facing left — rotate to face right
            transform.Rotate(0f, 180f, 0f);
            facingRight = true;
        }
    }
}
