using System.Collections.Generic;
using UnityEngine;

public class USV_Interactions : MonoBehaviour
{
    // Coin Setup
    [Header("Coin Setup")]
    [SerializeField] private GameObject coinHolderPrefab;
    [SerializeField] private GameObject CoinPrefab;
    public List<Transform> coinSpawnPoints;

    private List<GameObject> coinInstances = new();
    [HideInInspector] public bool isPaidUSV;
    private int coinsInserted = 0;

    [Header("Tree Blocking Settings")]
    [SerializeField] private Vector2 treeBlockSize = new Vector2(3f, 2f); // Width & Height
    [SerializeField] private string treeTag = "Tree";
    [SerializeField] private Transform treeBlockCenterPoint; // Detection area center

    [Header("Player")]
    private Player_Interactions player;

    [Header("Dialogue")]
    [SerializeField] private Dialogue dialogueToTrigger;
    private DialogueManager dialogueManager;

    public int CoinsRequired => coinSpawnPoints.Count;

    private void Start()
    {
        coinInstances.Clear();
        player = GameObject.FindWithTag("Player").GetComponent<Player_Interactions>();
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        isPaidUSV = false;
    }

    private void Update()
    {
        if (!isPaidUSV && coinInstances.Count > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            if (player != null && player.TrySpendCoin())
            {
                Transform holderTransform = coinInstances[coinsInserted].transform;
                Instantiate(CoinPrefab, holderTransform.position, Quaternion.identity, holderTransform);

                coinsInserted++;

                if (coinsInserted >= CoinsRequired)
                {
                    isPaidUSV = true;
                    OnPaymentCompleted(); // Ai toate monedele, execută acțiunea
                }
            }
        }
    }

    private void OnPaymentCompleted()
    {
        Debug.Log("[USV_Interactions] Toate monedele au fost plasate, ai deblocat USV-ul!");

        // Trigger dialogue if references are set
        if (dialogueManager != null && dialogueToTrigger != null)
        {
            dialogueManager.StartDialogue(dialogueToTrigger);
        }
        else
        {
            Debug.LogWarning("DialogueManager or Dialogue reference missing on USV_Interactions.");
        }
    }

    public bool IsPaid() => isPaidUSV;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isPaidUSV || IsTreeNearby() || coinInstances.Count > 0)
            return;

        foreach (Transform spawnPoint in coinSpawnPoints)
        {
            var coin = Instantiate(coinHolderPrefab, spawnPoint.position, Quaternion.identity, transform);
            coinInstances.Add(coin);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!isPaidUSV)
        {
            player.ReturnCoinsToPlayer(coinsInserted);

            foreach (var coin in coinInstances)
            {
                Destroy(coin);
            }

            coinInstances.Clear();
            coinsInserted = 0;
        }
        else
        {
            // Remove visual coin holders if already paid
            foreach (var coin in coinInstances)
            {
                Destroy(coin);
            }

            coinInstances.Clear();
        }
    }

    private bool IsTreeNearby()
    {
        Vector2 center = treeBlockCenterPoint != null ? (Vector2)treeBlockCenterPoint.position : (Vector2)transform.position;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, treeBlockSize, 0f);
        foreach (var col in colliders)
        {
            if (col.CompareTag(treeTag))
                return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (treeBlockCenterPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(treeBlockCenterPoint.position, treeBlockSize);
        }
    }
#endif
}
