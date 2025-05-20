using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestioneaza trupele de infanterie ale jucatorului.
/// </summary>
public class Player_InfantryManager : MonoBehaviour
{
    private List<Infantry> infantryList = new();

    /// <summary>
    /// Adauga o trupa noua de infanterie.
    /// </summary>
    public void AddInfantry(Infantry newInfantry)
    {
        if (newInfantry != null && !infantryList.Contains(newInfantry))
            infantryList.Add(newInfantry);
    }

    // Poti adauga aici metode pentru comenzi, upgrade, plasare etc.
}
