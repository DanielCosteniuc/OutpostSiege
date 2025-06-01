using System.Collections.Generic;
using UnityEngine;

public class Wall_Manager : MonoBehaviour
{
    [Header("Wall Types to Track")]
    [SerializeField] private List<GameObject> validWallPrefabs; // Assign wall prefabs here

    [Header("Position Adjustment")]
    [SerializeField] private float positionOffset = 1f; // Offset pentru a nu merge chiar in perete
    public float PositionOffset => positionOffset;

    private GameObject lastLeftWall;
    private GameObject lastRightWall;

    private GameObject previousLeftWall;
    private GameObject previousRightWall;

    void Update()
    {
        UpdateLastWalls();
        //Infantry_Manager.Instance.AssignIdleInfantryToNewWall();

    }

    void UpdateLastWalls()
    {
        GameObject[] allWalls = GameObject.FindGameObjectsWithTag("Wall");
        // Debug.Log($"[Wall_Manager] Found {allWalls.Length} wall objects.");

        List<GameObject> leftWalls = new List<GameObject>();
        List<GameObject> rightWalls = new List<GameObject>();

        foreach (GameObject wall in allWalls)
        {
            if (!IsValidWall(wall))
            {
                //Debug.Log($"[Wall_Manager] Skipping invalid wall: {wall.name}");
                continue;
            }

            float xPos = wall.transform.position.x;
            if (xPos < 0)
            {
                leftWalls.Add(wall);
            }
            else if (xPos > 0)
            {
                rightWalls.Add(wall);
            }
        }

        //Debug.Log($"[Wall_Manager] LeftWalls: {leftWalls.Count}, RightWalls: {rightWalls.Count}");

        lastLeftWall = GetFurthestLeftWall(leftWalls);
        lastRightWall = GetFurthestRightWall(rightWalls);

        bool wallChanged = false;

        if (lastLeftWall != previousLeftWall)
        {
            //Debug.Log($"[Wall_Manager] Last Left Wall changed: {(lastLeftWall != null ? lastLeftWall.name : "None")}");
            previousLeftWall = lastLeftWall;
            wallChanged = true;
        }

        if (lastRightWall != previousRightWall)
        {
            //Debug.Log($"[Wall_Manager] Last Right Wall changed: {(lastRightWall != null ? lastRightWall.name : "None")}");
            previousRightWall = lastRightWall;
            wallChanged = true;
        }

        if (wallChanged && Infantry_Manager.Instance != null)
        {
            Infantry_Manager.Instance.AssignIdleInfantryToNewWall();
            Infantry_Manager.Instance.ReassignMovingInfantryToWalls(); // Actualizeaza cei deja în miscare
        }

        if (!(leftWalls != null) || !(rightWalls != null))
        {
            Debug.Log("[Wall_Manager] Wall null+++++");
            Infantry_Manager.Instance.AssignIdleInfantryToNewWall();
        }
    }

    bool IsValidWall(GameObject wall)
    {
        foreach (GameObject prefab in validWallPrefabs)
        {
            if (wall.name.Contains(prefab.name)) return true;
        }
        return false;
    }

    GameObject GetFurthestLeftWall(List<GameObject> walls)
    {
        GameObject furthest = null;
        float lowestX = float.MaxValue;

        foreach (GameObject wall in walls)
        {
            float x = wall.transform.position.x;
            if (x < lowestX)
            {
                lowestX = x;
                furthest = wall;
            }
        }

        return furthest;
    }

    GameObject GetFurthestRightWall(List<GameObject> walls)
    {
        GameObject furthest = null;
        float highestX = float.MinValue;

        foreach (GameObject wall in walls)
        {
            float x = wall.transform.position.x;
            if (x > highestX)
            {
                highestX = x;
                furthest = wall;
            }
        }

        return furthest;
    }

    public Vector3 GetAdjustedWallPosition(GameObject wall)
    {
        if (wall == null) return Vector3.zero;

        Vector3 pos = wall.transform.position;

        if (pos.x < 0) // zid stanga
        {
            return new Vector3(pos.x + positionOffset, pos.y, pos.z);
        }
        else if (pos.x > 0) // zid dreapta
        {
            return new Vector3(pos.x - positionOffset, pos.y, pos.z);
        }

        return pos;
    }

    public GameObject GetLastLeftWall() => lastLeftWall;
    public GameObject GetLastRightWall() => lastRightWall;
}