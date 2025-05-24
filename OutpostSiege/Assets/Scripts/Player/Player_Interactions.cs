using UnityEngine;

/// <summary>
/// Clasa principala care conecteaza toate sistemele jucatorului: monede, copaci, ingineri, infanterie.
/// </summary>
public class Player_Interactions : MonoBehaviour
{
    [SerializeField] private Player_CoinManager coinManager;
    [SerializeField] private Player_TreeManager treeManager;
    [SerializeField] private Player_EngineerManager engineerManager;
    [SerializeField] private Player_InfantryManager infantryManager;

    private void Start()
    {
        coinManager.Initialize();
    }

    public void AddEngineer(Engineer engineer)
    {
        engineerManager.AddEngineer(engineer);
    }

    public void AddInfantry(Infantry infantry)
    {
        infantryManager.AddInfantry(infantry);
    }

    /// <summary>
    /// Apeleaza sistemul de monede pentru a incerca sa consume o moneda.
    /// </summary>
    public bool TrySpendCoin()
    {
        return coinManager.TrySpendCoin();
    }

    /// <summary>
    /// Apeleaza sistemul de monede pentru a returna monede jucatorului.
    /// </summary>
    public void ReturnCoinsToPlayer(int amount)
    {
        coinManager.AddCoin(amount);
        // Sau coinManager.ReturnCoinsWithDelay(amount, delay);
    }
}
