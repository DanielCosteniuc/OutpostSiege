using UnityEngine;

public class Tree : MonoBehaviour
{
    public bool isSelected = false;

    public void CutDown()
    {
        // Aici poți adăuga efecte, sunete, particule etc. înainte de distrugere
        Destroy(gameObject);
    }
}
