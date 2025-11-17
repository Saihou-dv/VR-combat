using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GrabPhysics : MonoBehaviour
{
    public InputActionProperty grabinput;
    public float radius = 0.1f;
    public LayerMask grabLayer;
    public bool isGrabPressed = false;
    public bool isGrabbing = false;
    public bool CantGrab;
    public static string tagheld;
    //public static bool HandsClosed;
    //public bool confirmedclose;
    private FixedJoint fixedJoint;
    [Range(0.0f, 1.0f)]
    public float jointstrength;

   
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrabPressed = grabinput.action.ReadValue<float>() > 0.1f;



        if (isGrabPressed && !isGrabbing)
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, radius, grabLayer, QueryTriggerInteraction.Ignore);

            if(nearbyColliders.Length == 0)
            {
                
                CantGrab = true;
            }


            if (!CantGrab)
            {
                if (nearbyColliders.Length > 0)
                {
                    Rigidbody rbcollider = nearbyColliders[0].attachedRigidbody;

                    fixedJoint = gameObject.AddComponent<FixedJoint>();
                    fixedJoint.autoConfigureConnectedAnchor = false;
                    if (rbcollider)
                    {
                        fixedJoint.connectedBody = rbcollider;
                        fixedJoint.massScale = jointstrength;
                        fixedJoint.connectedAnchor = rbcollider.transform.InverseTransformPoint(transform.position);
                        tagheld = rbcollider.tag;
                        
                    }
                    else
                    {
                        fixedJoint.connectedAnchor = transform.position;
                    }
                    isGrabbing = true;
                }
            }
            
        }
        else if(!isGrabPressed && isGrabbing)
        {
            
            isGrabbing = false;
            if (fixedJoint)
            {
                Destroy(fixedJoint);
                tagheld = "";
            }
        }
        else { CantGrab = false;  }

        if (tagheld.Equals("Enemyrb"))
        {
            FallHolder fall = fixedJoint.GetComponent<FallHolder>();
            fall.Events.Invoke();
        }
    }
}
