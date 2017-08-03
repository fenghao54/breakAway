using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TriggerBox : MonoBehaviour
{
    GameObject[] zombies;
    public bool isActive = false;

	void Start ()
    {
        zombies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void OnTriggerEnter(Collider other)
    {
        isActive = true;

        for(int i = 0; i < zombies.Length; i++)
        {
            zombies[i].GetComponent<Animator>().SetTrigger("Go");
            zombies[i].GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
