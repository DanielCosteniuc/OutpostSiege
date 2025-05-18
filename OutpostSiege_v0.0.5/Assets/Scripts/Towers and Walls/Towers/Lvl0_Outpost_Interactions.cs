using System.Collections.Generic;
using UnityEngine;

public class Lvl0_Outpost_Interactions : MonoBehaviour
{
    [Header("Coin Setup")]
    public GameObject coinPrefab;             // Prefab for the coin the player can collect
    public GameObject paidCoinPrefab;         // Prefab shown after coins are paid
    public List<Transform> coinSpawnPoints;   // Spawn points for coin placement

    [Header("Tree Blocking Settings")]
    [SerializeField] private float blockRadius = 5f; // Radius to check for nearby trees

    private List<GameObject> coinInstances = new();  // List of active coins in the scene
    private bool isPaid = false;                     // Flag to prevent repeat payments

    private void Start()
    {
        // Clear any leftover coin references on start
        coinInstances.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Trigger interaction only when player enters and coins haven't been paid
        if (other.CompareTag("Player") && !isPaid && coinInstances.Count == 0)
        {
            // Don't spawn coins if trees are nearby
            if (AreTreesNearby()) return;

            // Spawn coins at designated spawn points
            foreach (Transform spawnPoint in coinSpawnPoints)
            {
                var coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity, transform);
                coinInstances.Add(coin);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Remove unclaimed coins when the player leaves
        if (other.CompareTag("Player") && !isPaid)
        {
            foreach (var coin in coinInstances)
            {
                Destroy(coin);
            }
            coinInstances.Clear();
        }
    }

    // Called when the coins have been collected/paid
    public void MarkCoinsAsPaid()
    {
        isPaid = true;

        // Replace active coins with paid coin visuals
        foreach (var coin in coinInstances)
        {
            Instantiate(paidCoinPrefab, coin.transform.position, Quaternion.identity, transform);
            Destroy(coin);
        }

        coinInstances.Clear();
    }

    // Checks for trees within a certain radius
    private bool AreTreesNearby()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, blockRadius);
        foreach (Collider2D col in nearbyObjects)
        {
            if (col.CompareTag("Tree")) return true;
        }
        return false;
    }

#if UNITY_EDITOR
    // Visualize blocking radius in Scene view for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, blockRadius);
    }
#endif
}
