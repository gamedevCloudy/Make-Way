using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBehaviour : MonoBehaviour
{/// <summary>
 /// To do:
 /// 1. Animate the AI 
 /// 2. Multiply on triggering Multipliers
 /// 
 /// </summary>


    [SerializeField] float counter = 2f;

    [SerializeField] private GameObject clone;
    private Vector3 offset;
    private GameObject playerContainer;
    NavMeshAgent nav;

    // Start is called before the first frame update
    void Start()
    {
        playerContainer = GameObject.Find("PlayerContainer");
        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(GameObject.Find("Player").transform.position);
        counter = 2f;
    }
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.SetParent(playerContainer.transform);
    }
    private void FixedUpdate()
    {
        counter -= 1f * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Multiplier2x") && counter <= 0f)
        {

            Multiply(1);
            counter = 2f;

        }
    }

    void Multiply(int multiplicationFactor)
    {
        for (int i = 0; i < multiplicationFactor; i++)
        {
            float offX = Random.Range(0f, 0.2f);
            float offZ = Random.Range(0f, 0.2f);
            offset = new Vector3(offX, transform.position.y, offZ);
            Vector3 spawnPos = transform.position + Vector3.forward + offset;
            GameObject playerClone = Instantiate(clone, spawnPos, Quaternion.identity);
            playerClone.transform.parent = GameObject.Find("PlayerContainer").transform;
        }
    }

}
