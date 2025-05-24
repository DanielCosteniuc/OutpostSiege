using UnityEngine;

public class Lvl0_Base_Interactions : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject coinHolderPrefab;
    [SerializeField] private GameObject coinPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] coinSpawnPoints;

    private GameObject[] spawnedCoinHolders;
    private GameObject[] spawnedCoinVisuals;
    private int coinsInserted = 0;

    private Player_Interactions player;
    private bool playerInRange = false;

    private Main_Base_Generator baseGenerator;

    private void Start()
    {
        baseGenerator = GetComponentInParent<Main_Base_Generator>();
    }

    private void Update()
    {
        if (!playerInRange || player == null) return;

        // Spawn coin holders and visuals if they haven't been created yet
        if (spawnedCoinHolders == null || spawnedCoinVisuals == null)
        {
            SpawnCoinHolders();
            return;
        }

        // If SPACE is pressed, attempt to insert a coin
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (coinsInserted < coinSpawnPoints.Length)
            {
                if (player.TrySpendCoin())
                {
                    if (coinSpawnPoints[coinsInserted] == null || spawnedCoinHolders[coinsInserted] == null)
                    {
                        Debug.LogError($"SpawnPoint or Holder is null at index {coinsInserted}");
                        return;
                    }

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
                        Debug.Log("All coins inserted. Upgrading base...");
                        Invoke(nameof(TriggerBaseUpgrade), 0.25f); // Time delay after placing the final coin
                    }
                }
                else
                {
                    Debug.Log("Not enough coins!");
                }
            }
            else
            {
                Debug.Log("All coins have already been placed.");
            }
        }
    }

    private void TriggerBaseUpgrade()
    {
        baseGenerator?.UpgradeBase();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player_Interactions>();
            playerInRange = true;

            if (spawnedCoinHolders == null || spawnedCoinVisuals == null)
                SpawnCoinHolders();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Player_Interactions>() == player)
        {
            playerInRange = false;
            baseGenerator?.SetCanUpgrade(false);

            if (coinsInserted > 0 && coinsInserted < coinSpawnPoints.Length)
            {
                player.ReturnCoinsToPlayer(coinsInserted);
                Debug.Log($"{coinsInserted} coins returned to player.");
            }

            ResetCoinSlots();
            player = null;

            Debug.Log("Player left base area.");
        }
    }

    private void SpawnCoinHolders()
    {
        if (coinSpawnPoints == null || coinSpawnPoints.Length == 0)
        {
            Debug.LogError("No coinSpawnPoints assigned in the Inspector.");
            return;
        }

        spawnedCoinHolders = new GameObject[coinSpawnPoints.Length];
        spawnedCoinVisuals = new GameObject[coinSpawnPoints.Length];

        for (int i = 0; i < coinSpawnPoints.Length; i++)
        {
            if (coinSpawnPoints[i] == null)
            {
                Debug.LogError($"coinSpawnPoints[{i}] is null.");
                continue;
            }

            spawnedCoinHolders[i] = Instantiate(
                coinHolderPrefab,
                coinSpawnPoints[i].position,
                Quaternion.identity,
                transform
            );
        }

        Debug.Log("Coin holders spawned.");
    }

    private void ResetCoinSlots()
    {
        if (spawnedCoinVisuals != null)
        {
            for (int i = 0; i < spawnedCoinVisuals.Length; i++)
            {
                if (spawnedCoinVisuals[i] != null)
                {
                    Destroy(spawnedCoinVisuals[i]);
                    spawnedCoinVisuals[i] = null;
                }
            }
        }

        if (spawnedCoinHolders != null)
        {
            for (int i = 0; i < spawnedCoinHolders.Length; i++)
            {
                if (spawnedCoinHolders[i] != null)
                {
                    Destroy(spawnedCoinHolders[i]);
                    spawnedCoinHolders[i] = null;
                }
            }
        }

        coinsInserted = 0;
        spawnedCoinVisuals = null;
        spawnedCoinHolders = null;
    }
}
