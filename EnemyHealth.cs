using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHealth : MonoBehaviour
{
    public float enemyMaxHealth;
    public float enemyCurrentHealth;
    private EnemyBehavior eb;

    private void Start()
    {
        eb = GetComponent<EnemyBehavior>();
        enemyCurrentHealth = enemyMaxHealth;
    }

    private void Update()
    {
        if (enemyCurrentHealth <= 0)
        {
            kill();
        }

        //Debug.Log(this.name + " has " + enemyCurrentHealth);
    }

    private void OnDisable()
    {
        eb.enemyAnimator.SetTrigger("enemyDeath");
    }



    public void kill()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
