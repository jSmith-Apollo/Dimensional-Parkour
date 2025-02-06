using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Vaulting : MonoBehaviour
{
    [Header("Requirements")]
    public PlayerMovement mover;
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsObsticle;

    private RaycastHit obsticleHit;
    public float obsticleCheckDist;

    private RaycastHit aboveObsticleHit;
    public float aboveObsticleCheckDist;

    private RaycastHit obsticleAngledHit;
    public float heightCheckAngle;
    public float heightCheckDist;

    Vector3 heightCheckAxis;
    Quaternion axisRotation;

    Vector3 rotatedDirection;

    public float vaultJumpForce;
    public float vaultForwardForce;

    public float forceDownTimeInSecs;

    int step = 0;

    // Start is called before the first frame update
    void Start()
    {
        //canVault = true;
    }

    // Update is called once per frame
    void Update()
    {

        //print("Too High? " + tooHigh() + "  |  In Front? " + inFront() + "  |  Above? " + aboveObsticle() + "  |  Is Grounded? "+mover.GetGrounded()+"  |  Can Vault? " + canVault());

        if (canVault())
        {
            if (Input.GetKey(KeyCode.Space))
            {
                print("trying to vault");
                //step = 0;
                //print("Step is reset to " + step);
                vault();
            }

        }
        if (step < 3)
        {
            if (step == 0 && aboveObsticle())
            {
                transform.localScale = new Vector3(transform.localScale.x, mover.crouchYScale, transform.localScale.z);
                rb.AddForce(Vector3.down * 3f, ForceMode.Impulse);
                step++;
                print("step " + step);
            }
            else if (step == 1 && !aboveObsticle())
            {
                IEnumerator helper = VaultHelper();
                StartCoroutine(helper);
                step++;
                print("step " + step);
            }
            else if (step == 2)
            {
                step = 0;
                print("Step is reset to " + step);
            }
        }
    }

    private IEnumerator VaultHelper()
    {
        Invoke(nameof(forceDown), forceDownTimeInSecs);
        yield break;
    }

    public void forceDown()
    {
        transform.localScale = new Vector3(transform.localScale.x, mover.GetStartYScale(), transform.localScale.z);
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
    }

    public bool tooHigh()
    {
        heightCheckAxis = -orientation.right;
        axisRotation = Quaternion.AngleAxis(heightCheckAngle, heightCheckAxis);

        rotatedDirection = axisRotation * orientation.forward;

        if (Physics.Raycast(transform.position, rotatedDirection, out obsticleAngledHit, heightCheckDist, whatIsObsticle))
        {
            Debug.DrawRay(transform.position, rotatedDirection * obsticleAngledHit.distance, Color.yellow);
            //canVault = false;
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, rotatedDirection * heightCheckDist, Color.white);
            //canVault = true;
            return false;
        }
        
    }

    public bool inFront()
    {

        if (Physics.Raycast(transform.position, orientation.forward, out obsticleHit, obsticleCheckDist, whatIsObsticle))
        {
            Debug.DrawRay(transform.position, orientation.forward * obsticleHit.distance, Color.green);
            //canVault = false;
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, orientation.forward * obsticleCheckDist, Color.red);
            //canVault = true;
            return false;
        }
    }

    public bool aboveObsticle()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out aboveObsticleHit, aboveObsticleCheckDist, whatIsObsticle))
        {
            Debug.DrawRay(transform.position, Vector3.down * (aboveObsticleHit.distance), Color.cyan);
            //canVault = false;
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * (aboveObsticleCheckDist), Color.magenta);
            //canVault = true;
            return false;
        }
    }

    public bool canVault()
    {
        return (inFront() && !tooHigh() && mover.state != PlayerMovement.MovementState.idle && mover.GetGrounded());
    }

    public void vault()
    {
        mover.state = PlayerMovement.MovementState.vaulting;
        rb.AddForce(Vector3.up * vaultJumpForce, ForceMode.Impulse);
        rb.AddForce(orientation.forward * vaultForwardForce * 2f, ForceMode.Impulse);
        
    }
}
