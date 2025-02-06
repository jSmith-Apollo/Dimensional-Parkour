using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ClimbAndCling : MonoBehaviour
{
    [Header("Layer")]
    public LayerMask WhatIsWall;
    public LayerMask WhatIs4DWall;
    public LayerMask WhatIsWallInBoth;
    public LayerMask WhatIsWallAndPT;

    [Header("Climbing variables")]
    private bool canCling;
    public float climbTime;
    public float KickOffStrength;
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
        else if (Input.GetKey(Mover.jumpKey) && Mover.state == PlayerMovement.MovementState.clinging && ((Physics.SphereCast(transform.position, 0, orientation.forward, out RaycastHit hitInfo,2, WhatIsWall) || ((Physics.SphereCast(transform.position, 0, orientation.forward, out hitInfo, 2, WhatIs4DWall) && Mover.nav.IsIn4D()) || ((Physics.SphereCast(transform.position, 0, orientation.forward, out hitInfo, 2, WhatIsWallAndPT)) && !Mover.nav.IsIn4D()) || (Physics.SphereCast(transform.position, 0, orientation.forward, out hitInfo, 2, WhatIsWallInBoth)))) && !Debounce))
        {
            Climb();
        }
        //Check if wall is behind to KickOff
        else if (Input.GetKey(Mover.jumpKey) && (Mover.state == PlayerMovement.MovementState.clinging || Mover.state == PlayerMovement.MovementState.climbing) && Physics.SphereCast(transform.position, 0, orientation.forward * -1, out RaycastHit hitInfo2, 3, WhatIsWall) && !Debounce)
        {
            KickOff();
        }

        if (Mover.state == PlayerMovement.MovementState.clinging)
        {
            rb.velocity = new Vector3(0,-1,0); 
        }
        else if (Mover.state == PlayerMovement.MovementState.climbing)
        {
            rb.velocity = new Vector3(0, 3, 0);
        }

        if (Mover.GetGrounded() && !canCling)
        {
            Invoke(nameof(ResetCooldown), ClingCooldown);
        }
    }

    private void cling()
    {
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

    private void StopClimb()
    {
        Mover.state = PlayerMovement.MovementState.air;
        Mover.SetMoveSpeed(2);
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
        return Physics.SphereCast(transform.position, 0, orientation.forward, out RaycastHit hitInfo, 1, WhatIsWall) && Mover.state != PlayerMovement.MovementState.clinging;
    }
}
