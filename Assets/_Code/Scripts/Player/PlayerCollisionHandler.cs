using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    public int keyCount;
    GameManager gameManager;

    private void Awake() => gameManager = GameManager.Instance;

    private void Start() => keyCount = 0;
        
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            Debug.Log("Got a key!");
            keyCount++;
            var key = other.gameObject;
            Destroy(key);
            Debug.Log(keyCount);
        }
        
        if (other.gameObject.CompareTag("Paper"))
        {
            // Iterate through the children of the entered object.
            for (int i = 0; i < other.transform.childCount; i++)
            {
                Transform childTransform = other.transform.GetChild(i);

                if (childTransform.name == "LinkedDestination")
                {
                    gameManager.ChangeDimensions();
                    transform.SetPositionAndRotation(childTransform.position,childTransform.rotation);
                }
            }
        }
    }
}
