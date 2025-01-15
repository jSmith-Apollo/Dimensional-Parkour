using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerClass : Actor
{
    //Player Information//
    private float points;
    private List<GameObject> inventory;
    private GameObject[] Equipped;

    //Keybinds//
    [Header("keyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode special1Key = KeyCode.Alpha0;
    public KeyCode special2Key = KeyCode.Alpha1;
    public KeyCode special3Key = KeyCode.Alpha2;
    public KeyCode dimensionKey = KeyCode.F;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode DenyKey = KeyCode.X;
    public KeyCode MenuKey = KeyCode.Escape;

    //PlayerInputs//
    float horizontalInput;
    float verticalInput;

    public override void Start()
    {
        base.Start();
        inventory = new List<GameObject>();
        Equipped = new GameObject[3];


    }

    public override void FixedUpdate()
    {
        Move();
        MyInput();
    }

    public void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        CheckKeys();
    }

    public void CheckKeys()
    {
        //Jump when key is down//
        if (Input.GetKey(jumpKey) && readyToJump && (grounded || OnSlope()))
        {
            readyToJump = false;
            jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

            //Sprint when key is down//
            if (Input.GetKeyDown(sprintKey))
            sprint(true);

        //stop sprinting when key is up//
        if (Input.GetKeyUp(sprintKey))
            sprint(false);

        //Crouch when key is down//
        if (Input.GetKeyDown(crouchKey))
            crouch(true);

        //uncrouch when key is up//
        if (Input.GetKeyUp(crouchKey))
            crouch(false);

    }

    public void Move()
    {
        //Calculate Movement direction//

        moveDirection = Orientation.forward * verticalInput + Orientation.right * horizontalInput;

        // On slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        //Move player according to direction when on ground//
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            
        }
        //move slower when in air//
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    public void Interact()
    {

    }

    //For later date//
    public void OpenMenu()
    {

    }

    //For later date//
    public void CloseMenu()
    {

    }

    public void ChangeEquip(GameObject obj,int pos)
    {
        if (pos >= 3 || Equipped[pos] == obj) return;
        Equipped[pos] = obj;
    }

    public float GetPoints()
    {
        return points;
    }

    public void setPoints(float p)
    {
        points = p;
    }
    public void ChangeDimension(int d)
    {

    }

}
