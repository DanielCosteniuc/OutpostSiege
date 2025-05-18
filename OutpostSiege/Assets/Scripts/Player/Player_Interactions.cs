using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Interactions : MonoBehaviour
{
    // Coin System 
    [Header("Coin Settings")]
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private int startingCoins = 20;
    [SerializeField] private Text coinText;
    private int currentCoins;

    // Engineer Management 
    [Header("Engineers")]
    private List<Engineer> engineers = new List<Engineer>();

    // Tree Interaction 
    private List<GameObject> selectedTrees = new List<GameObject>();

    // Infantry Management
    private List<Infantry> infantryList = new List<Infantry>();

    private int maxTrees = 10;

    private void Start()
    {
        currentCoins = Mathf.Clamp(startingCoins, 0, maxCoins);
        UpdateCoinUI();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TrySelectOverlappedTree();
        }
    }

    private void TrySelectOverlappedTree()
    {
        foreach (var treeObj in overlappingTrees)
        {
            if (treeObj == null) continue;

            Cut_Tree tree = treeObj.GetComponent<Cut_Tree>();
            if (tree != null && !tree.isSelected)
            {
                TrySelectTree(treeObj);
                return; // Select only one at a time
            }
        }

        Debug.Log("No valid tree overlapped.");
    }

    // Trigger-based tree selection 
    private HashSet<GameObject> overlappingTrees = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            overlappingTrees.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            overlappingTrees.Remove(other.gameObject);
        }
    }

    private void TrySelectTree(GameObject treeObj)
    {
        if (selectedTrees.Count >= maxTrees || currentCoins <= 0)
        {
            Debug.Log(currentCoins <= 0 ? "Not enough coins." : "Tree selection limit reached.");
            return;
        }

        Cut_Tree tree = treeObj.GetComponent<Cut_Tree>();
        if (tree != null && !tree.isSelected)
        {
            // Ia banul imediat
            currentCoins--;
            UpdateCoinUI();
            tree.isSelected = true;
            selectedTrees.Add(treeObj);

            Tree_Interactions interaction = treeObj.GetComponent<Tree_Interactions>();
            if (interaction != null)
                interaction.ChangeCoinVisual();

            // Verifică după 0.5 secunde dacă există ingineri
            StartCoroutine(CheckEngineersAfterDelay(treeObj, 0.5f));
        }
    }

    private IEnumerator CheckEngineersAfterDelay(GameObject treeObj, float delay)
    {
        yield return new WaitForSeconds(delay);

        Engineer availableEngineer = GetAvailableEngineer();
        if (availableEngineer != null)
        {
            availableEngineer.RequestTreeCut(treeObj, OnTreeCut);
            Debug.Log($"Tree sent to {availableEngineer.name}");
        }
        else if (engineers.Count > 0)
        {
            // Găsește inginerul cu cele mai puține taskuri
            Engineer leastBusyEngineer = GetLeastBusyEngineer();
            leastBusyEngineer.RequestTreeCut(treeObj, OnTreeCut);
            Debug.Log($"All engineers busy, sent to {leastBusyEngineer.name}");
        }
        else
        {
            CancelTreeSelection(treeObj);
        }
    }

    private Engineer GetLeastBusyEngineer()
    {
        Engineer leastBusy = null;
        int minTasks = int.MaxValue;

        foreach (Engineer eng in engineers)
        {
            int taskCount = eng.GetTaskCount();
            if (taskCount < minTasks)
            {
                minTasks = taskCount;
                leastBusy = eng;
            }
        }
        return leastBusy;
    }

    private void CancelTreeSelection(GameObject treeObj)
    {
        treeObj.GetComponent<Cut_Tree>().isSelected = false;
        selectedTrees.Remove(treeObj);

        Tree_Interactions interaction = treeObj.GetComponent<Tree_Interactions>();
        if (interaction != null)
        {
            interaction.RemoveCoinVisual();
            interaction.ResetPayment();
        }

        AddCoinBack();
        Debug.Log("No engineers available. Coin returned.");
    }

    private Engineer GetAvailableEngineer()
    {
        foreach (var eng in engineers)
            if (!eng.IsBusy())
                return eng;
        return null;
    }

    private void OnTreeCut(GameObject tree)
    {
        if (selectedTrees.Contains(tree))
        {
            selectedTrees.Remove(tree);

            int coinsEarned = Random.Range(1, 3); // 1–2 coins
            currentCoins = Mathf.Min(currentCoins + coinsEarned, maxCoins);
            UpdateCoinUI();

            Debug.Log($"Tree cut. +{coinsEarned} coins. Total: {currentCoins}");
        }
    }

    // Coin Utilities
    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = $"x{currentCoins}";
    }

    public bool TrySpendCoin()
    {
        if (currentCoins > 0)
        {
            currentCoins--;
            UpdateCoinUI();
            return true;
        }
        return false;
    }

    public void ReturnCoinsToPlayer(int amount)
    {
        StartCoroutine(ReturnCoinsAfterDelay(1f, amount));
    }

    private IEnumerator ReturnCoinsAfterDelay(float delay, int amount)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < amount; i++)
            AddCoinBack();
    }

    public void AddCoinBack()
    {
        currentCoins = Mathf.Min(currentCoins + 1, maxCoins);
        UpdateCoinUI();
    }

    // External Additions
    public void AddEngineer(Engineer newEngineer)
    {
        if (newEngineer != null && !engineers.Contains(newEngineer))
            engineers.Add(newEngineer);
    }

    //Add infantry
    public void AddInfantry(Infantry newInfantry)
    {
        if (newInfantry != null && !infantryList.Contains(newInfantry))
            infantryList.Add(newInfantry);
    }

    

}
