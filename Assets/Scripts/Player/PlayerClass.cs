using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : Actor
{
    //Player Information//
    private float points;
    private List<GameObject> inventory = new List<GameObject>();
    private GameObject[] Equipped = new GameObject[3];

    //Keybinds//
    [Header("keyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode special1Key = KeyCode.Alpha0;
    public KeyCode special2Key = KeyCode.Alpha1;
    public KeyCode special3Key = KeyCode.Alpha2;
    public KeyCode dimensionKey = KeyCode.F;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode DenyKey = KeyCode.X;
    public KeyCode MenuKey = KeyCode.M;

    //PlayerInputs//
    float HorizontalInput;
    float VerticalInput;

    void Start()
    {

    }

    void Update()
    {
       
    }

    public void MyInput()
    {

    }

    public void CheckKeys()
    {

    }
    
    public void Interact()
    {

    }

    public void OpenMenu()
    {

    }

    public void CloseMenu()
    {

    }

    public void ChangeEquip(GameObject obj,int pos)
    {

    }

    public float GetPoints()
    {
        return points;
    }

    public void setPoints(float p)
    {
        points = p;
    }
    public void ChangeDimension(int d)
    {

    }

}
