using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [Header("Health")]
    public float health;
    private float maxhealth;
    private bool canRegen;
    private float regenDelay;
    private float regenAmount;

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
    public bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("GroundCheck")]
    public float height;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform Orientation;
    public Rigidbody rb;

    public bool canAct;
    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

}
