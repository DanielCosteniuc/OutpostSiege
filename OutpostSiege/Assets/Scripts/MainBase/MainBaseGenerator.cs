using UnityEngine;

public class MainBaseGenerator : MonoBehaviour
{
    [Header("Main base")]
    [SerializeField] private GameObject prefabLevel1;  // Prefab Nivel 1
    [SerializeField] private GameObject prefabLevel2;  // Prefab Nivel 2

    [Header("Coordonate X și Y pentru plasarea obiectului")]
    [SerializeField] private float xPosition = -0.22f;
    [SerializeField] private float yPosition = -3f;

    private GameObject currentPrefab;

    void Start()
    {
        PlacePrefabLevel1();  // Plasează nivelul 1 la început
    }

    private void PlacePrefabLevel1()
    {
        if (prefabLevel1 != null)
        {
            Vector3 spawnPosition = new Vector3(xPosition, yPosition, 0f);
            currentPrefab = Instantiate(prefabLevel1, spawnPosition, Quaternion.identity, transform);

            // Verificăm dacă obiectul instanțiat este valid
            if (currentPrefab != null)
            {
                // Adaugă scriptul de upgrade direct pe obiectul instanțiat (base_0)
                UpgradeMainBaseGenerator upgradeScript = currentPrefab.AddComponent<UpgradeMainBaseGenerator>();

                // Asigură-te că obiectele sunt setate corect
                upgradeScript.player = GameObject.FindWithTag("Player");
                upgradeScript.pressSpaceUI = GameObject.Find("Canvas/PressSpaceUI");  // Specifică calea completă dacă este un sub-obiect


                if (upgradeScript.player == null)
                {
                    Debug.LogError("Player not found with the tag 'Player' in the scene.");
                }

                if (upgradeScript.pressSpaceUI == null)
                {
                    Debug.LogError("PressSpaceUI object not found in the scene.");
                }

                upgradeScript.mainBase = this; // Se setează referința la baza curentă
            }
            else
            {
                Debug.LogError("Failed to instantiate prefabLevel1.");
            }
        }
        else
        {
            Debug.LogError("Prefab Level 1 is not set in the Inspector.");
        }
    }

    public void UpgradeBase()
    {
        if (currentPrefab != null)
        {
            Destroy(currentPrefab);  // Distruge prefab-ul actual (Nivelul 1)
        }

        if (prefabLevel2 != null)
        {
            Vector3 spawnPosition = new Vector3(xPosition, yPosition, 0f);
            currentPrefab = Instantiate(prefabLevel2, spawnPosition, Quaternion.identity, transform);
        }

        Debug.Log("Base Upgraded to Level 2!");
    }
}
