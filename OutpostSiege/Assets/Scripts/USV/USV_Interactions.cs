using System.Collections.Generic;
using UnityEngine;

public class USV_Interactions : MonoBehaviour
{
    // Coin Setup
    [Header("Coin Setup")]
    [SerializeField] private GameObject coinHolderPrefab;
    [SerializeField] private GameObject CoinPrefab;
    public List<Transform> coinSpawnPoints;

    private List<GameObject> coinInstances = new();
    private bool isPaid = false;
    private int coinsInserted = 0;

    [Header("Tree Blocking Settings")]
    [SerializeField] private Vector2 treeBlockSize = new Vector2(3f, 2f); // Width & Height
    [SerializeField] private string treeTag = "Tree";
    [SerializeField] private Transform treeBlockCenterPoint; // Detection area center

    [Header("Player")]
    private Player_Interactions player;

    public int CoinsRequired => coinSpawnPoints.Count;

    private void Start()
    {
        coinInstances.Clear();
        player = GameObject.FindWithTag("Player").GetComponent<Player_Interactions>();
    }

    private void Update()
    {
        if (!isPaid && coinInstances.Count > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            if (player != null && player.TrySpendCoin())
            {
                Transform holderTransform = coinInstances[coinsInserted].transform;
                Instantiate(CoinPrefab, holderTransform.position, Quaternion.identity, holderTransform);

                coinsInserted++;

                if (coinsInserted >= CoinsRequired)
                {
                    isPaid = true;
                    OnPaymentCompleted(); // Ai toate monedele, execută acțiunea
                }
            }
        }
    }

    private void OnPaymentCompleted()
    {
        Debug.Log("[USV_Interactions] Toate monedele au fost plasate, ai deblocat USV-ul!");
        // Aici poți adăuga efecte, sunete, activare altor obiecte etc.
    }

    public bool IsPaid() => isPaid;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isPaid || IsTreeNearby() || coinInstances.Count > 0)
            return;

        foreach (Transform spawnPoint in coinSpawnPoints)
        {
            var coin = Instantiate(coinHolderPrefab, spawnPoint.position, Quaternion.identity, transform);
            coinInstances.Add(coin);
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
            // Remove visual coin holders if already paid
            foreach (var coin in coinInstances)
            {
                Destroy(coin);
            }

            coinInstances.Clear();
        }
    }

    private bool IsTreeNearby()
    {
        Vector2 center = treeBlockCenterPoint != null ? (Vector2)treeBlockCenterPoint.position : (Vector2)transform.position;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, treeBlockSize, 0f);
        foreach (var col in colliders)
        {
            if (col.CompareTag(treeTag))
                return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (treeBlockCenterPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(treeBlockCenterPoint.position, treeBlockSize);
        }
    }
#endif
}
