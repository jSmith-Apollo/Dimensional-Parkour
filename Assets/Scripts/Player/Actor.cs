using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

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
    public float maxSpeed;

    [Header("Jumping")]
    public float jumpForce;
    protected float jumpForceAtTime;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    protected float startYScale;

    [Header("GroundCheck")]
    public float height;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    protected float maxSlopeAngle;
    protected RaycastHit slopeHit;
    protected bool exitingSlope;

    public Transform Orientation;
    public Rigidbody rb;

    public bool canAct;
    public MovementState state;
    public Vector3 moveDirection;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        dead
    }

    public void Start()
    {
        canAct = true;
        readyToJump = true;
        grounded = true;

        //Test
        
    }

    public void Update()
    {
        if (canAct)
        {
            canRegen = true;
        }
        else
        {
            canRegen = false;
            readyToJump = false;
            SetSpeed(0);
        }
        if(health <= 0)
        {
            Death();
        }

        if (health != maxhealth && canRegen)
        {
            Regen();
        }
    }

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

    public void TakeHealth(float amt)
    {
        health -= amt;
        StartCoroutine(RegenTimer());
    }

    public void Death()
    {
        state = MovementState.dead;
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
        if (health < maxhealth)
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
