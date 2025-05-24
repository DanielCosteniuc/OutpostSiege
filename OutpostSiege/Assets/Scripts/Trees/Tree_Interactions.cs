using UnityEngine;

public class Tree_Interactions : MonoBehaviour
{
    // Sprite renderer for tree highlight feedback
    private SpriteRenderer sr;
    private Color originalColor;
    public Color highlightColor = Color.white;

    [Header("Coin Setup")]
    public GameObject coinPrefab;           // Unpaid coin
    public GameObject paidCoinPrefab;       // Paid coin
    public Transform coinSpawnPoint;        // Where to spawn coin visuals

    private GameObject coinInstance;        // Active coin on tree
    private bool isPaid = false;            // Payment state
    private bool isPlayerNear = false;      // Tracks player proximity

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNear = true;
        sr.color = highlightColor;

        // Show unpaid coin if unpaid and not already showing
        if (!isPaid && coinInstance == null && coinPrefab != null)
        {
            coinInstance = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNear = false;
        sr.color = originalColor;

        RemoveCoinVisual(); // Always remove coin when player exits
    }

    public void ChangeCoinVisual()
    {
        // Swap to paid coin only while player is near
        if (coinInstance != null)
            Destroy(coinInstance);

        if (paidCoinPrefab != null && isPlayerNear)
        {
            coinInstance = Instantiate(paidCoinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
        }

        isPaid = true;
    }

    public void ForcePaidVisual()
    {
        if (paidCoinPrefab == null) return;

        if (coinInstance != null)
            Destroy(coinInstance);

        // Engineers use this — show paid only if player is still nearby
        if (isPlayerNear)
        {
            coinInstance = Instantiate(paidCoinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
        }

        isPaid = true;
    }

    public void RemoveCoinVisual()
    {
        if (coinInstance != null)
        {
            Destroy(coinInstance);
            coinInstance = null;
        }
    }

    public void ResetPayment()
    {
        if (isPaid)
        {
            isPaid = false;
            RemoveCoinVisual();

            // Show unpaid coin again only if player is currently nearby
            if (isPlayerNear && coinPrefab != null)
            {
                coinInstance = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
            }
        }
    }
}
