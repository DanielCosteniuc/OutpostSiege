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

    [Header("Engineer Spawn Range")]
    [SerializeField] private float minOffsetX = -3f;
    [SerializeField] private float maxOffsetX = 3f;

    private GameObject[] coinHolders;
    private GameObject[] coinVisuals;

    private int coinsInserted = 0;
    private bool playerInRange = false;
    private bool engineerSpawned = false;

    private Player_Interactions player;

    private void Update()
    {
        if (!playerInRange || player == null || engineerSpawned || coinsInserted >= coinSpawnPoints.Length)
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

            if (!engineerSpawned && coinsInserted > 0)
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
            Invoke(nameof(SpawnEngineer), 0.3f);
        }
    }

    private void SpawnEngineer()
    {
        if (engineerPrefab == null || engineerSpawnPoint == null)
        {
            Debug.LogWarning("Engineer prefab or spawn point is not assigned.");
            return;
        }

        Vector3 randomOffset = new Vector3(Random.Range(minOffsetX, maxOffsetX), 0f, 0f);
        GameObject engineer = Instantiate(engineerPrefab, engineerSpawnPoint.position + randomOffset, Quaternion.identity);

        if (player != null && engineer.TryGetComponent(out Engineer engineerComponent))
        {
            player.AddEngineer(engineerComponent);
        }

        engineerSpawned = true;
        ResetCoinVisuals();
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
        engineerSpawned = false;
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
