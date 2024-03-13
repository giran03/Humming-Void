using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsTriggerHandler : MonoBehaviour
{
    [Header("Choose Trigger Type")]    // select one trigger type
    [SerializeField] TriggerType triggerType;

    [Header("Trigger Type - Player Settings")]
    [SerializeField] GameObject[] flipThisObjectsActiveState;

    [Header("Trigger Animation")]
    [SerializeField] bool triggerAnimation;
    [SerializeField] Animator animator;
    [SerializeField] string animationTriggerName;


    [Header("Trigger Will Flip Light Switch")]
    [SerializeField] bool toggleLightSwitch;

    [Header("Trigger will also render region")]
    [SerializeField] bool renderRegion;

    LightsFlickerController lightsFlickerController;
    GameObject[] lightCollection;

    GameManager gameManager;
    bool isTriggered;
    bool sfxPlayed;

    enum TriggerType
    {
        Player,
        VoidCube,
        LightCube,
        BothCube,
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        // lightsFlickerController = GameObject.FindGameObjectWithTag("Light").GetComponent<LightsFlickerController>();
        lightCollection = GameObject.FindGameObjectsWithTag("Light");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isTriggered && triggerType == TriggerType.Player)
            Activated();

        if (other.gameObject.CompareTag("VoidCube") && !isTriggered && triggerType == TriggerType.VoidCube)
            Activated();

        if (other.gameObject.CompareTag("LightCube") && !isTriggered && triggerType == TriggerType.LightCube)
            Activated();

        if (other.gameObject.CompareTag("VoidCube") && other.gameObject.CompareTag("LightCube") &&
        !isTriggered && triggerType == TriggerType.BothCube)
            Activated();
    }

    void Activated()
    {
        if (renderRegion)
            gameManager.FindLights();

        gameManager.FlipState(flipThisObjectsActiveState);
        isTriggered = true;

        if (triggerAnimation)
        {
            gameManager.TriggerAnimation(animator, animationTriggerName);
            if (!sfxPlayed)
            {
                sfxPlayed = true;
                AudioManager.Instance.PlaySFX("Buzzer", animator.gameObject.transform.position);
                AudioManager.Instance.PlaySFX("MetalBars", animator.gameObject.transform.position);
            }
        }

        if (toggleLightSwitch)
        {
            AudioManager.Instance.PlaySFX("Lights", transform.position);
            foreach (var obj in lightCollection)
            {
                lightsFlickerController = obj.GetComponent<LightsFlickerController>();
                lightsFlickerController.FlipLightSwitch();
            }
        }
    }
}
