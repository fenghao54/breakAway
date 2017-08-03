using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource bg_AudioSource;

    void Start () {
        bg_AudioSource.volume=GameStatic.bg_volume;
		
	}
	
	void Update () {
		
	}
}
