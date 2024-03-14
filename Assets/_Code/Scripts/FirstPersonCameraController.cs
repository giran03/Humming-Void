using UnityEngine;
using UnityEngine.UI;

public class FirstPersonCameraController : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] Transform playerOrientation;
    [SerializeField] Transform playerCamPosition;
    [SerializeField] Transform firstPersonCamera;
    
    [Header("Mouse Sensitivity Slider")]
    [SerializeField] Slider mouseSensitivitySlider;
    float sensX;
    float sensY;

    [Header("Configs")]
    public GameObject[] flourescentLightCollection;
    float xRotation;
    float yRotation;
    private void Awake()
    {
        sensX = PlayerPrefs.GetFloat("sensX", 500f); // default value is 1.0
        sensY = PlayerPrefs.GetFloat("sensY", 500f); // default value is 1.0
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("sensX");
    }
    private void Start()
    {
        flourescentLightCollection = GameObject.FindGameObjectsWithTag("Light");
        mouseSensitivitySlider.onValueChanged.AddListener(AdjustSens);

        LevelSceneManager.Instance.DisableCursor();
    }
    private void Update()
    {
        // follow player
        transform.position = playerCamPosition.position;

        // mouse look
        float mousex = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mousey = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mousex;
        xRotation -= mousey;


        xRotation = Mathf.Clamp(xRotation, -90, 90);

        // rotate cam and orientation
        firstPersonCamera.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
        playerCamPosition.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    void AdjustSens(float value)
    {
        sensX = value;
        sensY = value;

        PlayerPrefs.SetFloat("sensX", sensX);
        PlayerPrefs.SetFloat("sensY", sensY);

        PlayerPrefs.Save();
    }
}