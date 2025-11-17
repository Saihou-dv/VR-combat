using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Controls")]
    [SerializeField] private float speed;
    [SerializeField] private float sprintspeed;
    public InputActionProperty moveInputSource;
    [SerializeField] private InputActionProperty jump;
    [SerializeField] private InputActionProperty sprint; 
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float JumpStrength = 20f;
    [Header("Transforms")]
    public Transform DirectionSource;
    private Vector2 moveinputaxis;
    [SerializeField] private Transform GroundCheck;

    [Header("Layers")]
    [SerializeField] private LayerMask floor;

    [Header("Boolean")]
    [SerializeField] private bool Grounded;

    [Header("Flying Attributes")]
    public Transform Head;
    [SerializeField] private Transform Hands;
    public Text Tester;
    public GameObject Player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveinputaxis = moveInputSource.action.ReadValue<Vector2>();


        Grounded=Physics.CheckSphere(GroundCheck.position, 0.4f, floor);
       
        if (Grounded)
        {
            Jump();
        }
    }


    void Jump()
    {
        if (jump.action.WasPressedThisFrame())
        {
            rb.AddForce(Vector3.up * JumpStrength, ForceMode.Impulse);
        }
        
    }
    private void FixedUpdate()
    {
        Quaternion yaw = Quaternion.Euler(0, DirectionSource.eulerAngles.y, 0);
        Vector3 direction = yaw * new Vector3(moveinputaxis.x, 0, moveinputaxis.y);
        if(sprint.action.ReadValue<float>()> 0.1f)
        {
            rb.MovePosition(rb.position + direction * Time.fixedDeltaTime * sprintspeed);
        }
        else
        {
            rb.MovePosition(rb.position + direction * Time.fixedDeltaTime * speed);
        }
        
    }
}
