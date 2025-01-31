using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class DimensionalObj : MonoBehaviour
{

    GameObject[] objects;
    public DimensionNavigation dimNav;
    //string tag = "4D";
    

    // Start is called before the first frame update
    void Start()
    {
        objects = getDescendants(transform.gameObject);
        checkAndSwitchMode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
        //print("In 4D mode? " + dimNav.IsIn4D());
        checkAndSwitchMode();


    }

    public void checkAndSwitchMode()
    {
        if (dimNav.IsIn4D())
        {
            foreach (GameObject obj in objects)
            {
                if (obj.activeInHierarchy == false)
                {
                    obj.SetActive(true);
                }
            }
        }
        else if (!dimNav.IsIn4D())
        {
            foreach (GameObject obj in objects)
            {
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    public GameObject[] getDescendants(GameObject obj)
    {
        //return null if no object is present
        if (obj == null) return null;
        //Get list of all  transforms in the main children object//
        Transform[] childrenT = obj.GetComponentsInChildren<Transform>();
        GameObject[] children = new GameObject[childrenT.Length];

        //Add each object of the transforms to the childrens list
        for (int i = 0; i < children.Length-1; i++)
        {
            
                children[i] = childrenT[i].gameObject;
                //print("" + children[i]);
            
        }

        GameObject[] allButMain = new GameObject[childrenT.Length-1];
        for(int f = 0; f<children.Length-1; f++)
        {
            allButMain[f] = children[f+1].gameObject;
            print("" + allButMain[f]);
        }
        return allButMain;
    }



}
