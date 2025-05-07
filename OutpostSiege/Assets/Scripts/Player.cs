using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("⚙️ Mișcare")]
    [SerializeField] private float moveForce = 10f;

    [Header("👷 Inginer")]
    [SerializeField] private Engineer engineer;
    [SerializeField] private float interactionRange = 2f;

    [Header("💰 Monede")]
    [SerializeField] private Text coinText;
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private int startingCoins = 5;
    private int currentCoins;

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

    private void Start()
    {
        currentCoins = Mathf.Clamp(startingCoins, 0, maxCoins);
        UpdateCoinUI();
    }

    private void Update()
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
        if (currentCoins <= 0)
        {
            Debug.Log("❌ Nu ai suficiente monede pentru a tăia un copac.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);

        foreach (var hit in hits)
        {
            Tree tree = hit.GetComponent<Tree>();
            if (tree != null && !tree.isSelected)
            {
                tree.isSelected = true;
                selectedTrees.Add(tree.gameObject);

                currentCoins--; // scade o monedă
                UpdateCoinUI();

                Tree_Interactions interaction = tree.GetComponent<Tree_Interactions>();
                if (interaction != null)
                {
                    interaction.ChangeCoinVisual(); // schimbă moneda
                }

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

            int coinsEarned = Random.Range(1, 3); // 1 sau 2
            currentCoins = Mathf.Min(currentCoins + coinsEarned, maxCoins);
            UpdateCoinUI();

            Debug.Log($"💰 Ai primit {coinsEarned} monedă(e). Total: {currentCoins}");
        }
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"x{currentCoins}";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    public bool TrySpendCoin()
    {
        if (currentCoins > 0)
        {
            currentCoins--;
            UpdateCoinUI();
            return true;
        }
        return false;
    }

    public void AddCoinBack()
    {
        currentCoins = Mathf.Min(currentCoins + 1, maxCoins);
        UpdateCoinUI();
    }



}
