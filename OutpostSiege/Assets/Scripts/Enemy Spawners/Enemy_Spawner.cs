using System.Collections;
using UnityEngine;
using static EnemyBase;

public class Enemy_Spawner : MonoBehaviour
{
    /*
    [Header("Spawn Options")]
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform leftPos, rightPos;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 120f;
    [SerializeField] private int enemiesPerWave = 5;

    [Header("Movement Range")]
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 4f;

    [Header("Difficulty Scaling")]
    [SerializeField] private int enemiesIncrementPerWave = 2; // Number of extra enemies added per wave
    public bool isEnemyDefeated { get; private set; } = false;

    [Header("Enemy Base")]
    [SerializeField] private GameObject leftBase;
    [SerializeField] private GameObject rightBase;

    private EnemyBase leftBaseScript;
    private EnemyBase rightBaseScript;
    private bool leftDestroyed = false;
    private bool rightDestroyed = false;

    private void Start()
    {
        EnemyBase.OnBaseDestroyed += OnBaseDestroyed;

        leftBaseScript = leftBase.GetComponent<EnemyBase>();
        rightBaseScript = rightBase.GetComponent<EnemyBase>();

        StartCoroutine(SpawnEnemiesLoop());
    }

    private void OnDestroy()
    {
        EnemyBase.OnBaseDestroyed -= OnBaseDestroyed;
    }

    private void OnBaseDestroyed(EnemyBase destroyedBase)
    {
        if (destroyedBase == leftBaseScript)
        {
            leftDestroyed = true;
            Debug.Log("Left base destroyed.");
        }           
        else if (destroyedBase == rightBaseScript)
        {
            rightDestroyed = true;
            Debug.Log("Right base destroyed.");
        }
        if (leftDestroyed && rightDestroyed)
        {
            isEnemyDefeated = true;
            Debug.Log("Enemy defeted.");
        }           
    }

    IEnumerator SpawnEnemiesLoop()
    {
        int currentWave = 1;

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int currentEnemies = enemiesPerWave + enemiesIncrementPerWave * (currentWave - 1);

            Debug.Log($"Spawning wave {currentWave} with {currentEnemies} enemies per side.");

            SpawnWave(leftPos, faceRight: true, currentEnemies);
            SpawnWave(rightPos, faceRight: false, currentEnemies);

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
    */
}