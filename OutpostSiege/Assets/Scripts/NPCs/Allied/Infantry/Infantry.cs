using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Infantry : MonoBehaviour
{
    public enum Direction { Left, Right }
    public Direction TargetDirection { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float minMoveSpeed = 2f;
    [SerializeField] private float maxMoveSpeed = 3f;
    private float moveSpeed;

    [Header("Combat Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private int health = 20;
    [SerializeField] private Weapon weapon;

    [Header("Gizmo Y Offset")]
    [SerializeField] private float gizmosYOffset = 1f;

    [Header("Visual Effects")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color hitColor = Color.white;

    private Animator animator;
    private float lastAttackTime;
    private GameObject targetEnemy;
    private bool facingRight = true;

    private SpriteRenderer sr;
    private Color originalColor;
    private Collider2D col;

    private Vector3 moveTarget;
    private bool isMoving = false;
    private bool isEngagingEnemy = false;

    public bool HasMoved => isMoving;
    public bool IsFacingRight() => facingRight;
    public bool IsFacingLeft() => !facingRight;

    public bool IsEngagingEnemy => isEngagingEnemy;
    void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        // Seteaza moveSpeed random anitre min si max
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    void Update()
    {
        // 1. Cauta inamic
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
            if (isEngagingEnemy)
            {
                isEngagingEnemy = false; // inamic eliminat
                if (isMoving)
                {
                    animator.SetTrigger("running"); // reia animatia de alergare
                }
            }

            animator.ResetTrigger("shooting");
        }

        // 2. Miscare doar daca nu suntem in lupta
        if (isMoving && !isEngagingEnemy)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
            animator.SetTrigger("running");

            float distance = Vector3.Distance(transform.position, moveTarget);
            if (distance < 1.5f)
            {
                Debug.Log("Infanteria a ajuns la destinatie");
                isMoving = false;
                animator.ResetTrigger("running");
            }
        }
    }

    public void MoveTo(Vector3 position)
    {
        moveTarget = position;
        //Debug.Log($"New target set at: {moveTarget}"); // Verifica unde se indreapta
        //Debug.Log($"[Infantry] Moving to {position}");
        isMoving = true;
        TargetDirection = position.x < transform.position.x ? Direction.Left : Direction.Right;
        Flip(position);
        animator.ResetTrigger("running");
    }

    public void SetTargetDirection(Direction direction)
    {
        TargetDirection = direction;
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

    // Apelat din Animation Event
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

    private IEnumerator FlashWhite()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    private void Die()
    {
        // Informeaza managerul de moarte lol
        Infantry_Manager manager = FindFirstObjectByType<Infantry_Manager>();
        if (manager != null)
        {
            manager.OnInfantryDeath(this);
        }

        if (col != null) col.enabled = false;
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
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
    public bool IsAtTarget()
    {
        return Vector3.Distance(transform.position, moveTarget) < 0.1f;
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
        Gizmos.color = Color.red;
        Vector3 posYOffset = new Vector3(0f, gizmosYOffset, 0f);
        Gizmos.DrawWireSphere(transform.position + posYOffset, detectionRadius);
    }

    public void ForceTurnAround()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        Debug.Log("Infanteria intoarsa 180 grade");
    }
}
