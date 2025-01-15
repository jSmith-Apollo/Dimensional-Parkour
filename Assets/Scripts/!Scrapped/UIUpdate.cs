using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdate : MonoBehaviour
{
    private static GameObject VelocitybarBg;
    private static GameObject VelocityBar;
    private static GameObject HealthbarBg;
    private static GameObject HealthBar;
    private bool FadeOutRun;
    private bool FadeInRun;
    // Start is called before the first frame update

    private float lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
    void Start()
    {
        VelocitybarBg = GameObject.Find("VelocityBarBackground");
        VelocityBar = GameObject.Find("VelocityBar");
        HealthbarBg = GameObject.Find("HealthBarBackground");
        HealthBar = GameObject.Find("HealthBar");
        
    }

    private void Awake()
    {

    }

    private IEnumerator FadeOutUI(GameObject obj)
    {
        for (float i = 0; i < 1.05; i += 0.05f)
        {
            //print(lerp(1, 0, i));
            yield return new WaitForSeconds(0.01f);
            obj.GetComponent<Image>().color = new Color(obj.GetComponent<Image>().color.r,
                                                        obj.GetComponent<Image>().color.g,
                                                        obj.GetComponent<Image>().color.b,
                                                        lerp(1, 0, i)
                                                       );
        }
    }
    private IEnumerator FadeInUI(GameObject obj)
    {
        for (float i = 0; i < 1.05; i += 0.05f)
        {
            yield return new WaitForSeconds(0.01f);
            obj.GetComponent<Image>().color = new Color(obj.GetComponent<Image>().color.r,
                                                        obj.GetComponent<Image>().color.g,
                                                        obj.GetComponent<Image>().color.b,
                                                        lerp(0, 1, i)
                                                       );
        }
    }

    private IEnumerator FadeOutUIVelocity(GameObject obj)
    {
        FadeOutRun = true;
        //Check if the player is not running for more than a second//
        yield return new WaitForSeconds(0.5f);
        if (GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMoveSpeed() > 1){
            FadeOutRun = false;
            yield break; 
        }

        //Fade out UI//
        for (float i = 0; i < 1.05; i += 0.05f)
        {
            //print(lerp(1, 0, i));
            yield return new WaitForSeconds(0.01f);
            obj.GetComponent<Image>().color = new Color(obj.GetComponent<Image>().color.r,
                                                        obj.GetComponent<Image>().color.g,
                                                        obj.GetComponent<Image>().color.b,
                                                        lerp(1, 0, i)
                                                       ) ;
        }
        FadeOutRun = false;
    }
    private IEnumerator FadeInUIVelocity(GameObject obj)
    {
        FadeInRun = true;
        yield return new WaitForSeconds(0.5f);
        //Check if the player is  running for more than a second//
        if (GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMoveSpeed() <= 1) {
            FadeInRun= false; 
            yield break; 
        }
        
        //Fade in UI//
        for (float i = 0; i < 1.05; i += 0.05f)
        {
            yield return new WaitForSeconds(0.01f);
            obj.GetComponent<Image>().color = new Color(obj.GetComponent<Image>().color.r,
                                                        obj.GetComponent<Image>().color.g,
                                                        obj.GetComponent<Image>().color.b,
                                                        lerp(0, 1, i)
                                                       );
        }
        FadeInRun = false;
    }
    // Update is called once per frame
    void Update()
    {
        // Velocity Bar FadeIn and Out // 
        IEnumerator Out = FadeOutUIVelocity(VelocitybarBg);
        IEnumerator In = FadeInUIVelocity(VelocitybarBg);
        float movespeed = GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMoveSpeed();
        if (movespeed <= 1 && VelocitybarBg.GetComponent<Image>().color.a >= 0.2 && !FadeOutRun) 
        {
            StopCoroutine(In);
            print("Out");
            StartCoroutine(Out);
        }
        if (movespeed > 1 && VelocitybarBg.GetComponent<Image>().color.a <= 0.2 && !FadeInRun)
        {
            StopCoroutine(Out);
            print("In");
            StartCoroutine(In);
        }
        //--------------------------//
        // HealthBar FadeIn and out //
        if (GameObject.Find("PlayerV2").GetComponent<PlayerClass>().GetHealth() < 100 && HealthbarBg.GetComponent<Image>().color.a <= 0.9)
        {
            StartCoroutine(FadeInUI(HealthbarBg));
        }
        else if (GameObject.Find("PlayerV2").GetComponent<PlayerClass>().GetHealth() >= 100 && HealthbarBg.GetComponent<Image>().color.a >= 0.9)
        {
            StartCoroutine(FadeOutUI(HealthbarBg));
        }
        //---------------------------//
        //Update Velocity and Health//
        VelocityBar.transform.localScale = new Vector3(movespeed / GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMaxSpeed(), VelocityBar.transform.localScale.y, VelocityBar.transform.localScale.z);
        HealthBar.transform.localScale = new Vector3(GameObject.Find("PlayerV2").GetComponent<PlayerClass>().GetHealth() / 100, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
        if (VelocitybarBg.GetComponent<Image>().color.a != VelocityBar.GetComponent<Image>().color.a)
        {
            VelocityBar.GetComponent<Image>().color = new Color(VelocityBar.GetComponent<Image>().color.r,
                                                                VelocityBar.GetComponent<Image>().color.b,
                                                                 VelocityBar.GetComponent<Image>().color.g,
                                                                  VelocitybarBg.GetComponent<Image>().color.a
                                                                 );
        }
        if (HealthbarBg.GetComponent<Image>().color.a != HealthBar.GetComponent<Image>().color.a)
        {
        HealthBar.GetComponent<Image>().color = new Color(HealthBar.GetComponent<Image>().color.r,
                                                            HealthBar.GetComponent<Image>().color.b,
                                                             HealthBar.GetComponent<Image>().color.g,
                                                              HealthbarBg.GetComponent<Image>().color.a
                                                             );
        } 
    }


}
