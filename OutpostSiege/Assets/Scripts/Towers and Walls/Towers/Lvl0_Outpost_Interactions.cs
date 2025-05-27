using System.Collections.Generic;
using UnityEngine;

public class Lvl0_Outpost_Interaction : MonoBehaviour
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
    private TowerWalls_Generation outpostGenerator;

    [Header("Upgrade Settings")]
    public int outpostLevel = 0;
    public int coinsRequired => coinSpawnPoints.Count;

    // Referință către USV
    private USV_Interactions usv;

    private void Start()
    {
        coinInstances.Clear();
        player = GameObject.FindWithTag("Player").GetComponent<Player_Interactions>();
        outpostGenerator = FindFirstObjectByType<TowerWalls_Generation>();
        usv = FindFirstObjectByType<USV_Interactions>();
    }

    private void Update()
    {
        if (!isPaid && coinInstances.Count > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            if (usv == null || !usv.IsPaid()) return; // Nu permite plată fără USV

            if (player.TrySpendCoin())
            {
                Transform holderTransform = coinInstances[coinsInserted].transform;
                Instantiate(coinPrefab, holderTransform.position, Quaternion.identity, holderTransform);

                coinsInserted++;

                if (coinsInserted >= coinsRequired)
                {
                    isPaid = true;
                    UpgradeOutpost();
                }
            }
        }
    }

    private void UpgradeOutpost()
    {
        Debug.Log("All coins inserted, upgrading outpost...");

        if (Engineer_Outpost_Manager.Instance != null)
        {
            Engineer_Outpost_Manager.Instance.AddOutpostTask(this, OnOutpostConstructed);
        }
        else
        {
            Debug.LogWarning("Engineer_Outpost_Manager.Instance is null!");
        }
    }

    private void OnOutpostConstructed(GameObject outpostObject)
    {
        if (outpostGenerator == null || outpostLevel + 1 >= outpostGenerator.TowerPrefabs.Count)
        {
            Debug.LogWarning("Nu există avanpost de nivel superior.");
            return;
        }

        Vector3 currentPos = transform.position;
        Quaternion rotation = transform.rotation;
        Transform parent = transform.parent;

        GameObject newOutpost = Instantiate(
            outpostGenerator.TowerPrefabs[outpostLevel + 1],
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
            if (usv == null || !usv.IsPaid()) return; // Nu arătăm holders dacă USV nu e plătit
            if (AreTreesNearby()) return;

            foreach (Transform spawnPoint in coinSpawnPoints)
            {
                var holder = Instantiate(coinHolderPrefab, spawnPoint.position, Quaternion.identity, transform);
                coinInstances.Add(holder);
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
