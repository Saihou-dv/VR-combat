using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SlowDownTime : MonoBehaviour
{
    private int count = 0;
    public InputActionProperty AbilityButton;
    private bool isSlowed = false;
    private float originalFixedDeltaTime;

    void Start()
    {
        originalFixedDeltaTime = Time.fixedDeltaTime;
        AbilityButton.action.performed += ctx => ToggleTime();
        AbilityButton.action.Enable();
    }   

    void ToggleTime()
    {
        isSlowed = !isSlowed;

        if (isSlowed)
        {
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }
    }

    private void OnDestroy()
    {
        AbilityButton.action.performed -= ctx => ToggleTime();
    }



}
