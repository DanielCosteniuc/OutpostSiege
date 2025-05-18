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

    private GameObject[] spawnedCoinHolders;
    private GameObject[] spawnedCoinVisuals;
    private int coinsInserted = 0;

    private Player_Interactions player;
    private bool playerInRange = false;
    private bool soldierBought = false;

    private void Update()
    {
        if (!playerInRange || player == null)
            return;

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
                    Invoke(nameof(SpawnSoldier), 0.3f);
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

            if (!soldierBought && coinsInserted > 0 && coinsInserted < coinSpawnPoints.Length)
            {
                player.ReturnCoinsToPlayer(coinsInserted);

                for (int i = 0; i < coinsInserted; i++)
                {
                    if (spawnedCoinVisuals != null && spawnedCoinVisuals[i] != null)
                    {
                        Destroy(spawnedCoinVisuals[i]);
                        spawnedCoinVisuals[i] = null;
                    }
                }

                Debug.Log($"🔁 {coinsInserted} monede au fost returnate jucătorului.");
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

            spawnedCoinHolders = null;
            spawnedCoinVisuals = null;
            coinsInserted = 0;
            soldierBought = false;
            player = null;

            Debug.Log("❌ Jucătorul a părăsit zona de interacțiune.");
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

    private void SpawnSoldier()
    {
        if (infantryPrefab != null && infantrySpawnPoint != null)
        {
            // Adaugă o variație random pe axa X (între -5 și +5)
            Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), 0f, 0f);
            Vector3 spawnPosition = infantrySpawnPoint.position + randomOffset;

            GameObject newInfantry = Instantiate(infantryPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("💂 Soldatul a fost generat!");

            soldierBought = true;

            if (player != null)
            {
                Infantry infantryComponent = newInfantry.GetComponent<Infantry>();
                if (infantryComponent != null)
                {
                    player.AddInfantry(infantryComponent);
                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Nu ai setat prefab-ul sau punctul de spawn pentru infanterie!");
        }

        ResetCoinSlots();
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
        spawnedCoinHolders = null;
        spawnedCoinVisuals = null;
    }
}
