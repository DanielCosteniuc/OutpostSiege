using System;
using System.Collections.Generic;
using UnityEngine;

public class Engineer_Outpost_Manager : MonoBehaviour
{
    public static Engineer_Outpost_Manager Instance { get; private set; }

    private readonly Queue<(MonoBehaviour outpostScript, Action<GameObject> callback)> outpostTasks = new();

    public event Action OnNewOutpostTaskAdded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddOutpostTask(MonoBehaviour outpost, Action<GameObject> callback)
    {
        Debug.Log("Outpost task added to queue.");
        outpostTasks.Enqueue((outpost, callback));
        OnNewOutpostTaskAdded?.Invoke();
    }

    public bool TryGetOutpostTask(out (MonoBehaviour, Action<GameObject>) task)
    {
        if (outpostTasks.Count > 0)
        {
            task = outpostTasks.Dequeue();
            return true;
        }

        task = default;
        return false;
    }
}
