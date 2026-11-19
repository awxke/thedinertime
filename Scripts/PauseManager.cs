using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("UI Панели")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);

        // Принудительно выключаем настройки при старте 
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        if (pausePanel == null)
        {
            Debug.LogError("[PauseManager] Не привязан Pause Panel!");
            return;
        }

        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PlayerMovement.Instance != null)
            PlayerMovement.Instance.SetMovementState(false);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);

        // Закрываем настройки, если они были открыты
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (PlayerMovement.Instance != null)
            PlayerMovement.Instance.SetMovementState(true);
    }

    public void OpenSettings()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}