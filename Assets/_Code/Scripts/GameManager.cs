using System;
using UnityEngine;

// TODO: CREDITS SCREEN
// TODO: LEVEL TRANSITION
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] dimensions;
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
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
        FindLights();
    }

    public void FindLights()
    {
        FirstPersonCameraController firstPersonCameraController = GameObject.Find("Camera Holder").GetComponent<FirstPersonCameraController>();
        firstPersonCameraController.flourescentLightCollection = GameObject.FindGameObjectsWithTag("Light");
    }

    public void TriggerAnimation(Animator anim, string animName) => anim.SetTrigger(animName);
}
