using UnityEngine;

public class Player_Menu_Controller : MonoBehaviour
{
    [Header("Menu buttons")]
    [SerializeField] private GameObject attackLeftButton;
    [SerializeField] private GameObject attackRightButton;
    [SerializeField] private GameObject buyTank;

    [Header("Menu objects")]
    [SerializeField] private GameObject playerMenu;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private Player_Health health;

    private USV_Interactions usv;

    public static Player_Menu_Controller Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        usv = FindFirstObjectByType<USV_Interactions>();
    }

    void Update()
    {
        // Try to find USV if it hasn't been found yet
        if (usv == null)
        {
            usv = FindFirstObjectByType<USV_Interactions>();
        }

        if (Pause_Menu_Controller.isPaused)
        {
            if (playerMenu.activeSelf)
                playerMenu.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.M) && usv != null && health.isPlayerDead == false && usv.IsPaid())
        {
            playerMenu.SetActive(!playerMenu.activeSelf);
        }
    }
}
