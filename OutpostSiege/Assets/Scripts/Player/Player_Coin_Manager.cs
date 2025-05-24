using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestioneaza sistemul de monede al jucatorului: numar maxim, monede curente, adaugare, scadere si actualizare UI.
/// </summary>
public class Player_CoinManager : MonoBehaviour
{
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private int startingCoins = 20;
    [SerializeField] private Text coinText;
    private int currentCoins;

    public int CurrentCoins => currentCoins;

    /// <summary>
    /// Initializeaza valoarea monedelor la inceputul jocului.
    /// </summary>
    public void Initialize()
    {
        currentCoins = Mathf.Clamp(startingCoins, 0, maxCoins);
        UpdateCoinUI();
    }

    /// <summary>
    /// Incearca sa consume o moneda. Returneaza true daca s-a putut.
    /// </summary>
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

    /// <summary>
    /// Adauga o cantitate de monede.
    /// </summary>
    public void AddCoin(int amount)
    {
        currentCoins = Mathf.Min(currentCoins + amount, maxCoins);
        UpdateCoinUI();
    }

    /// <summary>
    /// Returneaza monede dupa un delay.
    /// </summary>
    public void ReturnCoinsWithDelay(int amount, float delay)
    {
        StartCoroutine(ReturnAfterDelay(amount, delay));
    }

    private IEnumerator ReturnAfterDelay(int amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        AddCoin(amount);
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = $"x{currentCoins}";
    }

    public bool HasEnoughCoins(int amount)
    {
        return currentCoins >= amount;
    }

    public void SpendCoins(int amount)
    {
        currentCoins = Mathf.Max(currentCoins - amount, 0);
        UpdateCoinUI();
    }



}
