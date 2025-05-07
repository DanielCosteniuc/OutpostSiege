using UnityEngine;

public class Tree_Interactions : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    public Color highlightColor = Color.white;

    [Header("Coin Setup")]
    public GameObject coinPrefab;
    public GameObject paidCoinPrefab;
    private GameObject coinInstance;
    public Transform coinSpawnPoint;

    private bool isPaid = false; // 👉 adăugat

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sr.color = highlightColor;

            if (coinInstance == null && !isPaid)
            {
                coinInstance = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sr.color = originalColor;

            // 👉 nu mai distruge moneda dacă e deja plătită
            if (!isPaid && coinInstance != null)
            {
                Destroy(coinInstance);
                coinInstance = null;
            }
        }
    }

    public void ChangeCoinVisual()
    {
        if (coinInstance != null && paidCoinPrefab != null)
        {
            Destroy(coinInstance);
            coinInstance = Instantiate(paidCoinPrefab, coinSpawnPoint.position, Quaternion.identity, transform);
            isPaid = true; // 👉 marcat ca plătit
        }
    }

    // Oferă o metodă publică dacă vrei să ștergi moneda când copacul e tăiat
    public void RemoveCoinVisual()
    {
        if (coinInstance != null)
        {
            Destroy(coinInstance);
        }
    }
}
