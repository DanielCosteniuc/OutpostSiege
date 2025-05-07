using UnityEngine;

public class Lvl1_Base_Interactions : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject coinHolderPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform[] coinSpawnPoints;

    private GameObject[] coinHolders;
    private GameObject[] coinVisuals;
    private int coinsInserted = 0;

    private Player player;
    private bool playerInRange = false;

    private Main_Base_Generator baseGenerator;

    private void Start()
    {
        baseGenerator = GetComponentInParent<Main_Base_Generator>();
        coinHolders = new GameObject[coinSpawnPoints.Length];
        coinVisuals = new GameObject[coinSpawnPoints.Length];
    }

    private void Update()
    {
        if (!playerInRange || player == null) return;

        if (Input.GetKeyDown(KeyCode.Space) && coinsInserted < coinSpawnPoints.Length)
        {
            if (player.TrySpendCoin())
            {
                GameObject coin = Instantiate(coinPrefab, coinSpawnPoints[coinsInserted].position, Quaternion.identity, coinHolders[coinsInserted].transform);
                coinVisuals[coinsInserted] = coin;
                coinsInserted++;

                if (coinsInserted == coinSpawnPoints.Length)
                {
                    baseGenerator?.SetCanUpgrade(true);
                    Debug.Log("✅ Baza este gata de upgrade!");
                }
            }
            else
            {
                Debug.Log("❌ Nu ai destule monede!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            playerInRange = true;

            if (coinHolders[0] == null)
                SpawnCoinHolders();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Player>() == player)
        {
            playerInRange = false;
            baseGenerator?.SetCanUpgrade(false);

            if (coinsInserted < coinSpawnPoints.Length)
            {
                player.ReturnCoinsToPlayer(coinsInserted);

                for (int i = 0; i < coinsInserted; i++)
                {
                    if (coinVisuals[i] != null)
                        Destroy(coinVisuals[i]);
                }

                coinsInserted = 0;
            }

            player = null;
        }
    }

    private void SpawnCoinHolders()
    {
        for (int i = 0; i < coinSpawnPoints.Length; i++)
        {
            coinHolders[i] = Instantiate(coinHolderPrefab, coinSpawnPoints[i].position, Quaternion.identity, transform);
        }
    }
}
