using UnityEngine;

public class CoinHolder_Color : MonoBehaviour
{
    // Reference to the SpriteRenderer component
    private SpriteRenderer spriteRenderer;

    // Color to set the coin to (can be changed in the Inspector)
    public Color newColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        // Get the SpriteRenderer component attached to the same GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If a SpriteRenderer is found, change its color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = newColor;
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found on the GameObject!");
        }
    }

}
