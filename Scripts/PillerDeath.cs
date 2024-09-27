using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillerDeath : MonoBehaviour {

    public GameObject piller;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionExit()
    {
        piller.gameObject.SetActive(false);
    }
}
