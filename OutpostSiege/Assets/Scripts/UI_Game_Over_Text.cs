using UnityEngine;
using UnityEngine.UI;

public class UI_Game_Over_Text : MonoBehaviour
{
    [Header("Blinking Settings")]
    [SerializeField] private float blinkSpeed = 1.5f;

    [Header("Color Cycle Settings")]
    [SerializeField] private Color[] colors;
    [SerializeField] private float colorChangeSpeed = 1.0f;

    private Text gameOverText;
    private float blinkTimer;
    private float colorTimer;
    private int currentColorIndex;

    void Start()
    {
        gameOverText = GetComponent<Text>();
        if (gameOverText == null)
        {
            Debug.LogError("UI Text component not found!");
            enabled = false;
            return;
        }

        if (colors == null || colors.Length == 0)
        {
            // Default to red/white if no colors assigned
            colors = new Color[] { Color.red, Color.white };
        }

        gameOverText.color = colors[0];
    }

    void Update()
    {
        // Blink (alpha fade in/out)
        blinkTimer += Time.deltaTime * blinkSpeed;
        float alpha = Mathf.Abs(Mathf.Sin(blinkTimer));
        Color currentColor = gameOverText.color;
        currentColor.a = alpha;
        gameOverText.color = currentColor;

        // Change color over time
        colorTimer += Time.deltaTime;
        if (colorTimer >= colorChangeSpeed)
        {
            colorTimer = 0f;
            currentColorIndex = (currentColorIndex + 1) % colors.Length;
            Color newColor = colors[currentColorIndex];
            newColor.a = alpha; // keep alpha from blink
            gameOverText.color = newColor;
        }
    }
}
