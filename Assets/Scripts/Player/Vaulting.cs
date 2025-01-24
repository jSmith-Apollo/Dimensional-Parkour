using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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

    public float vaultForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //print("Obsticle is too high = " + tooHigh());
        //print("In front of an obsticle = " + inFront());
        //print("Above an obsticle = " + aboveObsticle());
        //print("Can Vault = " + canVault());

        if (canVault())
        {
            if (Input.GetKey(KeyCode.Space))
            {
                print("trying to vault");
                vault();
            }
        }
        else if (aboveObsticle())
        {
            transform.localScale = new Vector3(transform.localScale.x, mover.GetStartYScale(), transform.localScale.z);

        }
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
        return (inFront() && tooHigh());
    }

    public void vault()
    {
        rb.AddForce(Vector3.up * vaultForce, ForceMode.Impulse);
        rb.AddForce(orientation.forward * vaultForce * 2f, ForceMode.Impulse);
        print("trying to scale");
        transform.localScale = new Vector3(transform.localScale.x, mover.crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
}
