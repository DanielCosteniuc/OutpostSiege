using UnityEngine;

public class Infantry : MonoBehaviour
{
    /*private Animator animator;

    [Header("Settings")]
    public float moveSpeed = 2f;
    public Transform targetPoint; // Poate fi setat din inspector sau de către un manager

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Pornim cu animația de mers dacă avem o destinație
        if (targetPoint != null)
        {
            animator.SetBool("isRunning", true);
        }
    }

    private void Update()
    {
        if (targetPoint != null)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, targetPoint.position);

        if (distance < 0.1f)
        {
            animator.SetBool("isRunning", false);
            animator.SetTrigger("shoot"); // Treci la animația de tragere
            targetPoint = null; // Oprește mișcarea
        }
    }*/
}
