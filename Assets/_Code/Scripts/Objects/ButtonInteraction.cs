using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour, IInteractable
{
    [Header("Button Configs")]
    [SerializeField] float buttonCooldownDuration;
    [SerializeField] GameObject lightSphere;
    [SerializeField] Material lightsOff;
    [SerializeField] Material lightsOnGreen;
    [SerializeField] Material lightsOnRed;



    [Header("Button Interaction Type")]
    [SerializeField] ButtonType buttonType;


    [Header("Button Type - Activation Settings")]
    [Tooltip("'EnabledObjects' and 'DisabledObjects' active state will be flipped")]
    [SerializeField] GameObject[] buttonActivatedObjects;

    [Header("Button Type - Animation Trigger")]
    [SerializeField] Animator[] animator;
    [SerializeField] string[] animName;


    // If using this type; Make sure the buttons are equal to total length of 'solutionCode' in script 'PuzzleUnlockerHandler'
    [Header("Button Type - Puzzle Digit")]

    [Tooltip("this button's digit to input to PuzzleUnlockerHandler's solutionCode")]
    [SerializeField] int answerDigit;

    [Tooltip("drag the puzzle to be unlocked here")]
    [SerializeField] PuzzleUnlockerHandler puzzleUnlockerHandler;


    bool isButtonOnCooldown;
    bool buttonPressed;
    Animator buttonAnim;
    Renderer rend;
    int currentIndex;

    enum ButtonType
    {
        Nothing,
        Activation,
        AnimationTrigger,
        PuzzleDigit
    }

    private void Start()
    {
        buttonAnim = GetComponent<Animator>();
        rend = lightSphere.GetComponent<Renderer>();
    }

    public void Interact()
    {
        if (isButtonOnCooldown) return;

        buttonAnim.SetTrigger("ButtonPush");

        if (buttonType == ButtonType.Activation)
            ButtonPress(true);

        if (buttonType == ButtonType.AnimationTrigger)
            TriggerAnimation();

        if (buttonType == ButtonType.PuzzleDigit)
            ButtonCode();

        StartCoroutine(ButtonCooldown());
    }

    private void Update()
    {
        if (currentIndex >= animName.Length)
            currentIndex = 0;
    }

    void TriggerAnimation()
    {
        if (animator.Length == 1 && animName.Length == 1)
            animator[0].SetTrigger(animName[0]);
        else
            for (int i = 0; i < animator.Length; i++)
                animator[i].SetTrigger(animName[currentIndex]);
        currentIndex++;
    }

    // flips the active state of the game objects in this array
    // use this if you want to enable some game objects
    void ButtonPress(bool enable)
    {
        if (buttonPressed != enable)
        {
            foreach (GameObject obj in buttonActivatedObjects)
                obj.SetActive(!obj.activeSelf);
            buttonPressed = enable;
        }
        buttonPressed = !buttonPressed;
    }

    public void ButtonCode() => puzzleUnlockerHandler.answerCode.Add(answerDigit);

    IEnumerator ButtonCooldown()
    {
        isButtonOnCooldown = true;
        rend.material = lightsOnGreen;
        yield return new WaitForSeconds(buttonCooldownDuration);
        buttonAnim.SetTrigger("ButtonUp");
        rend.material = lightsOff;
        isButtonOnCooldown = false;
    }

    public void ButtonRedLight() => StartCoroutine(WarningLight());

    IEnumerator WarningLight()
    {
        isButtonOnCooldown = true;
        rend.material = lightsOnRed;
        yield return new WaitForSeconds(buttonCooldownDuration);
        rend.material = lightsOff;
        isButtonOnCooldown = false;
    }
}
