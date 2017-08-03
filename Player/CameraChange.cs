using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour {

    public GameObject Camera1;
    public GameObject Camera2;
    bool back;
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.T))
        {
            
            Camera1.gameObject.SetActive(false);
            Camera2.gameObject.SetActive(true);
            
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            Camera1.gameObject.SetActive(true);
            Camera2.gameObject.SetActive(false);

        }
        


    }
}
