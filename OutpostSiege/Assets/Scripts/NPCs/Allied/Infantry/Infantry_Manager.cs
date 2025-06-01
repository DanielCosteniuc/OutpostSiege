using System.Collections.Generic;
using UnityEngine;

public class Infantry_Manager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject infantryPrefab;
    [SerializeField] private Wall_Manager wallManager;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private Enemy_Spawn_Manager enemySpawnManager;


    [Header("Engineer Spawn Range")]
    [SerializeField] private float minOffsetX = -3f;
    [SerializeField] private float maxOffsetX = 3f;
    [SerializeField] private float offsetY = 3.2f;

    //private readonly List<Infantry> activeInfantry = new();
    private readonly List<Infantry> leftInfantry = new();
    private readonly List<Infantry> rightInfantry = new();

    [Header("Attack Positions")]
    [SerializeField] private Vector3 leftAttackPosition = new Vector3(-250f, -3.2f, 0f);
    [SerializeField] private Vector3 rightAttackPosition = new Vector3(250f, -3.2f, 0f);

    private Vector3 lastLeftAttackPosition;
    private Vector3 lastRightAttackPosition;



    public static Infantry_Manager Instance { get; private set; }
    private void Start()
    {
        if (spawnPoint != null)
        {
            lastLeftAttackPosition = new Vector3(spawnPoint.transform.position.x, -3.2f, spawnPoint.transform.position.z);
            lastRightAttackPosition = new Vector3(spawnPoint.transform.position.x, -3.2f, spawnPoint.transform.position.z);

        }
        else
        {
            Debug.LogWarning("[Infantry_Manager] spawnPoint not set! Defaulting last attack positions to zero.");
            lastLeftAttackPosition = new Vector3(0,0,0);
            lastRightAttackPosition = new Vector3(0, 0, 0);
        }
    }


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
        HandleAttackMovement();

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

        // ✅ Modificat: direcționare în dreapta dacă baza stângă e distrusă
        if (enemySpawnManager != null && enemySpawnManager.IsLeftBaseDestroyed || leftInfantry.Count >= rightInfantry.Count)
        {
            rightInfantry.Add(infantry);
            infantry.SetTargetDirection(Infantry.Direction.Right);
        }
        else
        {
            leftInfantry.Add(infantry);
            infantry.SetTargetDirection(Infantry.Direction.Left);
        }

        // Mutăm infanteria spre cel mai nou gard din direcția respectivă
        GameObject wall = infantry.TargetDirection == Infantry.Direction.Left
            ? wallManager.GetLastLeftWall()
            : wallManager.GetLastRightWall();

        if (wall != null)
        {
            Vector3 targetPos = wallManager.GetAdjustedWallPosition(wall) + RandomOffset();
            infantry.MoveTo(targetPos);

            // ✅ Memorăm ultima poziție de atac
            if (infantry.TargetDirection == Infantry.Direction.Left)
                lastLeftAttackPosition = targetPos;
            else
                lastRightAttackPosition = targetPos;
        }
    }

    public void OnInfantryDeath(Infantry infantry)
    {
        if (infantry == null) return;

        //activeInfantry.Remove(infantry);
        leftInfantry.Remove(infantry);
        rightInfantry.Remove(infantry);
    }

    // Apelata cand se construieste un gard nou
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

                // ✅ Actualizează ultima poziție de atac stânga
                lastLeftAttackPosition = finalTarget;
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

                // ✅ Actualizează ultima poziție de atac dreapta
                lastRightAttackPosition = finalTarget;
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

                // Daca e in dreapta, dar se uita spre stanga => intoarce
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


                // Daca e in stanga, dar se uita spre dreapta => intoarce
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
            if (infantry.HasMoved && !infantry.IsAtTarget()) // Se afla in miscare
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

    private void HandleAttackMovement()
    {
        if (Player_Menu_Manager.Instance == null || enemySpawnManager == null) return;

        // === ATAC STÂNGA ===
        if (Player_Menu_Manager.Instance.AttackLeft)
        {
            if (enemySpawnManager.IsLeftBaseDestroyed)
            {
                Debug.Log("[Infantry_Manager] Left base destroyed! Redirecting LEFT infantry to RIGHT side.");
                foreach (var infantry in leftInfantry)
                {
                    if (!infantry.IsAtTarget())
                    {
                        infantry.MoveTo(lastRightAttackPosition);
                        infantry.SetTargetDirection(Infantry.Direction.Right);
                        rightInfantry.Add(infantry);
                    }
                }
                leftInfantry.Clear();
            }
            else
            {
                foreach (var infantry in leftInfantry)
                {
                    if (!infantry.IsAtTarget())
                    {
                        infantry.MoveTo(leftAttackPosition);
                        lastLeftAttackPosition = leftAttackPosition;
                    }
                }
            }
        }

        // === ATAC DREAPTA ===
        if (Player_Menu_Manager.Instance.AttackRight)
        {
            if (enemySpawnManager.IsRightBaseDestroyed)
            {
                Debug.Log("[Infantry_Manager] Right base destroyed! Redirecting RIGHT infantry to LEFT side.");
                foreach (var infantry in rightInfantry)
                {
                    if (!infantry.IsAtTarget())
                    {
                        infantry.MoveTo(lastLeftAttackPosition);
                        infantry.SetTargetDirection(Infantry.Direction.Left);
                        leftInfantry.Add(infantry);
                    }
                }
                rightInfantry.Clear();
            }
            else
            {
                foreach (var infantry in rightInfantry)
                {
                    if (!infantry.IsAtTarget())
                    {
                        infantry.MoveTo(rightAttackPosition);
                        lastRightAttackPosition = rightAttackPosition;
                    }
                }
            }
        }
    }



}
