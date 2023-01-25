using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float tragetDistance;
    [SerializeField] private bool moveRandom;
    private bool allowedToShoot = false;

    public EnemyStates eSt;

    private NavMeshAgent agent;

    public GameObject player;
    [SerializeField] private GameObject bulletPrefab;

    private int currentIndex;
    public Animator enemyAnimator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNextTarget();
        eSt = EnemyStates.Patroling;
    }

    public void AttackEnemy()
    {
        eSt = EnemyStates.Attacking;
    }

    void Update()
    {
        //TO DO:
        //make a float where there is a wait timer at the waypoint and then go to the next waypoint

        if (agent.remainingDistance <= tragetDistance)
        {
            SetNextTarget();
        }
        if (eSt == EnemyStates.Attacking)
        {
            CheckIFPlayerIsTarget();
        }

        //print(this.name + " " + eSt);

        if (currentWait == 0 && allowedToShoot)
        {
            ShootAtPlayer();
        }

        if (shot && currentWait <= shootTime)
        {
            currentWait += 1 * Time.deltaTime;
        }

        if (currentWait >= shootTime)
        {
            currentWait = 0;
        }
    }

    void SetNextTarget()
    {
        int prevIndex = currentIndex;

        //if the enemies are not alerted or attacking the player will be the target of the follow
        if (eSt != EnemyStates.Attacking)
        {
            if (moveRandom)
            {
                currentIndex = Random.Range(0, waypoints.Count);
                if (currentIndex == prevIndex)
                {
                    currentIndex++;
                }
            }
            else
            {
                currentIndex++;
            }
            currentIndex %= waypoints.Count;

            agent.SetDestination(waypoints[currentIndex].position);
        }
        if (eSt == EnemyStates.Attacking)
        {
            CheckIFPlayerIsTarget();
        }
    }

    private void CheckIFPlayerIsTarget()
    {
        float _dist = Vector3.Distance(transform.position, player.transform.position);

        //if the enemy state is attaing the enemy will start  following the player
        if (eSt == EnemyStates.Attacking)
        {
            if (_dist <= 2)
            {
                StopEnemy();
                this.transform.LookAt(player.transform);
            }
            else
            {
                FollowPlayerWhenAttacking();
            }
        }
    }

    public void FollowPlayerWhenAttacking()
    {
        enemyAnimator.SetBool("enemyWalk", true);
        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
        allowedToShoot = false;
    }

    private void StopEnemy()
    {
        agent.isStopped = true;
        allowedToShoot = true;
    }

    public GameObject enemyShootLocation;
    private GameObject bulletSpwaned;

    public float shootTime;
    private float currentWait;
    private bool shot;

    private void ShootAtPlayer()
    { 
        enemyAnimator.SetTrigger("enemyShoot");
        shot = true;
        bulletSpwaned = Instantiate(bulletPrefab, enemyShootLocation.transform.position, transform.rotation);
    }

    private void OnDrawGizmos()
    {
        if (waypoints.Count == 0)
        {
            return;
        }
        Gizmos.color = Color.blue;
        foreach (Transform waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint.position, radius: 0.2f);
        }

        if (!Application.isPlaying)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, agent.destination);
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints.Count == 0)
        {
            return;
        }
        Gizmos.color = Color.magenta;
        foreach (Transform waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint.position, radius: 0.35f);
        }
    }
}
