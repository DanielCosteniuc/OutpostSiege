using UnityEngine;

public class Enemy_Spawn_Manager : MonoBehaviour
{
    [SerializeField] public GameObject leftBase;
    [SerializeField] public GameObject rightBase;
    [SerializeField] public You_Won_Controller wellDone;

    private bool leftDestroyed = false;
    private bool rightDestroyed = false;

    [HideInInspector] public bool isEnemyDefeated = false;

    // ✅ Proprietăți publice pentru a verifica starea bazelor
    public bool IsLeftBaseDestroyed => leftDestroyed;
    public bool IsRightBaseDestroyed => rightDestroyed;

    public void NotifyBaseDestroyed(GameObject baseObject)
    {
        if (baseObject == leftBase)
        {
            leftDestroyed = true;
            Debug.Log("Left base destroyed!");
        }
        else if (baseObject == rightBase)
        {
            rightDestroyed = true;
            Debug.Log("Right base destroyed!");
        }

        if (leftDestroyed && rightDestroyed && !isEnemyDefeated)
        {
            isEnemyDefeated = true;
            wellDone.TriggerYouWon();
            Debug.Log("All enemy bases destroyed! Enemy defeated!");
        }
    }
}
