using UnityEngine;

public class Lvl1_Engineer_Tent : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject coinHolderPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject engineerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] coinSpawnPoints;
    [SerializeField] private Transform engineerSpawnPoint;

    private GameObject[] spawnedCoinHolders;
    private GameObject[] spawnedCoinVisuals;
    private int coinsInserted = 0;

    private Player player;
    private bool playerInRange = false;

    private void Update()
    {
        if (!playerInRange || player == null)
            return;

        // Dacă s-au resetat (după ce ai cumpărat un inginer), recreează suporturile de monede
        if (spawnedCoinHolders == null && playerInRange)
{
    SpawnCoinHolders();
}

        if (Input.GetKeyDown(KeyCode.Space) && coinsInserted < coinSpawnPoints.Length)
        {
            if (player.TrySpendCoin())
            {
                GameObject coin = Instantiate(
                    coinPrefab,
                    coinSpawnPoints[coinsInserted].position,
                    Quaternion.identity,
                    spawnedCoinHolders[coinsInserted].transform
                );

                spawnedCoinVisuals[coinsInserted] = coin;
                coinsInserted++;

                if (coinsInserted == coinSpawnPoints.Length)
                {
                    SpawnEngineer();
                    ResetCoinSlots();
                }
            }
            else
            {
                Debug.Log("❌ Nu ai suficiente monede.");
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spawnedCoinHolders == null)
        {
            player = other.GetComponent<Player>();
            playerInRange = true;
            SpawnCoinHolders();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Player>() == player)
        {
            playerInRange = false;
            player = null;
        }
    }

    private void SpawnCoinHolders()
    {
        spawnedCoinHolders = new GameObject[coinSpawnPoints.Length];
        spawnedCoinVisuals = new GameObject[coinSpawnPoints.Length];

        for (int i = 0; i < coinSpawnPoints.Length; i++)
        {
            spawnedCoinHolders[i] = Instantiate(
                coinHolderPrefab,
                coinSpawnPoints[i].position,
                Quaternion.identity,
                transform
            );
        }
    }

    private void SpawnEngineer()
    {
        if (engineerPrefab != null && engineerSpawnPoint != null)
        {
            GameObject newEngineer = Instantiate(engineerPrefab, engineerSpawnPoint.position, Quaternion.identity);
            Debug.Log("👷 Inginerul a fost generat!");

            // Setează inginerul nou ca activ în player
            if (player != null)
            {
                Engineer engineerComponent = newEngineer.GetComponent<Engineer>();
                if (engineerComponent != null)
                {
                    player.AddEngineer(engineerComponent);
                }
            }


        }
        else
        {
            Debug.LogWarning("⚠️ Nu ai setat prefab-ul sau punctul de spawn pentru inginer!");
        }
    }


    private void ResetCoinSlots()
    {
        for (int i = 0; i < spawnedCoinVisuals.Length; i++)
        {
            if (spawnedCoinVisuals[i] != null)
            {
                Destroy(spawnedCoinVisuals[i]);
                spawnedCoinVisuals[i] = null;
            }
        }

        coinsInserted = 0;
        spawnedCoinHolders = null; // permite reinserarea

    }
}
