using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Gestioneaza lista de ingineri, alocarea taskurilor si verificarea disponibilitatii.
/// </summary>
public class Player_EngineerManager : MonoBehaviour
{
    private List<Engineer> engineers = new();

    /// <summary>
    /// Adauga un inginer la lista.
    /// </summary>
    public void AddEngineer(Engineer engineer)
    {
        if (engineer != null && !engineers.Contains(engineer))
            engineers.Add(engineer);
    }

    /// <summary>
    /// Aloca un copac catre un inginer disponibil sau cel mai putin ocupat.
    /// </summary>
    public void AssignEngineer(GameObject tree, System.Action<GameObject> onCut, System.Action onFail)
    {
        var available = engineers.FirstOrDefault(e => !e.IsBusy());

        if (available != null)
        {
            available.RequestTreeCut(tree, onCut);
        }
        else if (engineers.Count > 0)
        {
            var leastBusy = engineers.OrderBy(e => e.GetTaskCount()).First();
            leastBusy.RequestTreeCut(tree, onCut);
        }
        else
        {
            onFail?.Invoke();
        }
    }
}
