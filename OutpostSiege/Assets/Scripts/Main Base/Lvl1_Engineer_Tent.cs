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

    private Player_Interactions player;
    private bool playerInRange = false;
    private bool engineerBought = false;

    //valorile min max intre care sa fie dropati inginerii
    private int minSpawn = -3;
    private int maxSpawn = 3;

    private void Update()
    {
        // Verifică dacă jucătorul este în zona de coliziune
        if (!playerInRange || player == null)
            return;

        // Dacă s-au resetat (după ce ai cumpărat un inginer), recreează suporturile de monede
        if (spawnedCoinHolders == null && playerInRange)
        {
            SpawnCoinHolders();
        }

        // Permite interacțiunea doar dacă jucătorul este în zona de coliziune și mai sunt locuri pentru monede
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
                    Invoke(nameof(SpawnEngineer), 0.3f);
                    //ResetCoinSlots();
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

            // Returnează monedele doar dacă nu a fost cumpărat inginerul și s-au introdus doar parțial
            if (!engineerBought && coinsInserted > 0 && coinsInserted < coinSpawnPoints.Length)
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

            // Șterge vizualurile și holder-ele de monede
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

            // Reset complet
            spawnedCoinHolders = null;
            spawnedCoinVisuals = null;
            coinsInserted = 0;
            engineerBought = false;
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

    private void SpawnEngineer()
    {
        if (engineerPrefab != null && engineerSpawnPoint != null)
        {
            //loc random pt spawn
            Vector3 randomOffset = new Vector3(Random.Range(minSpawn,maxSpawn), 0f, 0f);
            Vector3 spawnPosition = engineerSpawnPoint.position + randomOffset;


            GameObject newEngineer = Instantiate(engineerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("👷 Inginerul a fost generat!");

            engineerBought = true;

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

        ResetCoinSlots(); // Se face după delay, odată cu spawn-ul
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
