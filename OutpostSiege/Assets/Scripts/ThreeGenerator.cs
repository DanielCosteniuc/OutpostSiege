using System.Collections.Generic;
using UnityEngine;

public class RandomTreeSpawner : MonoBehaviour
{
    [Header("🌲 Prefaburi Copaci")]
    [Tooltip("Lista cu toate prefabricatele de copaci disponibile")]
    [SerializeField] private List<GameObject> treePrefabs;

    [Header("📏 Distanțe și Setări Spawn")]
    [Tooltip("Distanța de la centru unde încep să fie plasați copacii")]
    [SerializeField] private float startDistance = 10f;

    [Tooltip("Distanța maximă până unde se pot întinde copacii, TREBUIE SA FIE 2*GROUNDLENGTH")]
    [SerializeField] private float stopDistance = 100f;

    [Tooltip("Numărul minim de copaci pe o direcție")]
    [SerializeField] private int minTrees = 10;

    [Tooltip("Numărul maxim de copaci pe o direcție")]
    [SerializeField] private int maxTrees = 15;

    [Tooltip("Înălțimea la care se vor plasa copacii (Y)")]
    [SerializeField] private float treeYPosition = 1.3f;

    [Header("↔️ Spațiere Copaci (automat în funcție de maxTrees)")]
    [SerializeField] private float minTreeSpacing;
    [SerializeField] private float maxTreeSpacing;

    void Start()
    {


        SpawnTrees(1);  // Dreapta
        SpawnTrees(-1); // Stânga
    }

    void SpawnTrees(int direction)
    {
        
        Vector3 startPosition = transform.position;
        

        int treeCount = Random.Range(minTrees, maxTrees + 1);
        float currentX =  (startDistance * direction);

        for (int i = 0; i < treeCount; i++)
        {
            float treeSpacing = Random.Range(minTreeSpacing, maxTreeSpacing);
            if (i > 0)
            {
                currentX += treeSpacing * direction;
            }
            if (Mathf.Abs(currentX) < stopDistance)
            {
                SpawnTree(currentX, startPosition.z, i);
            }
            else
            {
                break;
            }

        }
    }



    void SpawnTree(float x, float z, int index)
    {
        if (treePrefabs == null || treePrefabs.Count == 0)
        {
            Debug.LogWarning("Lista de prefaburi de copaci este goală!");
            return;
        }

        GameObject selectedPrefab = treePrefabs[Random.Range(0, treePrefabs.Count)];
        if (selectedPrefab != null)
        {
            float randomZ = Random.Range(z - 5f, z + 5f);
            Vector3 treePosition = new Vector3(x, treeYPosition, randomZ);
            GameObject tree = Instantiate(selectedPrefab, treePosition, Quaternion.identity);
            tree.name = "Tree" + (index + 1);
        }
    }
}