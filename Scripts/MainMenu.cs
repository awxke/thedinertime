using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Панели UI")]
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    void Start()
    {
        // Скрываем панели при запуске
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // мейн сцена
    }

    // --- МЕТОДЫ ДЛЯ НАСТРОЕК ---
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // --- МЕТОДЫ ДЛЯ АВТОРОВ ---
    public void OpenCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}