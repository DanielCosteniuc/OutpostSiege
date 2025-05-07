using UnityEngine;

public class Main_Base_Generator : MonoBehaviour
{
    [Header("Base Configuration")]
    [SerializeField] private GameObject[] baseLevels;
    [SerializeField] private float yPosition = -3f;

    private GameObject currentBase;
    private int currentLevel = 0;
    private bool canUpgrade = false;

    void Start()
    {
        SpawnBaseLevel(currentLevel);
    }

    void Update()
    {
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
            Debug.Log("✅ Baza a fost upgradată la nivelul " + currentLevel);
        }
        else
        {
            Debug.Log("⚠️ Baza este deja la nivelul maxim.");
        }
    }

    public void SetCanUpgrade(bool value)
    {
        canUpgrade = value;
    }
}
