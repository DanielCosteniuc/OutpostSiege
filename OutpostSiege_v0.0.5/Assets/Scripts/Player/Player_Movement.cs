using UnityEngine;

public class Player_Movements : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float moveForce = 10f;

    private float movementX;

    private Rigidbody2D playerBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private string RUNNING_ANIMATION = "running";

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;
    }

    void HandleAnimation()
    {
        if (movementX > 0f)
        {
            animator.SetBool(RUNNING_ANIMATION, true);
            spriteRenderer.flipX = false;
        }
        else if (movementX < 0f)
        {
            animator.SetBool(RUNNING_ANIMATION, true);
            spriteRenderer.flipX = true;
        }
        else
        {
            animator.SetBool(RUNNING_ANIMATION, false);
        }
    }
}
