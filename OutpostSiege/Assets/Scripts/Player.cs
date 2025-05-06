using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("⚙️ Mișcare")]
    [SerializeField] private float moveForce = 10f;

    [Header("👷 Inginer")]
    [SerializeField] private Engineer engineer;
    [SerializeField] private float interactionRange = 2f;

    private float movementX;
    private Rigidbody2D playerBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private string RUNNING_ANIMATION = "running";

    private List<GameObject> selectedTrees = new List<GameObject>();
    private int maxTrees = 3;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        PlayerMoveKeyboard();
        AnimatePlayer();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TrySelectTree();
        }
    }

    void PlayerMoveKeyboard()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(movementX, 0f, 0f) * Time.deltaTime * moveForce;
    }

    void AnimatePlayer()
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

    void TrySelectTree()
    {
        if (selectedTrees.Count >= maxTrees) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);

        foreach (var hit in hits)
        {
            Tree tree = hit.GetComponent<Tree>();
            if (tree != null && !tree.isSelected)
            {
                tree.isSelected = true;
                selectedTrees.Add(tree.gameObject);

                engineer.CutTree(tree.gameObject, OnTreeCut);

                Debug.Log($"🌳 Copac selectat: {tree.name} (Total: {selectedTrees.Count})");
                break;
            }
        }
    }

    void OnTreeCut(GameObject tree)
    {
        if (selectedTrees.Contains(tree))
        {
            selectedTrees.Remove(tree);
            Debug.Log($"✅ Copac tăiat și scos din listă: {tree.name} (Rămași: {selectedTrees.Count})");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
