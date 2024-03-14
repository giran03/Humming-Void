using System;
using UnityEngine;

// TODO: CREDITS SCREEN
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            LevelSceneManager.Instance.GoToScene("Level 1 Parreno");
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            LevelSceneManager.Instance.GoToScene("Level 2 Cifra");
    }

    public void FlipState(GameObject[] gameObjects)
    {
        foreach (GameObject obj in gameObjects)
            obj.SetActive(!obj.activeSelf);
    }

    public void ChangeDimensions(GameObject[] dimensions)
    {
        foreach (GameObject obj in dimensions)
            obj.SetActive(!obj.activeSelf);
    }

    public void TriggerAnimation(Animator anim, string animName) => anim.SetTrigger(animName);
}
