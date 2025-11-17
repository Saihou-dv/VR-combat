using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRange : MonoBehaviour
{
    public GameObject Player;
    public Animator enemy;
    void Start()
    {
   
    }

    
    public bool InRange()
    {
       if(Vector3.Distance(transform.position, Player.transform.position) < 2)
        {
            return true;
        }
        return false;
    }
   
    void Update()
    {
        if (InRange())
        {
            enemy.SetBool("InCombat",true);
            enemy.SetBool("Idle", false);
        }
        else
        {
            enemy.SetBool("InCombat",false);
            enemy.SetBool("Idle",true);
        }
    }
}
