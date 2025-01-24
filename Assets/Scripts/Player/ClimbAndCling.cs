using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ClimbAndCling : MonoBehaviour
{
    [Header("Layer")]
    public LayerMask WhatIsWall;

    [Header("Climbing variables")]
    private bool canCling;
    public float climbTime;
    public float KickOffStrength;
    private bool Debounce;
    public float ClingCooldown;

    private IEnumerator ClingReset;

    [Header("WallRun Info")]
    private bool canWallRunRight;
    private bool canWallRunLeft;
    public float WallRunTime;
    public float minWallRunSpeed;
    public float wallRunCharAngle;
    private float CharStartAngle;

    

    [Header("PlayerInfo")]
    public PlayerMovement Mover;
    public Transform orientation;
    public Rigidbody rb;

    [Header("DebugOptions")]
    public bool debugMode;
    public float WallRunRadius;
    public float WallRunMaxDistance;
    public float ClingRadius;
    public float ClingMaxDistance;
    public float KickOffRadius;
    public float KickOffMaxDistance;

    // Start is called before the first frame update
    void Start()
    {
        ClingReset = StopCling(2);
        canCling = true;
        canWallRunLeft = true;
        canWallRunRight = true;
        Debounce = false;
    }

    // Update is called once per frame
    void Update()
    {
        //-- Input Checker --//
        //Check if player is able to cling
        if (Input.GetKey(Mover.jumpKey) && ReadyToCling() && canCling)
        {
            canCling = false;
            Debounce = true;
            cling();

            //Reset debounce
            Invoke(nameof(ResetDebounce), 0.5f);
        }
        //Check if player is able to climb
        else if (Input.GetKey(Mover.jumpKey) && Mover.state == PlayerMovement.MovementState.clinging && Physics.SphereCast(transform.position, 0.4f, orientation.forward, out RaycastHit hitInfo, 0.5f, WhatIsWall) && !Debounce)
        {
            Climb();
        }
        //Check if wall is behind to KickOff
        else if (Input.GetKey(Mover.jumpKey) && (Mover.state == PlayerMovement.MovementState.clinging || Mover.state == PlayerMovement.MovementState.climbing) && Physics.SphereCast(transform.position, 0.5f, orientation.forward * -1, out RaycastHit hitInfo2, 0.4f, WhatIsWall) && !Debounce)
        {
            KickOff();
        }
        //Check for wall run to the right
        else if (Input.GetKey(Mover.jumpKey) && (Mover.state == PlayerMovement.MovementState.walking || Mover.state == PlayerMovement.MovementState.clinging) && !Mover.GetGrounded() && ReadyToWallRun(true) && !Debounce)
        {

            WallRun(true);
        }
        //Check for wall run to the left
        else if (Input.GetKey(Mover.jumpKey) && (Mover.state == PlayerMovement.MovementState.walking || Mover.state == PlayerMovement.MovementState.clinging) && !Mover.GetGrounded() && ReadyToWallRun(false) && !Debounce)
        {
            print("wallRunLeft");
            WallRun(false);
        }


        //Velocity updates//
        if (Mover.state == PlayerMovement.MovementState.clinging)
        {
            rb.velocity = new Vector3(0,-1,0); 
        }
        else if (Mover.state == PlayerMovement.MovementState.climbing)
        {
            rb.velocity = new Vector3(0, 3, 0);
        }

        //Cooldown Resets//
        if (Mover.GetGrounded() && !canCling)
        {
            StartCoroutine(StopCling(0.2f));
            Invoke(nameof(ResetCooldown), ClingCooldown);
        }
    }

    // Action Methods//
    private void cling()
    {
        print("Cling");
        Mover.state = PlayerMovement.MovementState.clinging;
        Mover.SetMoveSpeed(2);
        StartCoroutine(ClingReset);
        
    }

    private void KickOff()
    {
        print("KickOff");
        Mover.state = PlayerMovement.MovementState.air;
        StopCoroutine(ClingReset);
        Mover.SetMoveSpeed(KickOffStrength*2);
        rb.velocity = orientation.forward * KickOffStrength * 10f + orientation.up * KickOffStrength/5 ;

    }
    private void Climb()
    {
        print("climbing");
        Mover.state = PlayerMovement.MovementState.climbing;
        StopCoroutine(ClingReset);


        Invoke(nameof(StopClimb), climbTime);
    }

    private void WallRun(bool IsRight)
    {
        if (IsRight)
        {
            print("wallRunRight");
            canWallRunRight = false;
            GameObject.Find("PlayerV2").transform.rotation = GameObject.Find("PlayerV2").transform.rotation * new Quaternion(0, 0, wallRunCharAngle,0);
        }
        else 
        {
            print("wallRunLeft");
            canWallRunLeft = false;
            transform.parent.rotation = transform.parent.rotation * new Quaternion(0, 0, -1 * wallRunCharAngle, 0);

        }
    }

    // Stop Action Methods//
    private void StopClimb()
    {
        if (Mover.state == PlayerMovement.MovementState.climbing)
        {
            Mover.state = PlayerMovement.MovementState.air;
            Mover.SetMoveSpeed(2);
        }
        Invoke(nameof(ResetCooldown), ClingCooldown);
    }

    private IEnumerator StopCling(float t)
    {
        yield return new WaitForSeconds(t);
        Mover.state = PlayerMovement.MovementState.air;

 }

    // Reset Methods//
    public void ResetCooldown()
    {
        canCling = true;
    }

    public void ResetDebounce()
    {
        Debounce = false;
    }


    //Condition Checks//
    public bool ReadyToCling()
    {
        return Physics.SphereCast(transform.position, 0.4f, orientation.forward, out RaycastHit hitInfo, 0.5f, WhatIsWall) && Mover.state != PlayerMovement.MovementState.clinging;
    }

    public bool ReadyToWallRun(bool toRight)
    {
        if (toRight)
        {
            //print(canWallRun && Mover.GetMoveSpeed() >= minWallRunSpeed && Physics.SphereCast(orientation.position, 0.4f, orientation.right, out RaycastHit hitinfotest, 0.5f, WhatIsWall));
            return canWallRunRight && Mover.GetMoveSpeed() >= minWallRunSpeed && Physics.SphereCast(orientation.position, 0.4f, orientation.right, out RaycastHit hitinfo, 0.5f, WhatIsWall);
        }
        else
        {
            //print(canWallRun && Mover.GetMoveSpeed() >= minWallRunSpeed && Physics.SphereCast(orientation.position, 0.4f, orientation.right * -1, out RaycastHit hitinfotest, 0.5f, WhatIsWall));
            return canWallRunLeft && Mover.GetMoveSpeed() >= minWallRunSpeed && Physics.SphereCast(orientation.position, 0.4f, orientation.right * -1, out RaycastHit hitinfo, 0.5f, WhatIsWall);
        }
    }

    private void OnDrawGizmos()
    {
        if (debugMode)
        {
            Gizmos.color = Color.red;
            //Wall Run Checks
            Gizmos.DrawSphere(orientation.position + orientation.right * WallRunMaxDistance * -1, WallRunRadius);
            Gizmos.DrawSphere(orientation.position + orientation.right * WallRunMaxDistance, WallRunRadius);
            //Climb&ClingCheck
            Gizmos.DrawSphere(orientation.position + orientation.forward * ClingMaxDistance, ClingRadius);
            //KickOffCheck//
            Gizmos.DrawSphere(orientation.position + orientation.forward * KickOffMaxDistance * -1, KickOffRadius);

        }
    }
}
