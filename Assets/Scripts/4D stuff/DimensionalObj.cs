using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class DimensionalObj : MonoBehaviour
{

    public DimensionNavigation dimNav;
    //string tag = "4D";
    

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(dimNav.IsIn4D());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        print("In 4D mode? " + dimNav.IsIn4D());
        if (dimNav.IsIn4D() && gameObject)
        {
            gameObject.gameObject.SetActive(true);
            print("Set 4D objects Active");
        }
        else if (!dimNav.IsIn4D() && gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            print("Set 4D objects Inactive");
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
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = childrenT[i].gameObject;
        }

        return children;
    }



}
