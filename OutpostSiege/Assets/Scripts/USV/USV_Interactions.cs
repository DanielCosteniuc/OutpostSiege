using System.Collections.Generic;
using UnityEngine;

public class USV_Interactions : MonoBehaviour
{
    // Coin Setup
    [Header("Coin Setup")]
    public GameObject coinPrefab;
    public GameObject paidCoinPrefab;
    public List<Transform> coinSpawnPoints;

    private List<GameObject> coinInstances = new List<GameObject>();
    private bool isPaid = false;

    [Header("Tree Blocking Settings")]
    [SerializeField] private Vector2 treeBlockSize = new Vector2(3f, 2f); // Width & Height
    [SerializeField] private string treeTag = "Tree";
    [SerializeField] private Transform treeBlockCenterPoint; // Detection area center

    private void Start()
    {
        coinInstances.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (coinInstances.Count == 0 && !isPaid && !IsTreeNearby())
            {
                foreach (Transform spawnPoint in coinSpawnPoints)
                {
                    GameObject coinInstance = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity, transform);
                    coinInstances.Add(coinInstance);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isPaid && coinInstances.Count > 0)
            {
                foreach (GameObject coin in coinInstances)
                {
                    Destroy(coin);
                }
                coinInstances.Clear();
            }
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

    private void OnDrawGizmosSelected()
    {
        if (treeBlockCenterPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(treeBlockCenterPoint.position, treeBlockSize);
        }
    }
}
