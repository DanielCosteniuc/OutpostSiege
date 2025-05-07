using UnityEngine;

public class Tree_Interactions : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    public Color highlightColor = Color.white;

    [Header("Coin Setup")]
    public GameObject coinPrefab;
    private GameObject coinInstance;
    public Transform coinSpawnPoint;

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

            if (coinInstance == null) // only spawn once
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

            if (coinInstance != null)
            {
                Destroy(coinInstance);
                coinInstance = null;
            }
        }
    }
}
