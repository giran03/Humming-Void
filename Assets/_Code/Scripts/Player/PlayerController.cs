using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float groundDrag;
    float moveSpeed;

    [Header("Jumping")]
    [SerializeField] bool enableJump;
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode escMenuKey = KeyCode.Escape;
    
    // Presentation
    [SerializeField] KeyCode endGame = KeyCode.Alpha0;

    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;

    [Header("Flashlight")]
    [SerializeField] KeyCode flashlightKey = KeyCode.F;
    [SerializeField] GameObject flashlightObj;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] string[] layerMasks;
    LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    [Header("Configs")]
    [SerializeField] Transform orientation;
    [SerializeField] float gravityValue;

    [Header("Footsteps SFX")]
    [SerializeField] AudioClip[] tileSteps;
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
        whatIsGround = LayerMask.GetMask(layerMasks);
        readyToJump = true;
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
        else
            rb.drag = 0;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

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

        if (!enableJump) return;
        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void StateHandler()
    {
        // state - Sprinting
        if (grounded && Input.GetKey(sprintKey))
            moveSpeed = sprintSpeed;

        // state - Walking
        else if (grounded)
            moveSpeed = walkSpeed;

        // state - Air
        else
            rb.AddForce(gravityValue * rb.mass * Physics.gravity, ForceMode.Force);
    }

    void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(20f * moveSpeed * GetSlopeMoveDirection(), ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection.normalized, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
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
            int index = Random.Range(0, tileSteps.Length);
            AudioClip footstepSound = tileSteps[index];
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
