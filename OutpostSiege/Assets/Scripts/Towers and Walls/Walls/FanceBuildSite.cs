using System.Collections;
using UnityEngine;

public class FenceBuildSite : MonoBehaviour
{
    public int requiredCoins = 5;
    private int currentCoins = 0;
    private bool isPaid = false;
    private bool isBuilt = false;

    private Engineer assignedEngineer;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.red; // red until paid
    }

    public bool NeedsPayment()
    {
        return !isPaid;
    }

    public bool AddCoin()
    {
        if (isPaid) return false;

        currentCoins++;
        if (currentCoins >= requiredCoins)
        {
            isPaid = true;
            sr.color = Color.yellow; // waiting for engineer
            return true;
        }

        return false;
    }

    public void AssignEngineer(Engineer engineer)
    {
        if (!isPaid || isBuilt || engineer == null) return;

        assignedEngineer = engineer;
        assignedEngineer.RequestFenceBuild(gameObject, OnFenceBuilt);
    }

    private void OnFenceBuilt(GameObject fence)
    {
        isBuilt = true;
        sr.color = Color.green;
        Debug.Log("Fence built!");
    }
}
