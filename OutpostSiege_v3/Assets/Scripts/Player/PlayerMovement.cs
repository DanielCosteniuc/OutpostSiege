using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("⚙️ Mișcare")]
    [SerializeField] private float moveForce = 10f;

    private float movementX;
    private Rigidbody2D playerBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private readonly string RUNNING_ANIMATION = "running";

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleMovement();
        AnimatePlayer();
    }

    private void HandleMovement()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;
    }

    private void AnimatePlayer()
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
