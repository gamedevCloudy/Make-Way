using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{
    private NavMeshAgent enemyNav;
    private Vector3 startingPosition;

    private Animator anim;
    [SerializeField] private GameObject player;
    private Transform target;
    private float health;
    [SerializeField] private GameObject blood; 
    private int bloodInt = 1; 

    private enum State
    {
        Idle,
        ChaseTarget
    }
    private State state;


    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
        player = GameObject.Find("Player");
        health = 100;
        anim = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        target = player.transform;
        switch (state)
        {
            default:
            case State.Idle:
                FindTarget();

                break;


            case State.ChaseTarget:
                transform.LookAt(player.transform);
                anim.SetTrigger("Attack");
                break;

        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            health -= 10; 
        }


        if (health <= 0 && bloodInt == 1)
        {

            //spawn Particle fx
            Vector3 bloodPos = transform.position + Vector3.up;
            Instantiate(blood, bloodPos, Quaternion.identity);
            bloodInt = 0; 
            anim.SetTrigger("Death");
            Invoke("Death", 1f);
          
        }
    }

    private void FindTarget()
    {
        float targetRange = 1f;
        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < targetRange)
        {
            //attack target
            state = State.ChaseTarget;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            DealDamage(10);
        }
    }
    public void DealDamage(float damageToTake)
    {
        health -= (damageToTake * Time.deltaTime);

    }

    private void Death()
    {
        Destroy(gameObject);
    }

}
