using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] dimensions;
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void FlipState(GameObject[] gameObjects)
    {
        foreach (GameObject obj in gameObjects)
            obj.SetActive(!obj.activeSelf);
    }

    public void ChangeDimensions()
    {
        foreach (GameObject obj in dimensions)
            obj.SetActive(!obj.activeSelf);
    }

    public void TriggerAnimation(Animator anim, string animName) => anim.SetTrigger(animName);
}
