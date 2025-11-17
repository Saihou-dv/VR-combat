using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCreator : MonoBehaviour
{
    EnemyHP EnemyHP;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
        
        if (collision.collider.GetComponent<RootMotion.Dynamics.CustomPuppetBehaviour>())
        {
            Debug.Log("found");
            float impactforce = collision.relativeVelocity.magnitude;
            Debug.Log(impactforce);
            EnemyHP = collision.collider.GetComponent<EnemyHP>();
            EnemyHP.TakeDamage(impactforce);
        }
    }

   
}
