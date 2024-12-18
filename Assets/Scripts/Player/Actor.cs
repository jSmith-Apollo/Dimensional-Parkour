using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Actor : MonoBehaviour
{
    [Header("Basic Properties")]
    //public string name;

    [Header("Health")]
    public float health;
    protected float maxhealth;
    protected bool canRegen;
    protected float regenDelay;
    protected float regenAmount;

    [Header("Movement")]
    protected float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float walkAcceleration;
    public float groundDrag;
    protected float maxSpeed;

    [Header("Jumping")]
    public float jumpForce;
    protected float jumpForceAtTime;
    public float jumpCooldown;
    public float airMultiplier;
    protected bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    protected float startYScale;

    [Header("GroundCheck")]
    public float height;
    public LayerMask whatIsGround;
    protected bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    protected RaycastHit slopeHit;
    protected bool exitingSlope;

    public Transform Orientation;
    protected Rigidbody rb;

    [Header("State")]
    public MovementState state;
    public bool canAct;

    protected Vector3 moveDirection;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        dead,
        idle
    }

    public virtual void Start()
    {
        //Update Actor rigidbody// 
        rb = gameObject.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //---------------------//
        //Update player active level//
        canAct = true;
        readyToJump = true;
        grounded = true;
        state = MovementState.idle;
        //-------------------------//

        //Test
        
    }

    public void Update()
    {
        if (health != maxhealth && canRegen && canAct)
        {
            Regen();
        }

        UpdateState();
        UpdateValues();
    }

    public virtual void FixedUpdate()
    {

    }
    protected void crouch(bool mode)
    {
        if (mode)
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        else
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

    }

    protected void sprint(bool mode)
    {
        if (mode)
            maxSpeed = sprintSpeed;
        else
            maxSpeed = walkSpeed;
    }

    protected void jump()
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;

            rb.AddForce(transform.up * jumpForceAtTime, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    /* (Noticed based on research that the Move method has a few problems) 
      #1: doesnt replicate its based method MovePlayer() in PlayerMovement.cs
      #2: updates based on transform instead of rb, Which can lead to physics issues
    */
    public void Move(float x, float y, float z)
    {
        IEnumerator helper = MoveHelper(transform.position, new Vector3(x, y, z));
        StartCoroutine(helper);
    }
    private IEnumerator MoveHelper(Vector3 start, Vector3 end)
    {
        for (float i = 0; i < 1; i += 0.005f)
        {
            yield return new WaitForSeconds(0.01f);
            transform.position = Vector3.Lerp(start, end, i);
            print("X: " + transform.position.x + " | Y: " + transform.position.y + " | Z: " + transform.position.z);
        }
    }

    public void MoveByForce(float x, float y, float z)
    {
        rb.AddForce(new Vector3(x,y,z),ForceMode.Force);
    }

    public void TakeHealth(float amt)
    {
        health -= amt;
        StartCoroutine(RegenTimer());
    }

    public void Death()
    {
        canAct = false;
        //More will be added when other things are implemented
    }

    public void Respawn()
    {
        SetHealth(maxhealth);
        canAct = true;
        readyToJump = true;
    }

    private IEnumerator RegenTimer()
    {
        if (health < maxhealth && canAct)
        {
            canRegen = false;
            yield return new WaitForSeconds(regenDelay);
            canRegen = true;
        }
    }

    private void Regen()
    {
        health += regenAmount;
        if (health > maxhealth)
        {
            health = maxhealth;
        }
    }

    protected void UpdateState()
    {
        if (canAct == true)
        {
            if (grounded)
            {
                //Check for crouching
                if (transform.localScale.y == crouchYScale)
                {
                    state = MovementState.crouching;
                }
                //Check for walking
                else if (moveSpeed > 1 && moveSpeed < walkSpeed)
                {
                    state = MovementState.walking;
                }
                //check for sprinting
                else if (moveSpeed > walkSpeed)
                {
                    state = MovementState.sprinting;
                }
                else if (rb.velocity == Vector3.zero)
                    state = MovementState.idle;
            }
            else
            {
                state = MovementState.air;
            }
            
            //Check if player is dead
            if (health <= 0)
                state = MovementState.dead;
        }
    }

    protected void UpdateValues()
    {
        if (canAct == true)
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, height * 0.5f + 0.2f, whatIsGround);
            if (state == MovementState.walking)
            {
                //Increase speed by walkAcceleration until reaching walk speed//
                if (moveSpeed < walkSpeed)
                {
                    moveSpeed += walkAcceleration;
                }
                //deceleration if move speed is above walk speed// 
                else if (moveSpeed > walkSpeed)
                {
                    moveSpeed -= walkAcceleration;
                }
                else
                {
                    moveSpeed = walkSpeed;
                }
            }
            else if (state == MovementState.sprinting)
            {
                if (moveSpeed < sprintSpeed)
                {
                    //Increase walk speed by walk acceleration until sprinting //
                    if (moveSpeed < walkSpeed)
                        moveSpeed += walkAcceleration;
                    //when sprinting make acceleration increase by 1.25x //
                    else
                        moveSpeed += 1.25f * walkAcceleration;
                }
                else
                {
                    moveSpeed = sprintSpeed;
                }
            }
            else if (state == MovementState.crouching)
            {
                //jump down to crouch speed if greater than walk speed
                if (moveSpeed >= crouchSpeed)
                    moveSpeed = crouchSpeed;
                else
                    moveSpeed += walkAcceleration;
            }
            else if (state == MovementState.air)
            {

            }
            else if (state == MovementState.dead)
            {
                Death();
            }
        }
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public void SetSpeed(float spd)
    {
        moveSpeed = spd;
    }

    public void SetHealth(float amt)
    {
        health = amt;
    }

    public void SetMaxSpeed(float max)
    {
        maxSpeed = max;
    }

    public string GetState()
    {
        string val = ""+state.ToString();

        return ""+val;
    }

    public bool CheckAct()
    {
        return canAct;
    }
}
