using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Controller : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("Gameplay");
        Debug.Log("Star New Game - button was cliked.");
    }

    public void OnExitClick()
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
}
