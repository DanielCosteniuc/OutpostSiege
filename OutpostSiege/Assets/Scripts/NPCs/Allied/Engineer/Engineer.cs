using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class Engineer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float stopDistance = 0.2f;

    [Header("Tree Cutting")]
    [SerializeField] private float cutDuration = 2f; // configurable in Inspector

    private Vector3 basePosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    //queue pt trees
    private readonly Queue<(GameObject tree, Action<GameObject> callback)> taskQueue = new();

    //queue pt walls
    private readonly Queue<(MonoBehaviour wall, Action<GameObject> callback)> wallTaskQueue = new();

    //queue pt towers/outposts
    private readonly Queue<(MonoBehaviour outpost, Action<GameObject> callback)> outpostTaskQueue = new();


    private void OnEnable()
    {
        // Subscribe to Tree_Task_Manager's event to get notified when a new task is added
        if (Tree_Task_Manager.Instance != null)
            Tree_Task_Manager.Instance.OnNewTaskAdded += OnNewTaskAddedHandler;

        ////for walls
        if (Engineer_Wall_Manager.Instance != null)
            Engineer_Wall_Manager.Instance.OnNewWallTaskAdded += OnNewWallTaskAddedWall;

        //for outposts
        if (Engineer_Outpost_Manager.Instance != null)
            Engineer_Outpost_Manager.Instance.OnNewOutpostTaskAdded += OnNewOutpostTaskAdded;


    }

    private void OnDisable()
    {
        // Unsubscribe when disabled/destroyed to avoid memory leaks
        if (Tree_Task_Manager.Instance != null)
            Tree_Task_Manager.Instance.OnNewTaskAdded -= OnNewTaskAddedHandler;

        //for walls
        if (Engineer_Wall_Manager.Instance != null)
            Engineer_Wall_Manager.Instance.OnNewWallTaskAdded -= OnNewWallTaskAddedWall;

        //for outposts
        if (Engineer_Outpost_Manager.Instance != null)
            Engineer_Outpost_Manager.Instance.OnNewOutpostTaskAdded -= OnNewOutpostTaskAdded;


    }

    private void Start()
    {
        basePosition = transform.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        animator.SetBool("running", false);
        animator.SetBool("engineering", false);

        StartCoroutine(HandleQueue());

        // Removed initial manual global task check — event subscription covers that now
    }

    // Event handler called when a new tree task is added globally
    private void OnNewTaskAddedHandler()
    {
        if (Tree_Task_Manager.Instance.TryGetTask(out var task))
        {
            // Avoid duplicate tasks in local queue
            if (!IsTreeAlreadyQueued(task.Item1))
            {
                taskQueue.Enqueue(task);
                Debug.Log($"Engineer received new task via event: {task.Item1.name}");
            }
        }
    }

    public void RequestTreeCut(GameObject tree, Action<GameObject> onTreeCut)
    {
        if (tree == null) return;

        if (!IsTreeAlreadyQueued(tree))
        {
            taskQueue.Enqueue((tree, onTreeCut));
            Debug.Log($"{name} now has {taskQueue.Count} tasks");

            Tree_Interactions interaction = tree.GetComponent<Tree_Interactions>();
            if (interaction != null)
            {
                interaction.ForcePaidVisual();
            }
        }
    }

    private bool IsTreeAlreadyQueued(GameObject tree)
    {
        foreach (var item in taskQueue)
        {
            if (item.tree == tree) return true;
        }
        return false;
    }

    private IEnumerator HandleQueue()
    {
        while (true)
        {
            if (taskQueue.Count == 0 && wallTaskQueue.Count == 0 && outpostTaskQueue.Count == 0)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }


            // Procesare copaci
            while (taskQueue.Count > 0)
            {
                var (tree, callback) = taskQueue.Dequeue();

                if (tree == null) continue;

                yield return MoveTo(tree);
                animator.SetBool("engineering", true);
                yield return new WaitForSeconds(cutDuration);
                animator.SetBool("engineering", false);

                callback?.Invoke(tree);
                Destroy(tree);
            }

            // Procesare garduri
            // Procesare garduri
            while (wallTaskQueue.Count > 0)
            {
                var (wallScript, callback) = wallTaskQueue.Dequeue();

                if (wallScript == null) continue;

                yield return MoveTo(wallScript.gameObject);
                animator.SetBool("engineering", true);
                yield return new WaitForSeconds(3f); 
                animator.SetBool("engineering", false);

                callback?.Invoke(wallScript.gameObject);
            }

            // Procesare outposturi
            while (outpostTaskQueue.Count > 0)
            {
                var (outpostScript, callback) = outpostTaskQueue.Dequeue();

                if (outpostScript == null) continue;

                yield return MoveTo(outpostScript.gameObject);
                animator.SetBool("engineering", true);
                yield return new WaitForSeconds(4f); 
                animator.SetBool("engineering", false);

                callback?.Invoke(outpostScript.gameObject);
                Debug.Log("Engineer started building outpost.");

            }



            yield return MoveTo(basePosition);
            animator.SetBool("running", false);
        }
    }

    private IEnumerator MoveTo(Vector3 targetPos)
    {
        animator.SetBool("running", true);
        FlipToFace(targetPos.x);

        Vector3 targetFlat = new Vector3(targetPos.x, transform.position.y, transform.position.z);

        while (Vector3.Distance(transform.position, targetFlat) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetFlat, moveSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("running", false);
    }

    private IEnumerator MoveTo(GameObject targetObject)
    {
        Collider2D targetCollider = targetObject.GetComponent<Collider2D>();
        Collider2D myCollider = GetComponent<Collider2D>();

        if (targetCollider == null || myCollider == null)
        {
            Debug.LogWarning("Missing colliders on Engineer or Tree.");
            yield break;
        }

        animator.SetBool("running", true);
        FlipToFace(targetObject.transform.position.x);

        while (!myCollider.IsTouching(targetCollider))
        {
            Vector3 targetPos = new Vector3(targetObject.transform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("running", false);
    }

    private void FlipToFace(float targetX)
    {
        spriteRenderer.flipX = targetX < transform.position.x;
    }

    public bool IsBusy() => taskQueue.Count > 0;
    public int GetTaskCount()
    {
        return taskQueue.Count;
    }


    //for walls
    private void OnNewWallTaskAddedWall()
    {
        if (Engineer_Wall_Manager.Instance.TryGetWallTask(out var task))
        {
            if (!IsWallAlreadyQueued(task.Item1))
            {
                wallTaskQueue.Enqueue(task);
                Debug.Log($"{name} a primit un task de construit gard.");
            }
        }
    }

    private bool IsWallAlreadyQueued(MonoBehaviour wall)
    {
        foreach (var item in wallTaskQueue)
        {
            if (item.wall == wall) return true;
        }
        return false;
    }

    //for outposts
    private void OnNewOutpostTaskAdded()
    {
        if (Engineer_Outpost_Manager.Instance.TryGetOutpostTask(out var task))
        {
            if (!IsOutpostAlreadyQueued(task.Item1))
            {
                outpostTaskQueue.Enqueue(task);
                Debug.Log($"{name} a primit un task de construit avanpost.");
            }
        }
        else
        {
            Debug.LogWarning("No outpost task found in queue.");
        }

    }

    private bool IsOutpostAlreadyQueued(MonoBehaviour outpost)
    {
        foreach (var item in outpostTaskQueue)
        {
            if (item.outpost == outpost) return true;
        }
        return false;
    }
}
