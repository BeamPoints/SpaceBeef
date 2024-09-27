using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {

	[SerializeField] GameObject wpn_Ground;

	void Start ()
	{
		
	}

	private void OnTriggerEnter(Collider collsion)
	{
		if(collsion.tag == "Player")
		{

			Destroy(gameObject);
		}
	}



}
