using System.Collections.Generic;
using System;
using UnityEngine;

public class Tree_Task_Manager : MonoBehaviour
{
    public static Tree_Task_Manager Instance;

    private Queue<(GameObject, Action<GameObject>)> queuedTrees = new Queue<(GameObject, Action<GameObject>)>();

    // Event to notify subscribers (engineers) that a new task is available
    public event Action OnNewTaskAdded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddTask(GameObject tree, Action<GameObject> callback)
    {
        queuedTrees.Enqueue((tree, callback));
        Debug.Log($"Added tree task: {tree.name}");

        // Notify subscribers that a new task has been added
        OnNewTaskAdded?.Invoke();
    }

    public bool TryGetTask(out (GameObject, Action<GameObject>) task)
    {
        if (queuedTrees.Count > 0)
        {
            task = queuedTrees.Dequeue();
            return true;
        }
        task = default;
        return false;
    }
}
