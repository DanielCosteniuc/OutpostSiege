using UnityEngine;

public class Wall_Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer sr; // Assign from Inspector
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Replacement Settings")]
    [SerializeField] private GameObject replacementWallPrefab; // Prefab-ul pentru zidul de nivel inferior

    private Collider2D col;

    private void Awake()
    {
        currentHealth = maxHealth;
        col = GetComponent<Collider2D>();

        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// Aplică damage zidului.
    /// </summary>
    /// <param name="damage">Cantitatea de damage.</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashWhite());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Inițiază moartea (dispare și apare prefab nou dacă e setat).
    /// </summary>
    private void Die()
    {
        if (col != null) col.enabled = false;
        StartCoroutine(FadeAndReplace());
    }

    /// <summary>
    /// Flash scurt pentru feedback vizual.
    /// </summary>
    private System.Collections.IEnumerator FlashWhite()
    {
        if (sr == null) yield break;

        Color originalColor = sr.color;
        sr.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    /// <summary>
    /// Fade out și instanțierea noului prefab.
    /// </summary>
    private System.Collections.IEnumerator FadeAndReplace()
    {
        if (sr == null) yield break;

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

        // Înlocuire cu un alt zid dacă există un prefab setat
        if (replacementWallPrefab != null)
        {
            Transform currentParent = transform.parent;
            Vector3 spawnPos = currentParent.position;
            Quaternion spawnRot = currentParent.rotation;
            Transform spawnParent = currentParent.parent;

            Instantiate(replacementWallPrefab, spawnPos, spawnRot, spawnParent);
        }

        // Distruge zidul curent (GameObject-ul părinte care conține vizualul + coliderul)
        Destroy(transform.parent.gameObject);
    }
}
