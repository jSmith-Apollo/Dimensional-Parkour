using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIInterface : UIClass
{

    private GameObject SpeedBackground;
    private GameObject SpeedBar;
    private GameObject HealthBackground;
    private GameObject HealthBar;

    private bool SpeedFadeActive = false;
    private bool HealthFadeActive = false;

    // Start is called before the first frame update
    void Start()
    {
        SpeedBackground = GameObject.Find("VelocityBarBackground");
        SpeedBar = GameObject.Find("VelocityBar");
        HealthBackground = GameObject.Find("HealthBarBackground");
        HealthBar = GameObject.Find("HealthBar");
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpeedUI();
        UpdateHealthUI();
    }
    

    public void UpdateSpeedUI()
    {
        //Scale the speed UI to match the speed value of the player//
        scaleByVal(
            //Get the decimal percentage current speed is at//
            GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMoveSpeed() / GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMaxSpeed(),
            SpeedBar, //--Object being affected
            'x' //--Dimension being scaled
            );
        //--------------------------------------------------------//

        //Change the Bar to match the transparency of the Background//
        setColor(SpeedBar,//--object
                SpeedBar.GetComponent<UnityEngine.UI.Image>().color.r, //-- Object red value
                SpeedBar.GetComponent<UnityEngine.UI.Image>().color.g, //-- Object green value
                SpeedBar.GetComponent<UnityEngine.UI.Image>().color.b, //-- Object blue value
                SpeedBackground.GetComponent<UnityEngine.UI.Image>().color.a //-- Background transparency value
                );
        //---------------------------------------------------------//

        // Determine if the UI should be faded out due to lack of use //
        if (SpeedBar.GetComponent<UnityEngine.UI.Image>().color.a == 1 || SpeedBar.GetComponent<UnityEngine.UI.Image>().color.a == 0)
        {
            IEnumerator FadeCheck = FadeSpeedBar(GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMoveSpeed());
            StartCoroutine(FadeCheck);
            
        }
       
    }
    private IEnumerator FadeSpeedBar(float speed)
    {
        SpeedFadeActive = true;
        yield return new WaitForSeconds(1f);
        print("run");
        IEnumerator FadeIn = fadeUI(SpeedBackground, 1, 30, 1f);
        IEnumerator FadeOut = fadeUI(SpeedBackground, 1, 30, 0f);
        //Determine if the player speed is equal to the prveious speed and not at max speed
        if (speed != GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMoveSpeed() && speed != GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMaxSpeed())
        {
            if (SpeedBackground.GetComponent<UnityEngine.UI.Image>().color.a == 0)
            {
                StopCoroutine(FadeOut);
                print("Fadein");
                StartCoroutine(FadeIn);
            }
            else if (SpeedBar.GetComponent<UnityEngine.UI.Image>().color.a == 1 && speed < GameObject.Find("PlayerV2").GetComponent<PlayerMovement>().GetMoveSpeed())
            {
                StopCoroutine(FadeIn);
                print("Fadeout");
                StartCoroutine(FadeOut);
            }
        }
        SpeedFadeActive = false;
    }
    public void UpdateHealthUI()
    {
        //Scale the health UI to the health value of the player//
        //Add in when player is added//
        //----------------------------------------------------//

        //Change the Bar to match the transparency of the Background//
        setColor(HealthBar,//--object
                HealthBar.GetComponent<UnityEngine.UI.Image>().color.r, //-- Object red value
                HealthBar.GetComponent<UnityEngine.UI.Image>().color.g, //-- Object green value
                HealthBar.GetComponent<UnityEngine.UI.Image>().color.b, //-- Object blue value
                HealthBackground.GetComponent<UnityEngine.UI.Image>().color.a //-- Background transparency value
                );
        //---------------------------------------------------------//

        // Determine if the UI should be faded out due to lack of use //
    }
}
