using System.Collections.Generic;
using UnityEngine;

public class TowerOutpostsGenerator : MonoBehaviour
{
    [Header("Outposts Walls etc.")]
    [SerializeField] private List<GameObject> prefabs;

    [Header("Distante")]
    [SerializeField] private float startDistance = 15f;
    [SerializeField] private float endDistance = 70f;

    [Header("Distanta random intre prefabs (min/max)")]
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 12f;

    [Header("Coordonate Y pentru fiecare prefab")]
    [SerializeField] private List<float> yPositions;  // Lista de Y-uri pentru fiecare prefab

    void Start()
    {
        PlacePrefabs();
    }

    private void PlacePrefabs()
    {
        if (prefabs.Count != yPositions.Count || prefabs.Count < 2)
        {
            Debug.LogError("Numărul de prefabs și coordonatele Y nu se potrivesc.");
            return;
        }

        // Plasează obiectele pe partea dreaptă (direcția 1)
        PlacePrefabsInDirection(1);

        // Plasează obiectele pe partea stângă (direcția -1)
        PlacePrefabsInDirection(-1);
    }

    private void PlacePrefabsInDirection(int direction)
    {
        float currentPosition = (direction > 0) ? startDistance : -startDistance;  // Determină poziția de start pe axa X
        GameObject lastPrefab = null;
        int samePrefabCount = 0;

        // Continuă plasarea până ajungi la limită
        float endPos = (direction > 0) ? endDistance : -endDistance;  // Determină limita în funcție de direcție

        while ((direction > 0 && currentPosition <= endPos) || (direction < 0 && currentPosition >= endPos))
        {
            // Alege un prefab aleator
            GameObject chosenPrefab = prefabs[Random.Range(0, prefabs.Count)];

            // Verifică dacă alegerea unui prefab identic de două ori la rând
            if (chosenPrefab == lastPrefab)
            {
                samePrefabCount++;
            }
            else
            {
                samePrefabCount = 1; // Reset la 1 dacă nu e același
            }

            // Dacă sunt deja două obiecte identice, schimbă prefab-ul
            if (samePrefabCount > 2)
            {
                continue;
            }

            // Găsește indexul prefab-ului ales pentru a seta valoarea Y
            int prefabIndex = prefabs.IndexOf(chosenPrefab);
            float yPosition = yPositions[prefabIndex];  // Folosește coordonata Y corespunzătoare prefab-ului ales

            // Instanțiază prefab-ul la poziția curentă pe axa X și coordonatele Y specifice
            Instantiate(chosenPrefab, new Vector3(currentPosition, yPosition, 0f), Quaternion.identity, transform);

            // Avansează la următoarea poziție aleatorie pe axa X
            currentPosition += Random.Range(minDistance, maxDistance) * direction;  // Înmulțim cu direction pentru a controla direcția

            // Păstrează recordul pentru ultimul prefab plasat
            lastPrefab = chosenPrefab;
        }
    }

}
