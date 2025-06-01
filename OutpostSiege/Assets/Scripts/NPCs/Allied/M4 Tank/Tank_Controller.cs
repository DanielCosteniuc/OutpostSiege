using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Tank_Controller : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float detectionRadius = 7f;
    [SerializeField] private float minShootingRadius = 2f;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private Gun gun;

    [Header("Gizmo Y Offset")]
    [SerializeField] private float gizmosYOffset = 1f;

    private Animator animator;
    private float lastAttackTime;
    private GameObject targetEnemy;
    private bool isEngagingEnemy = false;
    private bool facingRight = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        targetEnemy = FindNearestEnemy();

        if (targetEnemy != null)
        {
            isEngagingEnemy = true;
            Flip(targetEnemy.transform.position);
            animator.ResetTrigger("isMoveing"); // in case you have other anims

            if (Time.time >= lastAttackTime + attackInterval)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("isShooting"); // Animation event calls Fire()
            }
        }
        else
        {
            isEngagingEnemy = false;
            animator.ResetTrigger("isShooting");
        }
    }

    GameObject FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);

                if (distance >= minShootingRadius && distance <= detectionRadius && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.gameObject;
                }
            }
        }

        return closestEnemy;
    }

    // Called from animation event
    public void Fire()
    {
        if (targetEnemy != null)
        {
            Vector2 targetCenter = targetEnemy.GetComponent<Collider2D>().bounds.center;
            Vector2 direction = (targetCenter - (Vector2)gun.firePoint.position).normalized;

            gun.Shoot(direction);
        }
    }

    private void Flip(Vector3 targetPosition)
    {
        if (targetPosition.x < transform.position.x && facingRight)
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = false;
        }
        else if (targetPosition.x > transform.position.x && !facingRight)
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 offset = new Vector3(0f, gizmosYOffset, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + offset, minShootingRadius);
    }
}
