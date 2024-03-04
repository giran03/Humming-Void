using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField] GameObject playerCamera;
    [SerializeField] Camera fpsCam;


    [Header("PhysGun Properties")]
    [SerializeField] LayerMask interactLayer;
    [SerializeField] float maxGrabDistance;
    [SerializeField] float minGrabDistance;
    [SerializeField] float scrollSpeed;
    [SerializeField] float dragSpeed;
    [HideInInspector] public int keyCount;
    [HideInInspector] public int graffitiCount;

    GameManager gameManager;
    float grabDistance;
    Quaternion rotationOffset;
    Rigidbody selectedObject;
    Vector3 grabOffset;
    Vector3 selectedObjectPosition;
    Vector3 grabForce;
    Ray ray;

    private void Start()
    {
        gameManager = GameManager.Instance;
        ResetStats();
    }

    private void Update()
    {
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
            rotationOffset = Quaternion.Inverse(fpsCam.transform.rotation) * hit.rigidbody.rotation;
            grabOffset = hit.transform.InverseTransformVector(hit.point - hit.transform.position);
            grabDistance = hit.distance;
            selectedObject = hit.rigidbody;
            selectedObject.useGravity = false;
        }
    }

    private void Release()
    {
        selectedObject.useGravity = true;
        selectedObject = null;
    }

    private void CheckForInteractable()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, interactLayer))
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactOjb))
                interactOjb.Interact();
    }

    void ResetStats()
    {
        keyCount = 0;
        graffitiCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            keyCount++;
            Debug.Log("Key Count" + keyCount);
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
                    gameManager.ChangeDimensions();
                    transform.SetPositionAndRotation(childTransform.position, childTransform.rotation);
                }
            }
        }
    }
}
