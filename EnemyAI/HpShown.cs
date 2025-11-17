using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class HpShown : MonoBehaviour
{
    public Image Health;
    EnemyHP enemyHP;
    public float StartingHP;
    void Start()
    {
        Health = GetComponent<Image>();
        enemyHP = GetComponentInParent<EnemyHP>();
        StartingHP = enemyHP.Enemy_HP;
    }

    // Update is called once per frame
    void Update()
    {
        Health.fillAmount = enemyHP.Enemy_HP/StartingHP ;
       
    }
}
