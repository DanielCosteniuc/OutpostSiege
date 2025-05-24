using UnityEngine;

public class Main_Base_Generator : MonoBehaviour
{
    [Header("Base Configuration")]
    [SerializeField] private GameObject[] baseLevels; // All base level prefabs (Level 0, Level 1, etc.)
    [SerializeField] private float yPosition = -3f;    // Y position of the base

    private GameObject currentBase;
    private int currentLevel = 0;
    private bool canUpgrade = false; // Controlled externally (e.g., via trigger)

    void Start()
    {
        SpawnBaseLevel(currentLevel); // Spawn the first base level
    }

    void Update()
    {
        // Player presses space and upgrade is allowed
        if (canUpgrade && Input.GetKeyDown(KeyCode.Space))
        {
            UpgradeBase();
        }
    }

    private void SpawnBaseLevel(int level)
    {
        if (level >= 0 && level < baseLevels.Length)
        {
            if (currentBase != null)
            {
                Destroy(currentBase);
            }

            Vector3 spawnPosition = new Vector3(0f, yPosition, 0f);
            currentBase = Instantiate(baseLevels[level], spawnPosition, Quaternion.identity, transform);
        }
        else
        {
            Debug.LogWarning("Invalid level index or no more base upgrades.");
        }
    }

    public void UpgradeBase()
    {
        if (currentLevel + 1 < baseLevels.Length)
        {
            currentLevel++;
            SpawnBaseLevel(currentLevel);
            Debug.Log("Base upgraded to level " + currentLevel);
        }
        else
        {
            Debug.Log("Base is already at the maximum level.");
        }
    }

    public void SetCanUpgrade(bool value)
    {
        canUpgrade = value;
    }
}
