using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Controller : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject exitPanel;

    public void OnStartClick()
    {
        Time.timeScale = 1f;
        Pause_Menu_Controller.isPaused = false;
        SceneManager.LoadScene("Gameplay");
        Debug.Log("Star New Game - button was cliked.");
    }

    public void OnContinueClick ()
    {

    }

    public void OnSettingsClick ()
    {
        CloseAllPanels();
        settingsPanel.SetActive(true);

    }

    public void OnCreditsClick ()
    {
        CloseAllPanels();
        creditsPanel.SetActive(true);
    }

    public void OnExitClick()
    {
        CloseAllPanels();
        exitPanel.SetActive(true);
    }
    
    public void OnConfirmationYes()
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

    public void OnConfirmationNo ()
    {
        CloseAllPanels() ;
    } 

    private void CloseAllPanels ()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        exitPanel.SetActive(false);
    }

    public void OnCloseClick()
    {
        CloseAllPanels ();
    }
}
