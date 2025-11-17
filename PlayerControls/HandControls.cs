using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class HandControls : MonoBehaviour
{
    public InputActionReference gripactionreference;
    public InputActionReference triggeractionreference;

    [SerializeField] private Animator Hand;
    private float triggerval;
    private float gripval;
    void Start()
    {
        
    }

    void AnimateTrigger()
    {
        triggerval = triggeractionreference.action.ReadValue<float>();
        Hand.SetFloat("Trigger", triggerval);
    }

    void AnimateGrip()
    {
        gripval = gripactionreference.action.ReadValue<float>();
        Hand.SetFloat("Grip", gripval);
    }
    void Update()
    {
        AnimateTrigger();
        AnimateGrip();
    }
}
