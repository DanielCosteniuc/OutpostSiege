using System.Collections;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int health = 100;
    [HideInInspector] public bool isPlayerDead { get; private set; }

    private SpriteRenderer sr;
    private Color originalColor;
    private Collider2D col;

    [Header("Visual Effects")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color hitColor = Color.white;

    [Header("References")]
    [SerializeField] private Game_Over_Controller gameOver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPlayerDead = false;

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(FlashWhite());

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {  
        isPlayerDead = true;

        if (col != null) col.enabled = false;
        StartCoroutine(FadeAndDestroy());

        gameOver.TriggerGameOver();
    }

    private IEnumerator FadeAndDestroy()
    {
        float timer = 0f;
        Color startColor = sr.color;

        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            sr.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, t));
            timer += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        Destroy(gameObject);
    }

    private IEnumerator FlashWhite()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }
}
