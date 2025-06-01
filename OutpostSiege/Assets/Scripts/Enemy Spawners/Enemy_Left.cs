using System.Collections;
using UnityEngine;

public class Enemy_Left : MonoBehaviour
{
    [Header("Spawn Options")]
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform leftPos;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 120f;
    [SerializeField] private int enemiesPerWave = 5;

    [Header("Movement Range")]
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 4f;

    [Header("Difficulty Scaling")]
    [SerializeField] private int enemiesIncrementPerWave = 2; // Number of extra enemies added per wave

    private void Start()
    {
        StartCoroutine(SpawnEnemiesLoop()); 
    }

    IEnumerator SpawnEnemiesLoop()
    {
        int currentWave = 1;

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int currentEnemies = enemiesPerWave + enemiesIncrementPerWave * (currentWave - 1);
            Debug.Log($"Spawning wave {currentWave} with {currentEnemies} enemies.");
            SpawnWave(leftPos, faceRight: true, currentEnemies);

            currentWave++;
        }
    }

    void SpawnWave(Transform spawnPoint, bool faceRight, int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject prefab = enemies[Random.Range(0, enemies.Length)];
            GameObject instance = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            if (!faceRight)
            {
                instance.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            if (instance.TryGetComponent(out Infantry_Enemy enemy))
            {
                enemy.moveSpeed = Random.Range(minSpeed, maxSpeed);
                enemy.moveDirection = faceRight ? 1 : -1;
            }
        }
    }
}
