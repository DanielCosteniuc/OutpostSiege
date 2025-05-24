using UnityEngine;

public class Cut_Tree : MonoBehaviour
{
    public bool isSelected = false;
    public void CutDown()
    {
        Destroy(gameObject);
    }
}
