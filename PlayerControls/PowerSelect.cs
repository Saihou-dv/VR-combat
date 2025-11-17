using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PowerSelect : MonoBehaviour
{
    public InputActionProperty flyToggle;
    public static bool flyOn_Off = false;
    public int count = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        if (flyToggle.action.WasPressedThisFrame())
        {
            count += 1;
       
        }
        if(count == 2)
        {
            count = 0;
        }

        CheckFly();
        
    }

    public void CheckFly()
    {
        if(count == 1)
        {
            flyOn_Off = true;
        }
        else
        {
            flyOn_Off = false;
        }
    }
}
