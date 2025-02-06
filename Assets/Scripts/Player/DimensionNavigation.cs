using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionNavigation : MonoBehaviour
{
    public UIClass ui;
    public PlayerMovement mover;
    public DimensionalObj dim;

    public KeyCode PhaseKey = KeyCode.E;
    public float switchCooldown;
    public bool canSwitchModes;
    public bool in4D;
    public float sphereRadius;
    public float minSpeed;

    public LayerMask whatIsPassThrough;
    public LayerMask whatIs4D;


    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<PlayerMovement>();
        if (IsIn4D())
        {
            ui.Negate();
        }
        //else 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(PhaseKey) && canSwitchModes)
        {
            print("trying to switch modes");
            SwitchMode();
        }
    }

    public void SwitchMode()
    {
        canSwitchModes = false;
        in4D = !in4D;
        ui.Negate();
        print("switched modes");
        Invoke(nameof(ResetSwitchCooldown), switchCooldown);
        
    }

    public void ResetSwitchCooldown()
    {
        canSwitchModes = true;
    }

    public bool IsIn4D()
    {
        return in4D;
    }


}
