using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] Animator doorAnim;
    [SerializeField] bool canOpen;
    [SerializeField] bool requireKey;

    [Header("Reference")]
    PlayerCollisionHandler playerCollisionHandler;

    private void Start()
    {
        // GameObject player = GameObject.FindWithTag("Player").TryGetComponent<PlayerCollisionHandler>(out PlayerCollisionHandler playerCollision);
        // playerCollisionHandler = playerCollision;

        GameObject player = GameObject.FindWithTag("Player");
        player.transform.parent.TryGetComponent<PlayerCollisionHandler>(out PlayerCollisionHandler temp);
        playerCollisionHandler = temp;
    }

    private void Update()
    {
        DoorInteract(requireKey);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            canOpen = true;
        else
            canOpen = false;
    }

    void DoorInteract(bool requireKey)
    {
        if (!requireKey)
            OpenDoor();
        else
            OpenDoor(true);
    }

    void OpenDoor(bool keyCheck = false)
    {

        if (keyCheck && playerCollisionHandler.keyCount <= 0)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (canOpen)
            {
                if(playerCollisionHandler.keyCount > 0)
                    playerCollisionHandler.keyCount--;
                Debug.Log(playerCollisionHandler.keyCount);
                doorAnim.Play("DoorOpen");
                canOpen = false;
            }
            else
            {
                doorAnim.Play("DoorClose");
                canOpen = true;
            }
        }
    }
}
