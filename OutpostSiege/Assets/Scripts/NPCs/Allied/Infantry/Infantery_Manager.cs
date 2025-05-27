using System.Collections.Generic;
using UnityEngine;

public class Infantry_Manager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject infantryPrefab;
    [SerializeField] private Wall_Manager wallManager;
    [SerializeField] private GameObject spawnPoint;

    [Header("Engineer Spawn Range")]
    [SerializeField] private float minOffsetX = -3f;
    [SerializeField] private float maxOffsetX = 3f;
    [SerializeField] private float offsetY = 3.2f;

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

        Vector3 spawnPos = spawnPoint.transform.position + new Vector3(Random.Range(minOffsetX, maxOffsetX), offsetY, 0f);
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


        Debug.Log($"[Infantry_Manager] Reassigning infantry: LeftWall={(leftWall != null ? leftWall.name : "null")}, RightWall={(rightWall != null ? rightWall.name : "null")}");

        foreach (Infantry infantry in leftInfantry)
        {
            if (!infantry.HasMoved && leftWall != null)
            {
                Vector3 baseTarget = wallManager.GetAdjustedWallPosition(leftWall);
                float spawnX = spawnPoint.transform.position.x;
                float currentX = infantry.transform.position.x;
                float targetX = baseTarget.x;

                // Se apropie de spawn? (merge spre centru)
                if (Mathf.Abs(targetX - spawnX) < Mathf.Abs(currentX - spawnX))
                {
                    targetX += 2f;
                }

                Vector3 finalTarget = new Vector3(targetX, baseTarget.y, baseTarget.z);
                infantry.MoveTo(finalTarget);
            }
            
        }

        foreach (Infantry infantry in rightInfantry)
        {
            if (!infantry.HasMoved && rightWall != null)
            {
                Vector3 baseTarget = wallManager.GetAdjustedWallPosition(rightWall);
                float spawnX = spawnPoint.transform.position.x;
                float currentX = infantry.transform.position.x;
                float targetX = baseTarget.x;

                if (Mathf.Abs(targetX - spawnX) < Mathf.Abs(currentX - spawnX))
                {
                    targetX -= 2f;
                }

                Vector3 finalTarget = new Vector3(targetX, baseTarget.y, baseTarget.z);
                infantry.MoveTo(finalTarget);
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

    public void ReassignMovingInfantryToWalls()
    {
        GameObject leftWall = wallManager.GetLastLeftWall();
        GameObject rightWall = wallManager.GetLastRightWall();

        foreach (Infantry infantry in leftInfantry)
        {
            if (infantry.HasMoved && !infantry.IsAtTarget()) // Se află în mișcare
            {
                Vector3 baseTarget = wallManager.GetAdjustedWallPosition(leftWall);
                float spawnX = spawnPoint.transform.position.x;
                float currentX = infantry.transform.position.x;
                float targetX = baseTarget.x;

                if (Mathf.Abs(targetX - spawnX) < Mathf.Abs(currentX - spawnX))
                    targetX += 2f;

                Vector3 finalTarget = new Vector3(targetX, baseTarget.y, baseTarget.z);
                infantry.MoveTo(finalTarget);
            }
        }

        foreach (Infantry infantry in rightInfantry)
        {
            if (infantry.HasMoved && !infantry.IsAtTarget())
            {
                Vector3 baseTarget = wallManager.GetAdjustedWallPosition(rightWall);
                float spawnX = spawnPoint.transform.position.x;
                float currentX = infantry.transform.position.x;
                float targetX = baseTarget.x;

                if (Mathf.Abs(targetX - spawnX) < Mathf.Abs(currentX - spawnX))
                    targetX -= 2f;

                Vector3 finalTarget = new Vector3(targetX, baseTarget.y, baseTarget.z);
                infantry.MoveTo(finalTarget);
            }
        }
    }

    private Vector3 RandomOffset() => new Vector3(Random.Range(-0.1f, 0.1f), 0f, 0f);
}
