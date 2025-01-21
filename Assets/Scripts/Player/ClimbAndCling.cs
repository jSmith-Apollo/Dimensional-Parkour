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
    private bool Debounce;
    public float ClingCooldown;

    private IEnumerator ClingReset;

    [Header("PlayerInfo")]
    public PlayerMovement Mover;
    public Transform orientation;
    public Rigidbody rb;
    

    // Start is called before the first frame update
    void Start()
    {
        ClingReset = StopCling();

        Mover = gameObject.GetComponent<PlayerMovement>();
        canCling = true;
        Debounce = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if player is able to cling
        if (Input.GetKey(Mover.jumpKey) && ReadyToCling() && canCling)
        {
            print("readyToCling");
            canCling = false;
            Debounce = true;
            cling();

            //Reset debounce
            Invoke(nameof(ResetDebounce), 0.5f);
        }
        //Check if player is able to climnb
        else if (Input.GetKey(Mover.jumpKey) && Mover.state == PlayerMovement.MovementState.clinging && Physics.SphereCast(transform.position, 0, transform.forward, out RaycastHit hitInfo,2, WhatIsWall) && !Debounce)
        {
            print("Climb");

            Climb();
        }

        if (Mover.state == PlayerMovement.MovementState.clinging)
        {
            rb.velocity = new Vector3(0,-1,0); 
        }
        else if (Mover.state == PlayerMovement.MovementState.climbing)
        {
            rb.velocity = new Vector3(0, 5, 0);
        }

        if (Mover.GetGrounded() && !canCling)
        {
            Invoke(nameof(ResetCooldown), ClingCooldown);
        }
    }

    private void cling()
    {
        Mover.state = PlayerMovement.MovementState.clinging;
        Mover.SetMoveSpeed(0);
        StartCoroutine(ClingReset);
        
    }
 
    private void Climb()
    {
        print("climbing");
        Mover.state = PlayerMovement.MovementState.climbing;
        StopCoroutine(ClingReset);


        Invoke(nameof(StopClimb), climbTime);
    }

    private void StopClimb()
    {
        Mover.state = PlayerMovement.MovementState.walking;
        Mover.SetMoveSpeed(1);
        Invoke(nameof(ResetCooldown), ClingCooldown);
    }

    private IEnumerator StopCling()
    {
        yield return new WaitForSeconds(3);
        print("clingEnded");
        Mover.state = PlayerMovement.MovementState.air;
        Mover.SetMoveSpeed(1);

 }

    public void ResetCooldown()
    {
        print("ResetCooldown");
        canCling = true;
    }

    public void ResetDebounce()
    {
        Debounce = false;
    }

    public bool ReadyToCling()
    {
        return Physics.SphereCast(transform.position, 0, transform.forward, out RaycastHit hitInfo, 1, WhatIsWall) && Mover.state != PlayerMovement.MovementState.clinging;
    }
}
