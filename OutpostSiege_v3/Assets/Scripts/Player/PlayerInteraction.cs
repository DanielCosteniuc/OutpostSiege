using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("💰 Monede")]
    [SerializeField] private Text coinText;
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private int startingCoins = 5;
    private int currentCoins;

    [Header("👷 Ingineri")]
    [SerializeField] private float interactionRange = 0.2f;
    private List<Engineer> engineers = new List<Engineer>();

    [Header("💂 Infanterie")]
    private List<Infantry> infantryList = new List<Infantry>();


    private List<GameObject> selectedTrees = new List<GameObject>();
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
            TrySelectTree();
        }
    }

    private void TrySelectTree()
    {
        if (selectedTrees.Count >= maxTrees || currentCoins <= 0)
        {
            Debug.Log(currentCoins <= 0 ? "❌ Nu ai suficiente monede." : "📦 Limită copaci atinsă.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);

        foreach (var hit in hits)
        {
            Tree tree = hit.GetComponent<Tree>();
            if (tree != null && !tree.isSelected)
            {
                tree.isSelected = true;
                selectedTrees.Add(tree.gameObject);
                currentCoins--;
                UpdateCoinUI();

                Tree_Interactions interaction = tree.GetComponent<Tree_Interactions>();
                if (interaction != null)
                    interaction.ChangeCoinVisual();

                Engineer eng = GetAvailableEngineer();
                if (eng != null)
                {
                    eng.CutTree(tree.gameObject, OnTreeCut);
                    Debug.Log($"🌳 Trimite copacul la {eng.name}");
                }
                else if (engineers.Count > 0)
                {
                    engineers[0].CutTree(tree.gameObject, OnTreeCut);
                    Debug.Log("⌛ Fără ingineri liberi, pus în așteptare.");
                }
                else
                {
                    Debug.Log("❌ Niciun inginer disponibil.");
                }

                break;
            }
        }
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

            int coinsEarned = Random.Range(1, 3);
            currentCoins = Mathf.Min(currentCoins + coinsEarned, maxCoins);
            UpdateCoinUI();

            Debug.Log($"✅ Copac tăiat. +{coinsEarned} monede. Total: {currentCoins}");
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = $"x{currentCoins}";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
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
        {
            AddCoinBack();
        }
    }

    public void AddCoinBack()
    {
        currentCoins = Mathf.Min(currentCoins + 1, maxCoins);
        UpdateCoinUI();
    }

    public void AddEngineer(Engineer newEngineer)
    {
        if (newEngineer != null && !engineers.Contains(newEngineer))
            engineers.Add(newEngineer);
    }

    public void AddInfantry(Infantry newInfantry)
    {
        if (newInfantry != null && !infantryList.Contains(newInfantry))
            infantryList.Add(newInfantry);
    }

}
