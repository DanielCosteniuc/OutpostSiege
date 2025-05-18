using System.Collections.Generic;
using UnityEngine;

public class Lvl2_Wall_Interaction : MonoBehaviour
{
    // Coin Setup
    [Header("Coin Setup")]
    public GameObject coinPrefab;            // Unpaid coin icon
    public GameObject paidCoinPrefab;        // Paid coin icon
    public List<Transform> coinSpawnPoints;  // List of locations to spawn coin visuals

    private List<GameObject> coinInstances = new List<GameObject>();  // List of current coin objects
    private bool isPaid = false;             // Track whether coins have been paid

    // Unity Lifecycle
    private void Start()
    {
        // Initialize the coin instances list (empty to start)
        coinInstances.Clear();
    }

    // Player Enters Wall Area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Spawn coins at available spawn points if not already paid
            if (coinInstances.Count == 0 && !isPaid)
            {
                foreach (Transform spawnPoint in coinSpawnPoints)
                {
                    GameObject coinInstance = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity, transform);
                    coinInstances.Add(coinInstance);  // Add the spawned coin to the list
                }
            }
        }
    }

    // Player Leaves Wall Area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Destroy coins when player leaves area and no coins are paid yet
            if (!isPaid && coinInstances.Count > 0)
            {
                foreach (GameObject coin in coinInstances)
                {
                    Destroy(coin);  // Destroy each coin in the list
                }
                coinInstances.Clear();  // Clear the list of coin instances
            }
        }
    }
}
