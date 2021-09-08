using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aaPathMover : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private Rigidbody rb;
    private Animator anim;

    [SerializeField] private Transform enemyTower;
    [SerializeField] private GameObject[] enemy;
    [SerializeField] private Transform win;
    

    [SerializeField] private GameObject enemyContainer; 

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
        anim = GetComponent<Animator>();
        state = State.FollowPath;
        enemyTower = GameObject.Find("EnemyTower").transform;
        enemyContainer = GameObject.Find("EnemyContainer");
        win = GameObject.Find("FinalDestination").transform; 
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
        if(navMeshAgent.velocity.magnitude > 1f)
        {
            anim.SetTrigger("Running");
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
        
        if (enemyTower == null) state = State.RunForWin; 
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
}
