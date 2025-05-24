using UnityEngine;

public class Lvl1_Infantry_Tent : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject coinHolderPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject infantryPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] coinSpawnPoints;
    [SerializeField] private Transform infantrySpawnPoint;

    [Header("Infantry Spawn Range")]
    [SerializeField] private float minOffsetX = -3f;
    [SerializeField] private float maxOffsetX = 3f;

    private GameObject[] coinHolders;
    private GameObject[] coinVisuals;

    private int coinsInserted = 0;
    private bool playerInRange = false;
    private bool infantrySpawned = false;

    private Player_Interactions player;

    private void Update()
    {
        if (!playerInRange || player == null || infantrySpawned || coinsInserted >= coinSpawnPoints.Length)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && player.TrySpendCoin())
        {
            PlaceCoin();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && coinHolders == null)
        {
            player = other.GetComponent<Player_Interactions>();
            playerInRange = true;
            SpawnCoinHolders();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Player_Interactions>() == player)
        {
            playerInRange = false;

            if (!infantrySpawned && coinsInserted > 0)
            {
                ReturnCoinsToPlayer();
            }

            Cleanup();
        }
    }

    private void SpawnCoinHolders()
    {
        coinHolders = new GameObject[coinSpawnPoints.Length];
        coinVisuals = new GameObject[coinSpawnPoints.Length];

        for (int i = 0; i < coinSpawnPoints.Length; i++)
        {
            coinHolders[i] = Instantiate(coinHolderPrefab, coinSpawnPoints[i].position, Quaternion.identity, transform);
        }
    }

    private void PlaceCoin()
    {
        GameObject coin = Instantiate(coinPrefab, coinSpawnPoints[coinsInserted].position, Quaternion.identity, coinHolders[coinsInserted].transform);
        coinVisuals[coinsInserted] = coin;
        coinsInserted++;

        if (coinsInserted == coinSpawnPoints.Length)
        {
            Invoke(nameof(SpawnInfantry), 0.3f);
        }
    }

    private void SpawnInfantry()
    {
        if (infantryPrefab == null || infantrySpawnPoint == null)
        {
            Debug.LogWarning("Infantry prefab or spawn point is not assigned.");
            return;
        }

        Vector3 randomOffset = new Vector3(Random.Range(minOffsetX, maxOffsetX), 0f, 0f);
        GameObject infantry = Instantiate(infantryPrefab, infantrySpawnPoint.position + randomOffset, Quaternion.identity);

        if (player != null && infantry.TryGetComponent(out Infantry infantryComponent))
        {
            player.AddInfantry(infantryComponent);
        }

        ResetCoinVisuals();
        ResetCoinSystem(); // 👈 Adaugă această linie pentru reset
    }

    private void ResetCoinSystem()
    {
        coinsInserted = 0;

        if (coinVisuals != null)
        {
            for (int i = 0; i < coinVisuals.Length; i++)
            {
                if (coinVisuals[i] != null)
                {
                    Destroy(coinVisuals[i]);
                    coinVisuals[i] = null;
                }
            }
        }

        if (coinHolders != null)
        {
            for (int i = 0; i < coinHolders.Length; i++)
            {
                if (coinHolders[i] != null)
                {
                    Destroy(coinHolders[i]);
                    coinHolders[i] = null;
                }
            }
        }

        SpawnCoinHolders(); // 👈 recreează vizualul pentru următoarea achiziție
    }


    private void ReturnCoinsToPlayer()
    {
        player.ReturnCoinsToPlayer(coinsInserted);

        for (int i = 0; i < coinsInserted; i++)
        {
            if (coinVisuals[i] != null)
            {
                Destroy(coinVisuals[i]);
                coinVisuals[i] = null;
            }
        }

        coinsInserted = 0;
    }

    private void Cleanup()
    {
        if (coinVisuals != null)
        {
            foreach (var visual in coinVisuals)
            {
                if (visual != null) Destroy(visual);
            }
        }

        if (coinHolders != null)
        {
            foreach (var holder in coinHolders)
            {
                if (holder != null) Destroy(holder);
            }
        }

        coinHolders = null;
        coinVisuals = null;
        coinsInserted = 0;
        infantrySpawned = false;
        player = null;
    }

    private void ResetCoinVisuals()
    {
        if (coinVisuals == null) return;

        foreach (var coin in coinVisuals)
        {
            if (coin != null)
                Destroy(coin);
        }

        coinVisuals = null;
        coinsInserted = 0;
    }
}
