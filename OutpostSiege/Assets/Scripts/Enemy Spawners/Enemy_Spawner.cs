using System.Collections;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [Header("Spawn Options")]
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform leftPos, rightPos;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 120f;
    [SerializeField] private int enemiesPerWave = 5;

    [Header("Movement Range")]
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 4f;

    private void Start()
    {
        StartCoroutine(SpawnEnemiesLoop());
    }

    IEnumerator SpawnEnemiesLoop()
    {
        while (true)
        {           
            yield return new WaitForSeconds(spawnInterval);
            SpawnWave(leftPos, faceRight: true);
            SpawnWave(rightPos, faceRight: false);
        }
    }

    void SpawnWave(Transform spawnPoint, bool faceRight)
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject prefab = enemies[Random.Range(0, enemies.Length)];
            GameObject instance = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            if (!faceRight)
            {
                instance.transform.Rotate(0f, 180f, 0f); // Visual flip
            }

            if (instance.TryGetComponent(out Infantry_Enemy enemy))
            {
                enemy.moveSpeed = Random.Range(minSpeed, maxSpeed);
                enemy.moveDirection = faceRight ? 1 : -1; // This makes it move in correct direction
            }
        }
    }

}