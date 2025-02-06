using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.VersionControl;
using UnityEngine;

public class DimensionalObj : MonoBehaviour
{

    GameObject[] objects;
    public DimensionNavigation dimNav;
    string tag4D = "4D";
    string tagBoth = "Both";
    string tag3D = "3D";

    public LayerMask whatIsWall;
    public LayerMask whatIsPassThrough;
    public LayerMask whatIsWallAndPT;
    

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
        //foreach (GameObject obj in objects)
        //{
        //    if (dimNav.IsIn4D())
        //    {
        //        if (obj.tag.Equals("3D"))
        //        {
        //            if (!obj.activeInHierarchy)
        //            {
        //                obj.SetActive(true);
        //            }
        //        }
        //        else if (obj.tag.Equals("3D"))
        //        {
        //            if (obj.layer.Equals("whatIsWallAndPT"))
        //            {
        //                int LayerPassthrough = LayerMask.NameToLayer("whatIsPassthrough");
        //                obj.layer = LayerPassthrough;
        //            }
        //        }
        //        else if (obj.tag.Equals("Both"))
        //        {
        //            //do nothing
        //            break;
        //        }
        //    }
        //}
        //
        //foreach (GameObject obj in objects)
        //{
        //    if (!dimNav.IsIn4D())
        //    {
        //        if (obj.tag.Equals("4D"))
        //        {
        //            if (obj.activeInHierarchy)
        //            {
        //                obj.SetActive(false);
        //            }
        //        }
        //        else if (obj.tag.Equals("3D"))
        //        {
        //            if (obj.layer.Equals("whatIsPassthrough"))
        //            {
        //                int LayerPassthrough = LayerMask.NameToLayer("whatIsWallAndPT");
        //                obj.layer = LayerPassthrough;
        //            }
        //        }
        //        else if (obj.tag.Equals("Both"))
        //        {
        //            //do nothing
        //            break;
        //        }
        //    }
        //}

        

        foreach (GameObject obj in objects)
        {
            //GameObject[] NotPT = GameObject.FindGameObjectsWithTag("whatIsWallAndPT");

            //GameObject[] IsPT = GameObject.FindGameObjectsWithTag("whatIsPassthrough");

            if (obj.tag.Equals(tag4D))
            {
                if (dimNav.IsIn4D())
                {
                    if (!obj.activeInHierarchy)
                        obj.SetActive(true);
                }
                else
                {
                    if (obj.activeInHierarchy)
                        obj.SetActive(false);
                }
            }

            else if (obj.tag.Equals(tag3D)){
                if (dimNav.IsIn4D())
                {
                    //if (obj.layer.Equals("whatIsWallAndPT"))
                    //{
                    //    int LayerPassthrough = LayerMask.NameToLayer("whatIsPassthrough");
                    //    obj.layer = LayerPassthrough;
                    //}
                    //if (obj.layer.Equals("whatIsWallAndPT"))
                    if(obj.layer == LayerMask.NameToLayer("whatIsWallAndPT"))
                    {
                        obj.layer = LayerMask.NameToLayer("whatIsPassthrough");
                        print("Object: " + obj.name + "  |  Layer was:  'whatIsWallAndPT'  |  Layer is now:  '" + obj.layer + "'");
                    }
                        

                }
                else
                {
                    //if (obj.layer.Equals("whatIsPassthrough"))
                    //{
                    //    int LayerPassthrough = LayerMask.NameToLayer("whatIsWallAndPT");
                    //    obj.layer = LayerPassthrough;
                    //}
                    //if (obj.layer.Equals("whatIsPassthrough"))
                    if (obj.layer == LayerMask.NameToLayer("whatIsPassthrough"))
                    {
                        obj.layer = LayerMask.NameToLayer("whatIsWallAndPT");
                        print("Object: " + obj.name + "  |  Layer was:  'whatIsPassthrough'  |  Layer is now:  '" + obj.layer + "'");
                    }
                        
                    
                }
            }

            else if (obj.tag.Equals(tagBoth))
            {
                //do nothing
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
        for (int i = 0; i < children.Length; i++)
        {
            
                children[i] = childrenT[i].gameObject;
                //print("" + children[i]);
            
        }

        GameObject[] allButMain = new GameObject[childrenT.Length - 1];
        for (int f = 0; f < children.Length - 1; f++)
        {
            allButMain[f] = children[f + 1].gameObject;
            print("" + allButMain[f] + "  |  tag: " + allButMain[f].tag + "  |  layer:  " + allButMain[f].layer);
        }
        return allButMain;

        //return children;
    }



}
