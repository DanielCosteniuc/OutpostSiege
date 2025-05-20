using System;
using System.Collections.Generic;
using UnityEngine;

public class Engineer_Wall_Manager : MonoBehaviour
{
    public static Engineer_Wall_Manager Instance { get; private set; }

    private readonly Queue<(MonoBehaviour wallScript, Action<GameObject> callback)> wallTasks = new();

    public event Action OnNewWallTaskAdded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddWallTask(MonoBehaviour wall, Action<GameObject> callback)
    {
        wallTasks.Enqueue((wall, callback));
        OnNewWallTaskAdded?.Invoke();
    }

    public bool TryGetWallTask(out (MonoBehaviour, Action<GameObject>) task)
    {
        if (wallTasks.Count > 0)
        {
            task = wallTasks.Dequeue();
            return true;
        }

        task = default;
        return false;
    }
}
