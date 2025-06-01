using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Sniper : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float minShootingRadius = 1f; // new field
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private Rifle weapon;

    [Header("Gizmo Y Offset")]
    [SerializeField] private float gizmosYOffset = 1f;

    private Animator animator;
    private float lastAttackTime;
    private GameObject targetEnemy;
    private bool facingRight = true;
    private bool isEngagingEnemy = false;

    public bool IsFacingRight() => facingRight;
    public bool IsFacingLeft() => !facingRight;
    public bool IsEngagingEnemy => isEngagingEnemy;

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
            animator.ResetTrigger("running");

            if (Time.time >= lastAttackTime + attackInterval)
            {
                lastAttackTime = Time.time;
                animator.SetTrigger("shooting");
            }
        }
        else
        {
            isEngagingEnemy = false;
            animator.ResetTrigger("shooting");
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

    // Called from Animation Event
    public void Fire()
    {
        if (targetEnemy != null)
        {
            Vector2 targetCenter = targetEnemy.GetComponent<Collider2D>().bounds.center;
            Vector2 direction = (targetCenter - (Vector2)weapon.firePoint.position).normalized;

            float verticalOffset = 0.03f; 
            direction.y += verticalOffset;
            direction = direction.normalized; 

            StartCoroutine(weapon.Shoot(direction));
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
        Vector3 posYOffset = new Vector3(0f, gizmosYOffset, 0f);

        // Max radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + posYOffset, detectionRadius);

        // Min radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + posYOffset, minShootingRadius);
    }

    public void ForceTurnAround()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        Debug.Log("Infanteria intoarsa 180 grade");
    }
}