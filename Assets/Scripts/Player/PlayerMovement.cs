using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private int TimeCount = 0;
    private int SlideTime = 0;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float walkAcceleration;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    private float jumpForceAtTime;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    private float startDrag;

    [Header("Sliding")]
    public float slideSpeed;
    public float slideYScale;
    public float slideDrag;
    private float slideForceAtTime;
    public float slideCooldown;
    bool readyToSlide;
    public float slideDecel;
    private float startSlideDecel;
    public float slideCurrentSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode testKey1 = KeyCode.Z;
    public KeyCode testKey2 = KeyCode.X;
    public KeyCode testKey3 = KeyCode.C;
    public KeyCode testKey4 = KeyCode.V;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public LayerMask whatIs4DGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Other Stuff")]
    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 lockedDir;

    Rigidbody rb;

    private bool test1Pressed;
    private bool test2Pressed;
    private bool test3Pressed;
    private bool test4Pressed;

    public Vaulting vault;
    public DimensionNavigation nav;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        clinging,
        climbing,
        air,
        sliding,
        idle,
        vaulting,
        phasing
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        readyToSlide = true;

        startYScale = transform.localScale.y;
        startDrag = groundDrag;
        slideCurrentSpeed = slideSpeed;
        startSlideDecel = slideDecel;
    }

    private void Update()
    {
        // Ground check
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        if(Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround) || (Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIs4DGround) && nav.IsIn4D()))
        {
            grounded = true;
        }
        else
            grounded = false;

        MyInput();
        SpeedControl();
        StateHandler();
        VelocityUpdate();
        UpdateUI();

        // Testing things
        /*
        if(grounded)
            Debug.Log("Grounded");
        if (readyToJump)
            Debug.Log("ready to jump");
        if (OnSlope())
            Debug.Log("On a slope");

        // Handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        */


        //Debug.Log("State: "+state);
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        // When to jump (Checks if ready to jump, on ground or a slope, and not close to a wall and can climb)
        if (!vault.canVault() && (Input.GetKey(jumpKey) && readyToJump && (grounded || OnSlope()) && !gameObject.GetComponent<ClimbAndCling>().ReadyToCling()))
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }


        // Start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            if (state == MovementState.idle || slideCurrentSpeed <= 1)
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                //readyToSlide = false;
            }

            else if (state == MovementState.walking || state == MovementState.sliding)
            {
                if (state == MovementState.walking)
                    lockedDir = moveDirection.normalized;
                state = MovementState.sliding;
                //readyToSlide = false;
                Slide();
            }
        }

        // Stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            readyToSlide = false;
            slideCurrentSpeed = slideSpeed;
            Invoke(nameof(ResetSlide), slideCooldown);
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }


        // Test button
        if (Input.GetKeyDown(testKey1))
        {
            if (!test1Pressed)
            {
                //GameObject.Find("TestCube").GetComponent<Actor>().Move(379.0316f, 1.47f, 328.306f);
            }
            test1Pressed = true;
        }
        if (Input.GetKeyUp(testKey1))
            test1Pressed = false;

        // Test button
        if (Input.GetKeyDown(testKey2))
        {
            if (!test2Pressed)
            {
                //GameObject.Find("TestCube").GetComponent<Actor>().Move(368.32f, 4.67f, 333.9f);
            }
            test2Pressed = true;
        }
        if (Input.GetKeyUp(testKey2))
            test2Pressed = false;

        // Test button
        if (Input.GetKeyDown(testKey3))
        {
            if (!test3Pressed)
            {
                //GameObject.Find("TestCube").GetComponent<Actor>().Move(339.6f, 7.01f, 344.48f);
            }
            test3Pressed = true;
        }
        if (Input.GetKeyUp(testKey3))
            test3Pressed = false;

        // Test button
        if (Input.GetKeyDown(testKey4))
        {
            if (!test4Pressed)
            {
                //GameObject.Find("TestCube").GetComponent<Actor>().Move(375.43f, 0.8f, 354.86f);
            }
            test4Pressed = true;
        }
        if (Input.GetKeyUp(testKey4))
            test4Pressed = false;
    }

    public void StateHandler()
    {
        if (state != MovementState.clinging && state != MovementState.climbing)
        {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            if (state == MovementState.idle || slideCurrentSpeed <= 1)
            {
                state = MovementState.crouching;
                moveSpeed = crouchSpeed;
            }
            else if (state == MovementState.sliding)
            {
                state = MovementState.sliding;
                moveSpeed = slideCurrentSpeed;
            }
        }

        // Mode - Sprinting
        if (grounded && Input.GetKey(sprintKey) && moveSpeed == walkSpeed)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(crouchKey))
            {
                if (state == MovementState.crouching)
                {
                    state = MovementState.crouching;
                    moveSpeed = crouchSpeed;
                }
                else
                {
                    state = MovementState.sliding;
                    moveSpeed = slideCurrentSpeed;
                    readyToSlide = false;
                }
            }
            else
                state = MovementState.walking;

        }

        // Mode - Idle
        else if (grounded)
        {
            moveSpeed = 1;
            state = MovementState.idle;
        }


        // Mode - Air
        else
        {
            state = MovementState.air;
        }

    }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // On ground
        if (grounded)
        {
            if (state == MovementState.sliding)
            {
                readyToSlide = false;
                rb.AddForce(lockedDir * moveSpeed * 10f, ForceMode.Force);
            }
            else
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
            

        // In air
        else if(!grounded && !Physics.Raycast(transform.position, orientation.forward, 1))
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // Turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    public void MoveTo(Vector3 newLoc)
    {
        rb.transform.position = newLoc;
    }

    //Out of bounds detection and relocation
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("OutOfBounds"))
        {
            MoveTo(new Vector3(rb.position.x, 20, rb.position.z));
        }
    }

    private void SpeedControl()
    {
        // Limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // Limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        //Debug.Log("exiting a slope");

        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForceAtTime, ForceMode.Impulse);
        Debug.Log("Trying to jump with a force of "+jumpForceAtTime);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public void Slide()
    {
        exitingSlope = true;
        //Debug.Log("exiting a slope");

        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        groundDrag = slideDrag;

            //readyToSlide = false;
            SlideTime++;
            IEnumerator helper = SlideHelper();
            StartCoroutine(helper);
            
        
    }

    private IEnumerator SlideHelper()
    {
        
        while (state == MovementState.sliding)
        {
            //readyToSlide = false;
            slideCurrentSpeed -= slideDecel;
            if (slideCurrentSpeed <= 1)
            {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                state = MovementState.crouching;
                moveSpeed = crouchSpeed;

                Invoke(nameof(ResetSlide), slideCooldown);
                yield break;
            }
            yield return new WaitForSeconds(0.25f);
        }
        
            
    }

    private void ResetSlide()
    {
        print("Resetting slide");
        readyToSlide = true;
        lockedDir = moveDirection.normalized;
        SlideTime = 0;
        slideDecel = startSlideDecel;
        slideCurrentSpeed = slideSpeed;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void UpdateUI()
    {
        GameObject speedText = GameObject.Find("SpeedTxt");
        speedText.gameObject.GetComponent<Text>().text = "Speed: " + moveSpeed;
        GameObject jumpText = GameObject.Find("JumpTxt");
        jumpText.gameObject.GetComponent<Text>().text = "Jump: " + jumpForceAtTime;
    }

    private void VelocityUpdate()
    {
        jumpForceAtTime = jumpForce * (moveSpeed / sprintSpeed) + 5;
        slideForceAtTime = slideSpeed * (moveSpeed / sprintSpeed) + 5;

        if (state == MovementState.walking)
        {
            TimeCount++;
            //print(TimeCount);
            if (TimeCount >= 150)
            {
                TimeCount = 0;
                moveSpeed += walkAcceleration;
                if (moveSpeed >= walkSpeed)
                {
                    moveSpeed = walkSpeed;
                }
            }
        }


        if (state == MovementState.sliding)
        {
            SlideTime++;

            if (SlideTime >= 25)
            {
                SlideTime = 0;
                moveSpeed -= slideDecel;
                slideDecel += 0.01f;

                if (moveSpeed <= 1)
                {
                    state = MovementState.walking;
                }
                
            }
        }

    }
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    public float GetMaxSpeed()
    {
        return walkSpeed;
    }
    public bool GetGrounded()
    {
        return grounded;
    }
    public float GetStartYScale()
    {
        return startYScale;
    }
}