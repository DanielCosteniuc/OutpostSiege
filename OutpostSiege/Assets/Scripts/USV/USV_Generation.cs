using UnityEngine;

public class USV_Generation : MonoBehaviour
{
    [SerializeField] private GameObject structurePrefab;

    [Header("Horizontal Bounds")]
    [SerializeField] private float minXLeft = -10f;
    [SerializeField] private float maxXLeft = -5f;
    [SerializeField] private float minXRight = 5f;
    [SerializeField] private float maxXRight = 10f;

    [Header("Vertical Placement")]
    [SerializeField] private float yPosition = 0f;

    private void Start()
    {
        SpawnStructure();
    }

    public void SpawnStructure()
    {
        // Randomly choose left or right side
        bool spawnLeft = Random.value < 0.5f;

        float randomX;
        if (spawnLeft)
        {
            randomX = Random.Range(minXLeft, maxXLeft);
        }
        else
        {
            randomX = Random.Range(minXRight, maxXRight);
        }

        Vector3 spawnPos = new Vector3(randomX, yPosition, 0f);
        Instantiate(structurePrefab, spawnPos, Quaternion.identity);
    }
}
