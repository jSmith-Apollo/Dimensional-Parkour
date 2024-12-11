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

    private bool speedIdle = false;
    private bool HealthIdle = false;
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
