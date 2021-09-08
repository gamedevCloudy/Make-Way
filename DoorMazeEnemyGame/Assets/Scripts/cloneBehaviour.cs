using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class cloneBehaviour : MonoBehaviour
{
    /// <summary>
    /// Should follow player?
    /// 
    /// </summary>
    // Start is called before the first frame update

    
    private NavMeshAgent nav;
    private Animator anim; 
    
        
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
            nav.SetDestination(GameObject.Find("Player").transform.position);
            transform.LookAt(GameObject.Find("Player").transform);
    }
}
