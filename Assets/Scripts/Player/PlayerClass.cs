using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    [Header("Stats")]
    public float health;
        private float maxhealth;
        private bool Regen = true; 

    public float MaxVelocity;
        private float Velocity;
        private float acceleration;
    public float JumpHeight;
    public float PlayerHeight;

    [Header("keyBinds")]
    public KeyCode Jump = KeyCode.Space;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode crouch = KeyCode.LeftControl;
    public KeyCode special1 = KeyCode.Alpha0;
    public KeyCode special2 = KeyCode.Alpha1;
    public KeyCode special3 = KeyCode.Alpha2;
    public KeyCode restart = KeyCode.R;
    public KeyCode respawn = KeyCode.Space;

    private bool CanAct;
    void Start()
    {
        maxhealth = health;
        CanAct = true;
    }

    void Update()
    {
        //Test Healthfunction//
        if (Input.GetKeyDown(KeyCode.Z))
        {
            print("dmgTaken");
            TakeHealth(maxhealth/5);
        }
        //Respawn player//
        if (health <= 0)
        {
            Respawn();
        }
        if (health != maxhealth && Regen) {
            health += 0.01f;
        }
    }

    public void TakeHealth(float amount)
    {
        health -= amount;
        StartCoroutine("regentimer");
    }

    private IEnumerator regentimer()
    {
        Regen = false;
        yield return new WaitForSeconds(5f);
        Regen = true;
    }

    public void Respawn()
    {
        CanAct = false;
        print("Super Dead X_X");
    }

    public float getHealth()
    {
        return health;
    }
}
