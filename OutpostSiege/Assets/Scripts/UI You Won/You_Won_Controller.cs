using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class You_Won_Controller : MonoBehaviour
{
    [SerializeField] private GameObject youWon;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject coinIcon;
    [SerializeField] private GameObject coinText;
    [SerializeField] private GameObject dialogBox;
    [HideInInspector] public static bool isWon;

    public void Start()
    {
        isWon = false;
    }

    public void TriggerYouWon()
    {
        Time.timeScale = 0f;
        youWon.SetActive(true);
        pauseMenu.SetActive(false);
        pauseButton.SetActive(false);
        coinIcon.SetActive(false);
        coinText.SetActive(false);
        dialogBox.SetActive(false);
        isWon = true;
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        StartCoroutine(ShowUIElementsWithDelay(1f));
    }

    private IEnumerator ShowUIElementsWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Uses real time, unaffected by Time.timeScale
        Time.timeScale = 1f;
        pauseButton.SetActive(true);
        coinIcon.SetActive(true);
        coinText.SetActive(true);
    }
}
