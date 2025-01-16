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
    private float slideSpeedAtTime;
    public float slideCooldown;
    bool readyToSlide;
    public float slideDecel;
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
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private bool test1Pressed;
    private bool test2Pressed;
    private bool test3Pressed;
    private bool test4Pressed;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        sliding,
        idle
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
        startDrag = groundDrag;
        slideCurrentSpeed = slideSpeed;
    }

    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

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

        Debug.Log(""+state);
    }
        

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // When to jump
        if (Input.GetKey(jumpKey) && readyToJump && (grounded || OnSlope()))
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        
            // Start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            if (state == MovementState.idle)
            {
                    transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                    rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }

            else if (state == MovementState.walking || state == MovementState.sliding && readyToSlide)
            {
                state = MovementState.sliding;
                //readyToSlide = false;

                Slide();

                Invoke(nameof(ResetSlide), slideCooldown);
            }
        }

        // Stop crouch
            if (Input.GetKeyUp(crouchKey))
            {
                if (state == MovementState.idle)
                {
                    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                }
                else if (state == MovementState.walking || state == MovementState.sliding)
                {
                    state = MovementState.sliding; state = MovementState.sliding;
                    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                    groundDrag = startDrag;
                }
            }


        // Test button
        if (Input.GetKeyDown(testKey1))
        {
            if (!test1Pressed)
            {
                GameObject.Find("TestCube").GetComponent<Actor>().Move(379.0316f, 1.47f, 328.306f);
            }
            test1Pressed = true;
        }
        if(Input.GetKeyUp(testKey1))
            test1Pressed = false;

        // Test button
        if (Input.GetKeyDown(testKey2))
        {
            if (!test2Pressed)
            {
                GameObject.Find("TestCube").GetComponent<Actor>().Move(368.32f, 4.67f, 333.9f);
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
                GameObject.Find("TestCube").GetComponent<Actor>().Move(339.6f, 7.01f, 344.48f);
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
                GameObject.Find("TestCube").GetComponent<Actor>().Move(375.43f, 0.8f, 354.86f);
            }
            test4Pressed = true;
        }
        if (Input.GetKeyUp(testKey4))
            test4Pressed = false;
    }

    public void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(crouchKey))
            {
                state = MovementState.sliding;
                moveSpeed = slideCurrentSpeed;
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
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // In air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // Turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // Limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        /*
        else if (state == MovementState.sliding)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Limit velocity if needed
            if (flatVel.magnitude > slideSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * slideSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        */

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
        //Debug.Log("Trying to jump");
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
        //rb.AddForce(moveDirection.normalized * slideSpeedAtTime * 10f, ForceMode.Impulse);
        groundDrag = slideDrag;

        
        while (slideCurrentSpeed > 0f )
        {
            SlideTime++;
            IEnumerator helper = SlideHelper();
            StartCoroutine(helper);
        }

        if (slideCurrentSpeed == 0f)
        {
            ResetSlide();
        }
        
    }

    private IEnumerator SlideHelper()
    {
        slideCurrentSpeed -= 0.5f;
        yield return new WaitForSeconds(1f);
            
    }

    private void ResetSlide()
    {
        readyToSlide = true;

        SlideTime = 0;
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
        slideSpeedAtTime = slideSpeed * (moveSpeed / sprintSpeed) + 5;

        if (state == MovementState.walking)
        {
            TimeCount++;
            //print(TimeCount);
            if (TimeCount >= 25)
            {
                TimeCount = 0;
                moveSpeed += walkAcceleration;
                if (moveSpeed >= walkSpeed)
                {
                    moveSpeed = walkSpeed;
                }
            }
        }


        //if (state == MovementState.sliding)
        //{
        //    SlideTime++;

        //    if (SlideTime >= 25)
        //    {
        //        TimeCount = 0;
        //        moveSpeed -= slideDecel;

        //        if (moveSpeed <= 1)
        //        {
        //            state = MovementState.walking;
        //        }
        //        /*
        //        if (moveSpeed <= slideSpeed)
        //        {
        //            moveSpeed = slideSpeed;
        //        }
        //        */
        //    }
        //}

    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    public float GetMaxSpeed()
    {
        return walkSpeed;
    }
}