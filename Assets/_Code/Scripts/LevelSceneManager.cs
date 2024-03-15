using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    [SerializeField] Animator animator = null;
    public static LevelSceneManager Instance;
    string currentScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        QualitySettings.vSyncCount = 1;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            EnableCursor();
    }

    private void Update() => currentScene = SceneManager.GetActiveScene().name;

    public void GoToScene(string sceneName) => SceneManager.LoadSceneAsync(sceneName);

    public void Play()
    {
        ResetGraffitiCount();
        GoToScene("Level 1 Parreno");
    }

    public void Quit()
    {
        ResetGraffitiCount();
        Application.Quit();
    }

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.StopMusic();
        ResetGraffitiCount();
        SceneManager.LoadScene("Main Menu");
    }

    public string CurrentScene() => currentScene;

    public void PauseGame(GameObject pauseMenu)
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        EnableCursor();
    }

    public void ResumeGame(GameObject pauseMenu)
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        DisableCursor();
    }

    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ResetGraffitiCount()
    {
        PlayerPrefs.SetInt("graffitiCount", 0);
        PlayerPrefs.Save();
    }

    // MAIN MENU ANIMATIONS
    public void CreditsScreenTransition() => animator.SetTrigger("creditsShow");
    public void CreditsToMenuTransition() => animator.SetTrigger("creditsToMenu");

    public void ControlsScreenTransition() => animator.SetTrigger("controlsShow");
    public void ControlsToMenuTransition() => animator.SetTrigger("controlsToMenu");
}
