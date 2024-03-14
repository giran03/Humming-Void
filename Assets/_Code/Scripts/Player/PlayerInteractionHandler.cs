using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header ("Level Dimensions")]
    [SerializeField] GameObject[] dimensions;

    [Header("Configs")]
    [SerializeField] GameObject playerCamera;
    [SerializeField] Camera fpsCam;
    [SerializeField] GameObject interactText;

    [Header("HUD")]
    [SerializeField] TMP_Text graffitiText;
    [SerializeField] TMP_Text keyText;

    [Header("PhysGun Properties")]
    [SerializeField] LayerMask interactLayer;
    [SerializeField] float maxGrabDistance;
    [SerializeField] float minGrabDistance;
    [SerializeField] float scrollSpeed;
    [SerializeField] float dragSpeed;
    [HideInInspector] public int keyCount;
    [HideInInspector] public int graffitiCount;

    float grabDistance;
    Quaternion rotationOffset;
    Rigidbody selectedObject;
    Vector3 grabOffset;
    Vector3 selectedObjectPosition;
    Vector3 grabForce;
    Ray ray;

    private void Start()
    {
        ResetStats();
    }

    private void Update()
    {
        CheckForInteractableHover();

        graffitiText.SetText("X " + graffitiCount);
        keyText.SetText("X " + keyCount);

        if (Input.GetKeyDown(KeyCode.E))
            CheckForInteractable();

        KeyInputs();
        grabDistance = Mathf.Clamp(grabDistance + Input.mouseScrollDelta.y * scrollSpeed, minGrabDistance, maxGrabDistance);
        Physics.SyncTransforms();
    }

    private void FixedUpdate()
    {
        ray = fpsCam.ViewportPointToRay(Vector3.one * .5f);

        if (selectedObject != null)
        {
            selectedObjectPosition = ray.origin + ray.direction * grabDistance - selectedObject.transform.TransformVector(grabOffset);
            var forceDir = selectedObjectPosition - selectedObject.position;
            grabForce = forceDir / Time.fixedDeltaTime * dragSpeed / selectedObject.mass; // 0.3f
            selectedObject.velocity = grabForce;
            selectedObject.transform.rotation = playerCamera.transform.rotation * rotationOffset;    // set rotation of object based on camera | disabled to use manual rotation
        }
    }

    void KeyInputs()
    {
        if (Input.GetMouseButtonDown(0))
            Grab();
        if (Input.GetMouseButtonUp(0))
            if (selectedObject)
                Release();
    }

    private void Grab()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, interactLayer)
            && hit.rigidbody != null && !hit.rigidbody.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX("Cube_Grab", transform.position, true);
            rotationOffset = Quaternion.Inverse(fpsCam.transform.rotation) * hit.rigidbody.rotation;
            grabOffset = hit.transform.InverseTransformVector(hit.point - hit.transform.position);
            grabDistance = hit.distance;
            selectedObject = hit.rigidbody;
            selectedObject.useGravity = false;
        }
    }

    private void Release()
    {
        AudioManager.Instance.StopSFX();
        selectedObject.useGravity = true;
        selectedObject = null;
    }

    private void CheckForInteractable()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, interactLayer))
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactOjb))
                interactOjb.Interact();
    }

    private void CheckForInteractableHover()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, interactLayer))
        {
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactOjb))
                interactText.SetActive(true);
        }
        else
            interactText.SetActive(false);
    }

    void ResetStats()
    {
        keyCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            keyCount++;
            Debug.Log("Key Count" + keyCount);
            AudioManager.Instance.PlaySFX("Key_PickUp", other.transform.position);
            var key = other.gameObject;
            Destroy(key);
        }

        if (other.gameObject.CompareTag("Paper"))
        {
            Debug.Log("Ancient Paper Interacted");
            // Iterate through the children of the entered object.
            for (int i = 0; i < other.transform.childCount; i++)
            {
                Transform childTransform = other.transform.GetChild(i);

                if (childTransform.name == "LinkedDestination")
                {
                    GameManager.Instance.ChangeDimensions(dimensions);
                    transform.SetPositionAndRotation(childTransform.position, childTransform.rotation);
                    Physics.SyncTransforms();
                    AudioManager.Instance.PlaySFX("Paper_PickUp", childTransform.position);
                }
            }
        }

        if (other.gameObject.CompareTag("Finish"))
            LevelSceneManager.Instance.GoToScene("Level 2 Cifra");
            
    }
}
