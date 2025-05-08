using UnityEngine;

public class Lvl1_Base_Interactions : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject coinHolderPrefab;
    [SerializeField] private GameObject coinPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] coinSpawnPoints;

    private GameObject[] spawnedCoinHolders;
    private GameObject[] spawnedCoinVisuals;
    private int coinsInserted = 0;

    private PlayerInteraction player;
    private bool playerInRange = false;

    private Main_Base_Generator baseGenerator;

    // Se apelează o singură dată, la început. Preia referința către generatorul bazei.
    private void Start()
    {
        baseGenerator = GetComponentInParent<Main_Base_Generator>();
    }

    // Se apelează în fiecare frame. Verifică dacă jucătorul e în zonă și dacă poate plasa monede.
    private void Update()
    {
        if (!playerInRange || player == null) return;

        // Inițializează suporturile și vizualurile dacă nu au fost deja
        if (spawnedCoinHolders == null || spawnedCoinVisuals == null)
        {
            SpawnCoinHolders();
            return;
        }

        // Când jucătorul apasă SPACE, încearcă să introducă o monedă
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (coinsInserted < coinSpawnPoints.Length)
            {
                if (player.TrySpendCoin())
                {
                    // Verificare suplimentară pentru a evita erori
                    if (coinSpawnPoints[coinsInserted] == null || spawnedCoinHolders[coinsInserted] == null)
                    {
                        Debug.LogError($"❌ SpawnPoint sau Holder null la index {coinsInserted}");
                        return;
                    }

                    // Creează vizualul monedei la poziția dorită
                    GameObject coin = Instantiate(
                        coinPrefab,
                        coinSpawnPoints[coinsInserted].position,
                        Quaternion.identity,
                        spawnedCoinHolders[coinsInserted].transform
                    );

                    spawnedCoinVisuals[coinsInserted] = coin;
                    coinsInserted++;

                    // Dacă toate monedele au fost inserate, se poate face upgrade
                    if (coinsInserted == coinSpawnPoints.Length)
                    {
                        Debug.Log("✅ Toate monedele au fost introduse. Baza va fi upgradată automat...");
                        Invoke(nameof(TriggerBaseUpgrade), 0.1f); // Mică întârziere pentru efect
                    }

                }
                else
                {
                    Debug.Log("❌ Nu ai destule monede!");
                }
            }
            else
            {
                Debug.Log("⚠️ Toate monedele au fost deja plasate.");
            }
        }
    }

    private void TriggerBaseUpgrade()
    {
        baseGenerator?.UpgradeBase();
    }


    // Se apelează când jucătorul intră în trigger-ul bazei
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerInteraction>();
            playerInRange = true;

            // Dacă nu există încă suporturi/monede, le generează
            if (spawnedCoinHolders == null || spawnedCoinVisuals == null)
                SpawnCoinHolders();
        }
    }

    // Se apelează când jucătorul iese din trigger-ul bazei
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PlayerInteraction>() == player)
        {
            playerInRange = false;
            baseGenerator?.SetCanUpgrade(false);

            // Dacă nu s-au introdus toate monedele, le returnează jucătorului
            if (coinsInserted > 0 && coinsInserted < coinSpawnPoints.Length)
            {
                player.ReturnCoinsToPlayer(coinsInserted);
                Debug.Log($"🔁 {coinsInserted} monede au fost returnate jucătorului.");
            }

            ResetCoinSlots(); // Resetează starea vizuală și internă
            player = null;

            Debug.Log("❌ Jucătorul a părăsit zona bazei.");
        }
    }

    // Generează obiectele vizuale pentru fiecare locație de plasare a monedelor
    private void SpawnCoinHolders()
    {
        if (coinSpawnPoints == null || coinSpawnPoints.Length == 0)
        {
            Debug.LogError("❌ Nu ai setat coinSpawnPoints în Inspector!");
            return;
        }

        spawnedCoinHolders = new GameObject[coinSpawnPoints.Length];
        spawnedCoinVisuals = new GameObject[coinSpawnPoints.Length];

        for (int i = 0; i < coinSpawnPoints.Length; i++)
        {
            if (coinSpawnPoints[i] == null)
            {
                Debug.LogError($"❌ coinSpawnPoints[{i}] este null!");
                continue;
            }

            // Creează fiecare suport pentru monede la poziția specificată
            spawnedCoinHolders[i] = Instantiate(
                coinHolderPrefab,
                coinSpawnPoints[i].position,
                Quaternion.identity,
                transform
            );
        }

        Debug.Log("✅ Coin holders au fost generați.");
    }

    // Curăță toate monedele și suporturile generate, resetează contorul
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
