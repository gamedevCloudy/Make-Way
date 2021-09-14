using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aaPathMover : MonoBehaviour
{
    //Locomotion
    private NavMeshAgent navMeshAgent;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private Rigidbody rb;
    private Animator anim;

    [Header("Targets")]
    [SerializeField] private GameObject enemyContainer;
    [SerializeField] private Transform enemyTower;
    [SerializeField] private Transform win;
    
    [Header("PlayerHealth")]
    [SerializeField] private int health;
    [SerializeField] private int healthMax = 100;
    [SerializeField] private ParticleSystem blueBlood; 

    //StateMachine
    private enum State
    {
        FollowPath,
        AttackTower,
        AttackEnemy,
        RunForWin
    }
    private State state;
   


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        FindObjectOfType<aaPathCreator>().OnNewPathCreated += SetPoints;
        
        state = State.FollowPath;
        anim = GetComponent<Animator>();

        enemyTower = GameObject.Find("EnemyTower").transform;
        enemyContainer = GameObject.Find("EnemyContainer");
        win = GameObject.Find("FinalDestination").transform;

        health = healthMax; 
    }


    private void SetPoints(IEnumerable<Vector3> points)
    {
        pathPoints = new Queue<Vector3>(points);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case State.FollowPath:
                UpdatePathing();
                FindEnemies();
                
                break;

            case State.AttackEnemy:
                UpdatePathing();
                if (enemyContainer.transform.childCount > 0)
                {
                    Transform target = enemyContainer.transform.GetChild(0);
                    navMeshAgent.SetDestination(target.position);
                    if (navMeshAgent.remainingDistance < 0.4f)
                    {
                        anim.SetTrigger("Attack");
                        target.GetComponent<enemyAI>().DealDamage(20);
                    }
                }
                else state = State.AttackTower;   
                break;

            case State.AttackTower:
                if (enemyTower != null)
                {
                    Vector3 attackOffset = enemyTower.position + new Vector3(0.5f, 0, -0.5f);
                    navMeshAgent.SetDestination(attackOffset);
                    transform.LookAt(enemyTower);
                    InvokeRepeating("DealTowerDamage", 0.5f, 1f);

                }
                else state = State.RunForWin; 
                break;

            case State.RunForWin:
                navMeshAgent.SetDestination(win.transform.position);
                transform.LookAt(win.transform); 
                break;

        }


        //Animation zone
        if(navMeshAgent.velocity.magnitude > 1.5f)
        {
            anim.SetTrigger("Running");
        }

        if( health <= 0)
        {
            Instantiate(blueBlood, transform.position, Quaternion.identity);
            Invoke("PlayerDead", 1.5f); 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Enemy"))
        {
            health -= 7;
            Debug.Log("Current Health: " + health); 
        }
    }

    private void UpdatePathing()
    {
        if (ShouldSetDestination())
            navMeshAgent.SetDestination(pathPoints.Dequeue());

    }

    private bool ShouldSetDestination()
    {
        if (pathPoints.Count == 0)
            return false;
        if (navMeshAgent.hasPath == false || navMeshAgent.remainingDistance < 0.5f)
            return true;
        return false;
    }

    private void FindEnemies()
    {
        if(enemyContainer.transform.childCount > 0)
        {
            float targetRange = 3f;
            if (Vector3.Distance(transform.position, enemyContainer.transform.GetChild(0).position) <= targetRange)
            {
                state = State.AttackEnemy;
            }
        }
        else FindTowerTarget(); 
    }

    private void FindTowerTarget()
    {
        float targetRange = 3f;
        if(enemyTower!= null)
        {
            if (Vector3.Distance(transform.position, enemyTower.position) < targetRange)
            {
                state = State.AttackTower;
            }
        }
        else state = State.RunForWin; 
    }

    private void DealTowerDamage()
    {
        if (enemyTower != null)
        {
            EnemyTower tower = enemyTower.GetComponent<EnemyTower>();
            anim.SetTrigger("Attack");
            tower.DealDamage(1);
        }
    }

    private void PlayerDead()
    {

        Destroy(gameObject); 
    }
}
