using UnityEngine;

public class Tree_Interactions : MonoBehaviour
{
    // Visual Setup
    private SpriteRenderer sr;
    private Color originalColor;
    public Color highlightColor = Color.white;

    // Coin Setup
    [Header("Coin Setup")]
    public GameObject coinPrefab;           // Unpaid coin icon
    public GameObject paidCoinPrefab;       // Paid coin icon
    public Transform coinSpawnPoint;        // Location to spawn coin visuals

    private GameObject coinInstance;        // Current coin object (if any)
    private bool isPaid = false;            // Was this tree already paid for?

    // Unity Lifecycle
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        // If the tree is already marked as paid, restore coin visual
        if (isPaid && paidCoinPrefab != null)
        {
            coinInstance = Instantiate(paidCoinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
        }
    }

    // Player Enters Tree Area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sr.color = highlightColor;

            // Show unpaid coin visual only if not already paid
            if (coinInstance == null && !isPaid)
            {
                coinInstance = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
            }
        }
    }

    // Player Leaves Tree Area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sr.color = originalColor;

            // Remove unpaid coin visual when exiting
            if (!isPaid && coinInstance != null)
            {
                Destroy(coinInstance);
                coinInstance = null;
            }
        }
    }

    // Tree was Paid For (used during selection)
    public void ChangeCoinVisual()
    {
        if (coinInstance != null && paidCoinPrefab != null)
        {
            Destroy(coinInstance);
            coinInstance = Instantiate(paidCoinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
            isPaid = true;
        }
    }

    // Tree was Cut Down or Deselected
    public void RemoveCoinVisual()
    {
        if (coinInstance != null)
        {
            Destroy(coinInstance);
            coinInstance = null;
        }
    }

    // Force Visual to Paid State (used if tree task picked later)
    public void ForcePaidVisual()
    {
        if (paidCoinPrefab == null) return;

        if (coinInstance != null)
            Destroy(coinInstance);

        coinInstance = Instantiate(paidCoinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
        isPaid = true;
    }
    public void ResetPayment()
    {
        if (isPaid)
        {
            isPaid = false;
            RemoveCoinVisual();
        }
    }
}
