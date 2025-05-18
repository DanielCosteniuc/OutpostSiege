using System.Collections.Generic;
using UnityEngine;

public class TowerWalls_Generation : MonoBehaviour
{
    [Header("Outposts and Walls")]
    [SerializeField] private List<GameObject> prefabs;

    [Header("Distances")]
    [SerializeField] private float startDistance = 15f;
    [SerializeField] private float endDistance = 70f;

    [Header("Random distance between prefabs (min/max)")]
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 12f;

    [Header("OY coordinates for each prefab")]
    [SerializeField] private List<float> yPositions;  // List of Y positions for each prefab

    void Start()
    {
        PlacePrefabs();
    }

    private void PlacePrefabs()
    {
        if (prefabs.Count != yPositions.Count || prefabs.Count < 2)
        {
            Debug.LogError("The number of prefabs and Y coordinates do not match.");
            return;
        }

        // Place objects on the right side (direction 1)
        PlacePrefabsInDirection(1);

        // Place objects on the left side (direction -1)
        PlacePrefabsInDirection(-1);
    }

    private void PlacePrefabsInDirection(int direction)
    {
        float currentPosition = (direction > 0) ? startDistance : -startDistance;  // Determine the starting position on the X axis
        GameObject lastPrefab = null;
        int samePrefabCount = 0;

        // Continue placing until reaching the limit
        float endPos = (direction > 0) ? endDistance : -endDistance;  // Determine the limit based on direction

        while ((direction > 0 && currentPosition <= endPos) || (direction < 0 && currentPosition >= endPos))
        {
            // Choose a random prefab
            GameObject chosenPrefab = prefabs[Random.Range(0, prefabs.Count)];

            // Check if the same prefab is chosen twice in a row
            if (chosenPrefab == lastPrefab)
            {
                samePrefabCount++;
            }
            else
            {
                samePrefabCount = 1; // Reset to 1 if not the same
            }

            // If there are already two identical objects, change the prefab
            if (samePrefabCount > 2)
            {
                continue;
            }

            // Find the index of the chosen prefab to set the Y value
            int prefabIndex = prefabs.IndexOf(chosenPrefab);
            float yPosition = yPositions[prefabIndex];  // Use the Y coordinate corresponding to the chosen prefab

            // Instantiate the prefab at the current X position and specific Y coordinates
            Instantiate(chosenPrefab, new Vector3(currentPosition, yPosition, 0f), Quaternion.identity, transform);

            // Move to the next random position on the X axis
            currentPosition += Random.Range(minDistance, maxDistance) * direction;  // Multiply by direction to control direction

            // Keep track of the last placed prefab
            lastPrefab = chosenPrefab;
        }
    }
}
