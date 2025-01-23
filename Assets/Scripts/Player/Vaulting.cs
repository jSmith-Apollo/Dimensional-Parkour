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
    public float heightCheckAngle;
    public float heightCheckDist;

    public bool canVault;

    Vector3 heightCheckAxis;
    Quaternion axisRotation;

    Vector3 rotatedDirection;


    // Start is called before the first frame update
    void Start()
    {
        canVault = true;
    }

    // Update is called once per frame
    void Update()
    {
        print("Obsticle is too high = " + tooHigh());
    }

    public bool tooHigh()
    {
        heightCheckAxis = -orientation.right;
        axisRotation = Quaternion.AngleAxis(heightCheckAngle, heightCheckAxis);

        rotatedDirection = axisRotation * orientation.forward;

        Debug.DrawRay(transform.position, rotatedDirection * obsticleHit.distance, Color.magenta);

        if (Physics.Raycast(transform.position, rotatedDirection, out obsticleHit, heightCheckDist, whatIsObsticle))
        {
            canVault = false;
            return true;
        }
        canVault = true;
        return false;
    }

    //public void vault()
    //{
    //    if (canVault)
    //    {
    //        if(mover.Ge)
    //    }
    //}
}
