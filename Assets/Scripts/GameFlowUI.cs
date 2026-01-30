using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject hudRoot; // drag HUD here
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    [Header("Systems")]
    [SerializeField] private AREncounterController encounterController;
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        ShowMainMenu();
    }

    public void OnPressPlay()
    {
        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        
        SetHudVisible(true);
        // Start run
        if (playerHealth) playerHealth.ResetHealth();
        if (encounterController) encounterController.ResetRun();

        // Enable controller logic (GPS navigation)
        if (encounterController) encounterController.enabled = true;
    }

    public void OnGameOver()
    {
        gameOverPanel.SetActive(true);
        winPanel.SetActive(false);
        
        SetHudVisible(false);
        if (encounterController) encounterController.enabled = false;
    }

    public void OnWin()
    {
        winPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        
        SetHudVisible(false);

        if (encounterController) encounterController.enabled = false;
    }

    public void OnPressRetry()
    {
        // simplest: reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ShowMainMenu()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        
        SetHudVisible(false);
        
        // Freeze gameplay logic until Play
        if (encounterController) encounterController.enabled = false;
    }
    
    private void SetHudVisible(bool visible)
    {
        if (hudRoot) hudRoot.SetActive(visible);
    }

}