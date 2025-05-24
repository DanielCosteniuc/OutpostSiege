using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [Header("Tree Prefabs")]
    [SerializeField] private GameObject[] treePrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float minDistance = 4f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private float xLimit = 50f;
    [SerializeField] private float safeZoneWidth = 10f; // New: half-width of spawn zone in the center

    private void Start()
    {
        SpawnTrees();
    }

    private void SpawnTrees()
    {
        // LEFT SIDE
        float xLeft = -safeZoneWidth;
        while (xLeft >= -xLimit)
        {
            xLeft -= Random.Range(minDistance, maxDistance);
            Vector3 position = new Vector3(xLeft, yOffset, 0f);
            SpawnRandomTree(position);
        }

        // RIGHT SIDE
        float xRight = safeZoneWidth;
        while (xRight <= xLimit)
        {
            Vector3 position = new Vector3(xRight, yOffset, 0f);
            SpawnRandomTree(position);
            xRight += Random.Range(minDistance, maxDistance);
        }
    }

    private void SpawnRandomTree(Vector3 position)
    {
        GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
        Instantiate(treePrefab, position, Quaternion.identity, transform);
    }
}

