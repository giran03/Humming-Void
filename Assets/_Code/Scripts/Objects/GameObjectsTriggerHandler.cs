using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsTriggerHandler : MonoBehaviour
{
    public GameObject[] flipThisObjectsActiveState;
    GameManager gameManager;
    bool isFlipped;

    private void Start() => gameManager = GameManager.Instance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isFlipped)
            gameManager.FlipState(flipThisObjectsActiveState);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            isFlipped = true;
    }
}
