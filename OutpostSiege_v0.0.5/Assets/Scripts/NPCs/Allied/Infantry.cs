using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Infantry : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private int health = 20;
    [SerializeField] private Weapon weapon;

    private Animator animator;
    private float lastAttackTime;
    private GameObject targetEnemy;
    private bool facingRight = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastAttackTime = -attackInterval;
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
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
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
