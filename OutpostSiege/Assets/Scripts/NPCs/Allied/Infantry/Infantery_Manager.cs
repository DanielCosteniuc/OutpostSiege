using System.Collections.Generic;
using UnityEngine;

public class Infantry_Manager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject infantryPrefab;
    [SerializeField] private Wall_Manager wallManager;
    [SerializeField] private GameObject spawnPoint;

    //private readonly List<Infantry> activeInfantry = new();
    private readonly List<Infantry> leftInfantry = new();
    private readonly List<Infantry> rightInfantry = new();

    public static Infantry_Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("[Infantry_Manager] Awake called and Instance assigned.");
    }
    private void Update()
    {
        CorrectInfantryFacing();
    }

    public void SpawnInfantry()
    {
        if (infantryPrefab == null || spawnPoint == null || wallManager == null)
        {
            Debug.LogWarning("[Infantry_Manager] Missing reference(s)!");
            return;
        }

        Vector3 spawnPos = spawnPoint.transform.position + new Vector3(Random.Range(-3f, 3f), 0f, 0f);
        GameObject go = Instantiate(infantryPrefab, spawnPos, Quaternion.identity);
        Infantry infantry = go.GetComponent<Infantry>();

        if (infantry == null)
        {
            Debug.LogError("[Infantry_Manager] Spawned prefab doesn't contain Infantry script.");
            return;
        }

        // 1. Alocăm direcția încă de la început
        if (leftInfantry.Count <= rightInfantry.Count)
        {
            leftInfantry.Add(infantry);
            infantry.SetTargetDirection(Infantry.Direction.Left);
        }
        else
        {
            rightInfantry.Add(infantry);
            infantry.SetTargetDirection(Infantry.Direction.Right);
        }

        // 2. Dacă gardul în direcția respectivă există, mutăm infanteria
        GameObject wall = infantry.TargetDirection == Infantry.Direction.Left
            ? wallManager.GetLastLeftWall()
            : wallManager.GetLastRightWall();

        if (wall != null)
        {
            Vector3 targetPos = wallManager.GetAdjustedWallPosition(wall) + RandomOffset();
            infantry.MoveTo(targetPos);
        }
        
    }

    public void OnInfantryDeath(Infantry infantry)
    {
        if (infantry == null) return;

        //activeInfantry.Remove(infantry);
        leftInfantry.Remove(infantry);
        rightInfantry.Remove(infantry);
    }

    // 🔄 Apelată când se construiește un gard nou
    public void AssignIdleInfantryToNewWall()
    {
        GameObject leftWall = wallManager.GetLastLeftWall();
        GameObject rightWall = wallManager.GetLastRightWall();

        //Debug.Log($"[Infantry_Manager] Reassigning infantry: LeftWall={(leftWall != null ? leftWall.name : "null")}, RightWall={(rightWall != null ? rightWall.name : "null")}");

        foreach (Infantry infantry in leftInfantry)
        {
            if (!infantry.HasMoved)
            {
                Debug.Log($"[Infantry_Manager] Reassigning idle LEFT infantry: {infantry.name}");

                if (leftWall != null)
                {
                    Vector3 pos = wallManager.GetAdjustedWallPosition(leftWall) + new Vector3(wallManager.PositionOffset, 0f, 0f);
                    //Debug.Log($"[Infantry_Manager] → Moving LEFT infantry to wall at: {pos}");
                    infantry.MoveTo(pos);
                }
                
            }
        }

        foreach (Infantry infantry in rightInfantry)
        {
            if (!infantry.HasMoved)
            {
                Debug.Log($"[Infantry_Manager] Reassigning idle RIGHT infantry: {infantry.name}");

                if (rightWall != null)
                {
                    Vector3 pos = wallManager.GetAdjustedWallPosition(rightWall) - new Vector3(wallManager.PositionOffset, 0f, 0f);
                    //Debug.Log($"[Infantry_Manager] → Moving RIGHT infantry to wall at: {pos}");
                    infantry.MoveTo(pos);
                }
                
            }
        }
    }


    public void CorrectInfantryFacing()
    {
        foreach (var infantry in rightInfantry)
        {
            bool isIdle = !infantry.HasMoved;

            if (isIdle)
            {
                bool isOnRightSide = infantry.transform.position.x > 0f;

                bool isFacingLeft = infantry.IsFacingLeft();

                // Dacă e în dreapta, dar se uită spre stânga => întoarce
                if (isOnRightSide && isFacingLeft)
                {
                    infantry.ForceTurnAround();
                }
                
            }
        }
        foreach (var infantry in leftInfantry)
        {
            bool isIdle = !infantry.HasMoved;

            if (isIdle)
            {
                bool isOnLeftSide = infantry.transform.position.x < 0f;

                bool isFacingRight = infantry.IsFacingRight();

                
                // Dacă e în stânga, dar se uită spre dreapta => întoarce
                if (isOnLeftSide && isFacingRight)
                {
                    infantry.ForceTurnAround();
                }
            }
        }
    }


    private Vector3 RandomOffset() => new Vector3(Random.Range(-0.1f, 0.1f), 0f, 0f);
}
