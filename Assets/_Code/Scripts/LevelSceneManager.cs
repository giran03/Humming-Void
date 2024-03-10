using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    public static LevelSceneManager Instance;
    public static Action<string> OnSceneChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        OnSceneChanged += (scene) => GoToScene(scene);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void GoToScene(string sceneName) => SceneManager.LoadSceneAsync(sceneName);

    public void Play() => OnSceneChanged("Game");

    public void Quit() => Application.Quit();
}
