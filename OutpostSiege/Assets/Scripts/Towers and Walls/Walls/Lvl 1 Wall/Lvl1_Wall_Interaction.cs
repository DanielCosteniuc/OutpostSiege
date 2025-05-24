using UnityEngine;
using System.Collections.Generic;

public class Lvl1_Wall_Interaction : MonoBehaviour
{
    [Header("Coin Setup")]
    public GameObject coinHolderPrefab;
    public GameObject coinPrefab;
    public List<Transform> coinSpawnPoints;

    [Header("Tree Blocking Settings")]
    [SerializeField] private float blockRadius = 5f;

    private List<GameObject> coinInstances = new();
    private bool isPaid = false;
    private int coinsInserted = 0;

    private Player_Interactions player;
    private TowerWalls_Generation wallGenerator;

    [Header("Upgrade Settings")]
    public int wallLevel = 1;
    public int coinsRequired => coinSpawnPoints.Count;

    private void Start()
    {
        coinInstances.Clear();
        player = GameObject.FindWithTag("Player").GetComponent<Player_Interactions>();
        wallGenerator = FindFirstObjectByType<TowerWalls_Generation>();
    }

    private void Update()
    {
        if (!isPaid && coinInstances.Count > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            if (player.TrySpendCoin())
            {
                Transform holderTransform = coinInstances[coinsInserted].transform;
                Instantiate(coinPrefab, holderTransform.position, Quaternion.identity, holderTransform);

                coinsInserted++;

                if (coinsInserted >= coinsRequired)
                {
                    isPaid = true;
                    UpgradeWall();
                }
            }
        }
    }

    private void UpgradeWall()
    {
        if (Engineer_Wall_Manager.Instance != null)
        {
            Engineer_Wall_Manager.Instance.AddWallTask(this, OnWallConstructed);
        }
        else
        {
            Debug.LogWarning("Wall_Task_Manager lipsã.");
        }
    }

    private void OnWallConstructed(GameObject wallObject)
    {
        if (wallGenerator == null || wallLevel + 1 >= wallGenerator.WallPrefabs.Count)
        {
            Debug.LogWarning("Nu existã gard de nivel superior.");
            return;
        }

        Vector3 currentPos = transform.position;
        Quaternion rotation = transform.rotation;
        Transform parent = transform.parent;

        GameObject newWall = Instantiate(
            wallGenerator.WallPrefabs[wallLevel + 1],
            currentPos,
            rotation,
            parent
        );

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPaid && coinInstances.Count == 0)
        {
            if (AreTreesNearby()) return;

            foreach (Transform spawnPoint in coinSpawnPoints)
            {
                var coin = Instantiate(coinHolderPrefab, spawnPoint.position, Quaternion.identity, transform);
                coinInstances.Add(coin);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!isPaid)
        {
            player.ReturnCoinsToPlayer(coinsInserted);

            foreach (var coin in coinInstances)
            {
                Destroy(coin);
            }

            coinInstances.Clear();
            coinsInserted = 0;
        }
        else
        {
            // New behavior: remove coin holders when paid
            foreach (var coin in coinInstances)
            {
                Destroy(coin);
            }

            coinInstances.Clear();
        }
    }

    private bool AreTreesNearby()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, blockRadius);
        foreach (Collider2D col in nearbyObjects)
        {
            if (col.CompareTag("Tree")) return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, blockRadius);
    }
#endif
}
