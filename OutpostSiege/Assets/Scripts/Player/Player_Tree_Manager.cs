using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestioneaza selectarea copacilor, alocarea inginerilor si anularea selectiei daca nu sunt resurse disponibile.
/// </summary>
public class Player_TreeManager : MonoBehaviour
{
    [SerializeField] private int maxTrees = 10;
    private List<GameObject> selectedTrees = new();
    private HashSet<GameObject> overlappingTrees = new();

    [SerializeField] private Player_CoinManager coinManager;
    [SerializeField] private Player_EngineerManager engineerManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TrySelectOverlappedTree();
        }
    }

    /// <summary>
    /// Incearca sa selecteze un copac suprapus.
    /// </summary>
    private void TrySelectOverlappedTree()
    {
        foreach (var treeObj in overlappingTrees)
        {
            if (treeObj == null) continue;

            Cut_Tree tree = treeObj.GetComponent<Cut_Tree>();
            if (tree != null && !tree.isSelected)
            {
                TrySelectTree(treeObj);
                return;
            }
        }

        Debug.Log("Niciun copac valid suprapus.");
    }

    /// <summary>
    /// Incearca sa selecteze un copac si sa consume o moneda.
    /// </summary>
    private void TrySelectTree(GameObject treeObj)
    {
        if (selectedTrees.Count >= maxTrees || !coinManager.TrySpendCoin()) return;

        var tree = treeObj.GetComponent<Cut_Tree>();
        if (tree == null || tree.isSelected) return;

        tree.isSelected = true;
        selectedTrees.Add(treeObj);

        treeObj.GetComponent<Tree_Interactions>()?.ChangeCoinVisual();

        StartCoroutine(AssignTreeAfterDelay(treeObj, 0.5f));
    }

    private IEnumerator AssignTreeAfterDelay(GameObject treeObj, float delay)
    {
        yield return new WaitForSeconds(delay);

        engineerManager.AssignEngineer(treeObj, OnTreeCut, () => CancelTree(treeObj));
    }

    /// <summary>
    /// Anuleaza selectia unui copac si returneaza moneda.
    /// </summary>
    private void CancelTree(GameObject treeObj)
    {
        treeObj.GetComponent<Cut_Tree>().isSelected = false;
        selectedTrees.Remove(treeObj);

        var interaction = treeObj.GetComponent<Tree_Interactions>();
        interaction?.RemoveCoinVisual();
        interaction?.ResetPayment();

        coinManager.AddCoin(1);
        Debug.Log("Niciun inginer disponibil. Moneda returnata.");
    }

    /// <summary>
    /// Apeleaza cand copacul a fost taiat. Se adauga inapoi monede.
    /// </summary>
    private void OnTreeCut(GameObject tree)
    {
        if (selectedTrees.Contains(tree))
        {
            selectedTrees.Remove(tree);
            int reward = Random.Range(1, 3);
            coinManager.AddCoin(reward);
            Debug.Log($"Copac taiat. +{reward} monede.");
        }
    }

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
}
