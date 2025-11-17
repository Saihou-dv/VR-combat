using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.Dynamics;
public class NavMesh_Enemy : MonoBehaviour
{
    public static List<Vector3> ReservedSpots = new List<Vector3>();

    public NavMeshAgent agent;
    public Transform Target;
    public Transform Player;
    public CustomPuppetBehaviour puppet;
    public Animator animator;
    float distancetoTarget;
    [Header("Attacking")]
    [Range(2, 7)]
    public float fighting_Range;
    public bool IsAttacking;
    [Header("Positions")]
    Vector3 Infront;
    enum Flankside { Left,Right,Behind,None};
    Flankside chosenflank = Flankside.None;
    public Vector3 behind;
    public Vector3 leftflank;
    public Vector3 rightflank;
    public Vector3 chosenspot;
    bool haschosenspot = false;
    float destinationupdatetimer = 0f;
    float destinationupdateinterval = 2;
    Vector3 playerprevpos;
    void Start()
    {
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(chosenspot, 0.2f);
    }

    void UpdateChosenFlankside()
    {
        destinationupdatetimer += Time.deltaTime;
        if (destinationupdatetimer >= destinationupdateinterval)
        {
            Vector3 targetpos = transform.position;
            switch (chosenflank)
            {
                case Flankside.Behind:
                    targetpos = Player.position - Player.forward * 2;
                    break;
                case Flankside.Right:
                    targetpos = Player.position + Player.right * 2;
                    break;
                case Flankside.Left:
                    targetpos = Player.position - Player.right * 2;
                    break;
            }

            if (!IsSpotReserved(targetpos))
            {
                chosenspot = targetpos;
                FaceTarget();
                agent.SetDestination(chosenspot);
            }
            destinationupdatetimer = 0;
        }
        
    }
    void GetPlayerFlankPositions()
    {
        leftflank = Player.position - Player.right * 2;
        rightflank = Player.position + Player.right * 2;
        behind = Player.position - Player.forward* 2;
    }

    bool IsSpotReserved(Vector3 point)
    {
        foreach (Vector3 reserved in ReservedSpots)
        {
            if (Vector3.Distance(point, reserved) < 0.5f)  // Small threshold
                return true;
        }
        return false;
    }
    void ClosetPoint()
    {
        Vector3[] points = { leftflank, rightflank, behind };
        Flankside[] sides = { Flankside.Left, Flankside.Right, Flankside.Behind };
        float distance;
        float closestdist = Mathf.Infinity;
        Vector3 bestspot = transform.position;

        Flankside bestSide = Flankside.None;

        for (int i = 0; i < points.Length; i++)
        {
            if (IsSpotReserved(points[i]))
                continue;

            float dist = Vector3.Distance(points[i], transform.position);
            if (dist < closestdist)
            {
                closestdist = dist;
                bestspot = points[i];
                bestSide = sides[i];
            }
        }
        if (bestSide != Flankside.None)
        {
            ReservedSpots.Add(bestspot);
            chosenspot = bestspot;
            chosenflank = bestSide;
            agent.isStopped = false;
            agent.SetDestination(chosenspot);
        }
        playerprevpos = Player.position;
        

    }
    bool InfightingRange()
    {
        return distancetoTarget <= fighting_Range;
    }
    void ResetTrigger()
    {
        IsAttacking = false;
        animator.ResetTrigger("Punch");
    }

    void FaceTarget()
    {
        Vector3 direction = (Player.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookrotation = Quaternion.LookRotation(direction);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookrotation, Time.deltaTime*3);
    }
    void Update()
    {
        Debug.Log(agent.destination);
    
        distancetoTarget = Vector3.Distance(agent.transform.position, Target.position);
        agent.enabled = puppet.puppetMaster.state == PuppetMaster.State.Alive;

        if (agent.enabled)
        {
            GetPlayerFlankPositions();
            if (InfightingRange())
            {
                FaceTarget();
                if (!haschosenspot)
                {
                    ClosetPoint();
                    haschosenspot = true;
                }
                else
                {
                    
                     UpdateChosenFlankside();
                    
                }
                
                AttackPlayer();
            }
            else
            {
                ChasePlayer();
                haschosenspot = false;
                ResetTrigger();
            }
            animator.SetFloat("Forward", agent.velocity.magnitude * 0.25f);
            //Debug.Log(agent.velocity.x);
        }

    }

    void ChasePlayer()
    {
        
        if (distancetoTarget > fighting_Range)
        {
            agent.isStopped = false;
            agent.SetDestination(Target.position);
        }
        else
        {
            agent.isStopped = true;
        }

        
    }

    void AttackPlayer()
    {
        IsAttacking = true;
        animator.SetTrigger("Punch");
    }
}
