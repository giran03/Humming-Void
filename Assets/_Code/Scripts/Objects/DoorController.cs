using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Configs")]
    [SerializeField] bool requireKey;
    [SerializeField] float autoCloseDoorCooldown;
    [SerializeField] GameObject findKeyInfo;

    bool isDoorOnCooldown;
    bool isDoorOpen;

    Animator doorAnim;
    PlayerInteractionHandler playerCollisionHandler;

    private void Start()
    {
        doorAnim = GetComponentInChildren<Animator>();
        Transform parent = GameObject.FindWithTag("Player").transform.parent;
        playerCollisionHandler = parent.GetComponent<PlayerInteractionHandler>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            if (Input.GetKey(KeyCode.E))
                if (requireKey && playerCollisionHandler.keyCount > 0 && !isDoorOpen && !isDoorOnCooldown)
                {
                    UseDoor();
                    if (playerCollisionHandler.keyCount > 0)
                    {
                        playerCollisionHandler.keyCount--;
                        requireKey = false;
                    }
                }
                else if (requireKey && playerCollisionHandler.keyCount <= 0)
                    StartCoroutine(ShowKeyInfo());
                else if (!requireKey && !isDoorOpen && !isDoorOnCooldown)
                    UseDoor();
    }

    public void UseDoor()
    {
        AudioManager.Instance.PlaySFX("Door_Open", gameObject.transform.position);
        doorAnim.SetTrigger("OpenDoor");
        isDoorOpen = true;
        StartCoroutine(DoorCooldown());
    }

    IEnumerator DoorCooldown()
    {
        isDoorOnCooldown = true;
        yield return new WaitForSeconds(autoCloseDoorCooldown);
        AudioManager.Instance.PlaySFX("Door_Close", gameObject.transform.position);
        doorAnim.SetTrigger("CloseDoor");
        isDoorOpen = false;
        isDoorOnCooldown = false;
    }

    IEnumerator ShowKeyInfo()
    {
        findKeyInfo.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        findKeyInfo.SetActive(false);
    }
}
