using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement1 : MonoBehaviour
{
    NavMeshAgent agent;
    Transform targetTrans;
    GameObject triggerBox;

	void Start ()
    {
        targetTrans = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        triggerBox = GameObject.FindGameObjectWithTag("TriggerBox");
    }
	
	void Update ()
    {
        if (triggerBox.GetComponent<TriggerBox>().isActive == true)
        {
            agent.destination = targetTrans.position;
            transform.LookAt(targetTrans);
        }
    }
}
