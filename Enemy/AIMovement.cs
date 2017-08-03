using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Transform targetTrans;
    TriggerBox triggerBox;

	void Start ()
    {
        targetTrans = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        triggerBox = GetComponent<TriggerBox>();

    }
	
	void Update ()
    {
        agent.destination = targetTrans.position;
        transform.LookAt(targetTrans);
    }
}
