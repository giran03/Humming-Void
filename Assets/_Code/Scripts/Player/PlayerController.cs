using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float groundDrag;
    float moveSpeed;

    [Header("Keybinds")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode escMenuKey = KeyCode.Escape;
    // Presentation Key Binds
    [SerializeField] KeyCode endGame = KeyCode.Alpha0;

    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;

    [Header("Flashlight")]
    [SerializeField] KeyCode flashlightKey = KeyCode.F;
    [SerializeField] GameObject flashlightObj;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask whatIsGround;
    bool grounded;

    [Header("Configs")]
    [SerializeField] Transform orientation;

    [Header("Footsteps SFX")]
    [SerializeField] AudioClip[] footSteps;
    AudioSource audioSource;

    float horizontalInput;
    float verticalInput;

    (Vector3, Quaternion) initialPosition;
    Vector3 moveDirection;
    Rigidbody rb;
    bool isPlaying;
    Vector3 flatVel;
    bool isPaused;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        initialPosition = (transform.position, transform.rotation);
    }

    void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        PlayerBounds();

        if (grounded && verticalInput != 0 || horizontalInput != 0)
            PlayFootstepSound();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
    }

    void FixedUpdate() => MovePlayer();

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(flashlightKey))
        {
            AudioManager.Instance.PlaySFX("Flashlight", transform.position);
            flashlightObj.SetActive(!flashlightObj.activeSelf);
        }

        //esc menu
        if (Input.GetKeyDown(escMenuKey))
        {
            isPaused = !isPaused;
            if (isPaused)
                LevelSceneManager.Instance.PauseGame(pauseMenu);
            else
                LevelSceneManager.Instance.ResumeGame(pauseMenu);
        }

        if (Input.GetKeyDown(endGame))
            LevelSceneManager.Instance.GoToScene("End Screen");
    }

    void StateHandler()
    {
        // state - Sprinting
        if (grounded && Input.GetKey(sprintKey))
            moveSpeed = sprintSpeed;

        // state - Walking
        else if (grounded)
            moveSpeed = walkSpeed;
    }

    void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
    }

    void SpeedControl()
    {
        // limiting speed on ground
        flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void PlayerBounds()
    {
        if (transform.position.y < -20)
            RespawnPlayer();
    }

    void RespawnPlayer()
    {
        var (pos, rot) = initialPosition;
        transform.SetPositionAndRotation(pos, rot);
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        Physics.SyncTransforms();
    }

    public void PlayFootstepSound()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            int index = Random.Range(0, footSteps.Length);
            AudioClip footstepSound = footSteps[index];
            audioSource.PlayOneShot(footstepSound);
            audioSource.pitch = flatVel.magnitude > 10f ? 2.5f : 1f;
            StartCoroutine(Cooldown(footstepSound.length));
        }
    }

    IEnumerator Cooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPlaying = false;
    }

    public void TransitionLevel(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Physics.SyncTransforms();
    }
}
