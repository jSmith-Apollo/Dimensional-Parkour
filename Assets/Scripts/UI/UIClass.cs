using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator fadeUI(GameObject obj, float t,float fps,float val)
    {
        float start = obj.GetComponent<Image>().color.a;
        for (float i = 0; i < t; i += 0.05f)
        {
            //print(lerp(1, 0, i));
            yield return new WaitForSeconds((1/fps)*t);
            obj.GetComponent<Image>().color = new Color(obj.GetComponent<Image>().color.r,
                                                        obj.GetComponent<Image>().color.g,
                                                        obj.GetComponent<Image>().color.b,
                                                        Mathf.Lerp(start, val, i)
                                                       );
        }
    }

    //Get all objects direct children in the hierarcy//
    private GameObject[] getChildren(GameObject obj)
    {
        //return null if no object is present
        if (obj == null) return null;
        //Get list of all  transforms in the main children object//
        Transform[] childrenT = obj.GetComponentsInChildren<Transform>();
        GameObject[] children = new GameObject[childrenT.Length];

        //Add each object of the transforms to the childrens list
        for(int i = 0; i < children.Length; i++)
        {
            children[i] = childrenT[i].gameObject;
        }

        return children;
    }

    //Get all objects Descendants in the hierarcy//
    private GameObject[] getDescendants(GameObject obj)
    {
        //return null if no object is present
        if (obj == null) return null;

        //Get a list for all the Descendants of the object//
        List<GameObject> Descendants = new List<GameObject>();
        //for every Transform saved in the transform add the corresponding object//
        foreach (Transform child in obj.transform)
        {
            Descendants.Add(child.gameObject);
        }
        //return the array of descendants//
        return Descendants.ToArray();
    }

    //Change color of object according to RGB and transparency//
    public void setColor(GameObject obj, float r, float g, float b, float a)
    {
        //return null if no object is present or no image present//
        if (obj == null || obj.GetComponent<Image>() == null)
        {
            return;
        }

        //change the objects color value//
        obj.GetComponent<Image>().color = new Color(r, g, b, a);
    }

    private void visibleState(float state)
    {
        //Get all descendants of the UIClass//
        GameObject[] Descendants = getDescendants(transform.gameObject);
        //For every object 
        foreach (GameObject obj in Descendants)
        {
            setColor(obj, obj.GetComponent<Image>().color.r, obj.GetComponent<Image>().color.g, obj.GetComponent<Image>().color.b, state);
        }
    }
    
    //Change the scale of a deminsion to the a value between 0-1//
    private void scaleByVal(float val, GameObject obj, char Dem)
    {
        //return null if no object is present//
        if (obj == null) return;

        //choose which demension to change the scale
        if (Dem == 'x')
        {
            obj.transform.localScale = new Vector3(val, obj.transform.localScale.y, obj.transform.localScale.z);
        }
        else if (Dem == 'y')
        {
            obj.transform.localScale = new Vector3(obj.transform.localScale.x, val, obj.transform.localScale.z);
        }
        else if (Dem == 'z')
        {
            obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, val);
        }
    }
}
