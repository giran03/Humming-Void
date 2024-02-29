using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : MonoBehaviour
{
    public static Action<string> OnSceneChanged;
    private void Awake() => OnSceneChanged += (scene) => GoToScene(scene);
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        // KartHandler.CursorLock(false);
    }

    public void GoToScene(string sceneName) => SceneManager.LoadScene(sceneName);

    public void Play() => OnSceneChanged("Level_1");

    public void Quit() => Application.Quit();
}
