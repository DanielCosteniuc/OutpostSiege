using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class Pause_Menu_Controller : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject coinIcon;
    [SerializeField] private GameObject coinText;
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private Animator animator;
    [SerializeField] private Player_Health palyer;
    public static bool isPaused;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused == false && palyer.isPlayerDead == false)
        {
            PauseGame();
           
        } else if (Input.GetKeyDown(KeyCode.Escape) && isPaused == true && palyer.isPlayerDead == false)
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        coinIcon.SetActive(false);
        coinText.SetActive(false);
        dialogBox.SetActive(false);    
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        coinIcon.SetActive(true);
        coinText.SetActive(true);
        dialogBox.SetActive(true);        
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void MainMenu()
    {        
        SceneManager.LoadScene("MainMenu");
        StartCoroutine(ShowUIElementsWithDelay(1f));
    }

    public void QuitGame()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
        Debug.Log("Exit - button was clicked.");
    }

    private IEnumerator ShowUIElementsWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Uses real time, unaffected by Time.timeScale
        pauseButton.SetActive(true);
        coinIcon.SetActive(true);
        coinText.SetActive(true);
    }

}
