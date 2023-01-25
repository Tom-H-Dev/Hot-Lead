using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private float maxDistance;

    [SerializeField] private float bulletDMG;
    private GameManager gm;

    private Playerhealth pH;

    private void Start()
    {
        pH = FindObjectOfType<Playerhealth>();
        gm = FindObjectOfType<GameManager>();
        if (bulletDMG == null)
        {
            bulletDMG = 15;
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        maxDistance += 1 * Time.deltaTime;
        if (maxDistance >= 5.0f)
        {
            Destroy(this.gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            pH.kill();

        }
        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
