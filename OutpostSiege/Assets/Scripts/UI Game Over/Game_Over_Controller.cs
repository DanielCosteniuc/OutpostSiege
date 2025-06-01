using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Over_Controller : MonoBehaviour
{
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject coinIcon;
    [SerializeField] private GameObject coinText;
    [SerializeField] private GameObject dialogBox;
    public void TriggerGameOver()
    {
        gameOverMenu.SetActive(true);
        pauseMenu.SetActive(false);
        pauseButton.SetActive(false);
        coinIcon.SetActive(false);
        coinText.SetActive(false);
        dialogBox.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        StartCoroutine(ShowUIElementsWithDelay(1f));
    }

    private IEnumerator ShowUIElementsWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Uses real time, unaffected by Time.timeScale
        pauseButton.SetActive(true);
        coinIcon.SetActive(true);
        coinText.SetActive(true);
    }
}
