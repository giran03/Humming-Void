using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    public static LevelSceneManager Instance;
    public static Action<string> OnSceneChanged;
    string currentScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        // OnSceneChanged += (scene) => GoToScene(scene);

        QualitySettings.vSyncCount = 1;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            EnableCursor();
    }

    private void Update() => currentScene = SceneManager.GetActiveScene().name;

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void Play() => GoToScene("Level 1 Parreno");

    public void Quit() => Application.Quit();

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.StopMusic();
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
}
