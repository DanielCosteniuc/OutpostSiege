using UnityEngine;

public class UpgradeMainBaseGenerator : MonoBehaviour
{
    [Header("Referințe")]
    public GameObject player;  // Referința la player
    public GameObject pressSpaceUI; // UI care afiseaza "Press Space"
    public MainBaseGenerator mainBase; // Referință la ProceduralMainBase

    private bool isNearPrefab = false;

    void Start()
    {
        Debug.Log("Start method called.");

        // Căutăm obiectul PressSpaceUI doar la runtime
        if (pressSpaceUI == null)
        {
            pressSpaceUI = GameObject.Find("PressSpaceUI");
            if (pressSpaceUI == null)
            {
                Debug.LogError("PressSpaceUI object not found in the scene.");
            }
            else
            {
                Debug.Log("PressSpaceUI found in the scene at runtime.");
            }
        }
        if (pressSpaceUI != null)
        {
            pressSpaceUI.SetActive(true);  // Arată UI-ul
        }
        else
        {
            Debug.LogError("PressSpaceUI is null!");
        }


        // Alte verificări
        Debug.Log("Player and MainBase references checked.");

        
        Debug.Log("Press Space UI initialized");
    }




    void Update()
    {
        if (isNearPrefab && Input.GetKeyDown(KeyCode.Space))  // Dacă playerul este aproape și apasă Space
        {
            // Apelăm funcția de upgrade a bazei principale
            Debug.Log("Space key pressed, upgrading main base");
            UpgradeMainBase();
        }
    }

    private void UpgradeMainBase()
    {
        if (mainBase != null)
        {
            mainBase.UpgradeBase();  // Apelăm funcția de upgrade din ProceduralMainBase
            Debug.Log("Base upgraded");
        }
        else
        {
            Debug.LogError("MainBase reference is missing!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPrefab = true;
            Debug.Log("Player entered trigger zone.");

            if (pressSpaceUI != null)
            {
                pressSpaceUI.SetActive(true);  // Arată mesajul "Press Space"
                Debug.Log("PressSpaceUI activated.");
            }
            else
            {
                Debug.LogError("PressSpaceUI is null!");
            }

            Debug.Log("Player is near the prefab!");
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPrefab = false;
            if (pressSpaceUI != null)
            {
                pressSpaceUI.SetActive(false);  // Ascunde mesajul "Press Space"
            }
            else
            {
                Debug.LogError("PressSpaceUI is null!");
            }
            Debug.Log("Player left the prefab area!");
        }
    }
}
