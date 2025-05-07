using UnityEngine;

public class Lvl1_Engineer_Tent : MonoBehaviour
{
    [Header("Coin Interaction")]
    [SerializeField] private GameObject coinHolderPrefab;
    [SerializeField] private Transform[] coinSpawnPoints;

    private GameObject[] spawnedCoins;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spawnedCoins == null)
        {
            SpawnCoinHolders();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DestroyCoinHolders();
        }
    }

    private void SpawnCoinHolders()
    {
        spawnedCoins = new GameObject[coinSpawnPoints.Length];

        for (int i = 0; i < coinSpawnPoints.Length; i++)
        {
            spawnedCoins[i] = Instantiate(coinHolderPrefab, coinSpawnPoints[i].position, Quaternion.identity, transform);
        }
    }

    private void DestroyCoinHolders()
    {
        if (spawnedCoins != null)
        {
            foreach (GameObject coin in spawnedCoins)
            {
                if (coin != null)
                {
                    Destroy(coin);
                }
            }

            spawnedCoins = null;
        }
    }
}
