using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class FlightControl : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    public Transform Head;
    

    public bool Flying = false;
    public Rigidbody Body;
    public ForceMode ChangeForce;
    public float FlyingStrength = 20f;
    public float SlowDownVel = 0.5f;
    private float ogVel;
    public Text SpeedText;
    public Text Lefttext;
    public Text Righttext;
    [Header("Hands")]
    public InputActionProperty lefthandinput;
    public InputActionProperty righthandinput;
    public Transform RightHand;
    public Transform LeftHand;
    public GameObject PhysicHandR;
    public GameObject PhysicHandL;
    [Range(0, 0.75f)]
    public float ArmLength;
    [Header("Effects")]
    public ParticleSystem speedline;
    public ParticleSystem wind;
    public AudioSource flying;
    void Start()
    {
        ogVel = SlowDownVel;
    }

    // Update is called once per frame
    void Update()
    {
        Fly();
        SpeedText.text = Body.velocity.magnitude.ToString();
        //   Lefttext.text = Vector3.Distance(Head.position, RightHand.position).ToString();
        // Righttext.text = Vector3.Distance(Head.position, LeftHand.position).ToString();
        if (Body.velocity.magnitude > 45f)
        {
            speedline.Play();
            wind.Play();
        }
        else
        {
            wind.Stop();

            speedline.Stop();
        }

        if (Body.velocity.magnitude < 10)
            flying.volume = Mathf.Lerp(flying.volume, 0, Time.deltaTime * 10);

        void Fly()
        {
            Canfly();
            float HandDistance = Vector3.Distance(Head.position, RightHand.position);
            float HandDistance1 = Vector3.Distance(Head.position, LeftHand.position);

            if (Flying)
            {
                Body.drag = Mathf.Lerp(Body.drag, 10, Time.deltaTime * SlowDownVel);

                if (Body.velocity.magnitude > 80f)
                {
                    flying.volume = Mathf.Lerp(flying.volume, 1, Time.deltaTime * 5);
                    SlowDownVel = .2f;
                }
                else if (Body.velocity.magnitude > 10)
                {
                    flying.volume = Mathf.Lerp(flying.volume, .3f, Time.deltaTime * 5);

                }
                else
                {
                    flying.volume = Mathf.Lerp(flying.volume, 0, Time.deltaTime * 10);
                    SlowDownVel = ogVel;
                }


                if (lefthandinput.action.ReadValue<float>() > 0.1)
                {
                    if (HandDistance1 > ArmLength)
                    {
                        Body.drag = Mathf.Lerp(Body.drag, SlowDownVel, Time.deltaTime * SlowDownVel);
                        Body.AddForce(PhysicHandL.transform.TransformDirection(Vector3.forward) * FlyingStrength, ChangeForce);
                    }
                }
                if (righthandinput.action.ReadValue<float>() > 0.1)
                {
                    if (HandDistance > ArmLength)
                    {
                        Body.drag = Mathf.Lerp(Body.drag, SlowDownVel, Time.deltaTime * SlowDownVel);
                        Body.AddForce(PhysicHandR.transform.TransformDirection(Vector3.forward) * FlyingStrength, ChangeForce);
                    }
                }



            }
            else
            {
                Body.drag = 0;
            }
        }
    }

    private bool CheckForFixedJoint(GameObject hand)
    {
        if (hand.GetComponent<FixedJoint>())
        {
            return true;
        }
        return false;
    }
    void Canfly()
    {
        Rigidbody[] g = Player.GetComponentsInChildren<Rigidbody>();
        if (PowerSelect.flyOn_Off)
        {
            Flying = true;
            foreach (Rigidbody r in g)
            {

                r.useGravity = false;
            }
        }
        else
        {
            Flying = false;
            foreach (Rigidbody r in g)
            {
                r.useGravity = true;
            }
        }
    }
}
