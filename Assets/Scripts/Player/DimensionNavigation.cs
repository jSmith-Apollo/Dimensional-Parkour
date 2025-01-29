using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionNavigation : MonoBehaviour
{

    public PlayerMovement mover;
    public DimensionalObj dim;

    public KeyCode PhaseKey = KeyCode.E;
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

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(PhaseKey) && canSwitchModes)
        {
            print("trying to switch modes");
            Invoke(nameof(modeSwitcher), 0f);
        }
    }

    public void SwitchMode()
    {
        
    }

    public IEnumerator modeSwitcher()
    {
        canSwitchModes = false;
        yield return new WaitForSeconds(0.5f);
        in4D = !in4D;
        print("switched modes");
        canSwitchModes = true;

    }

}
