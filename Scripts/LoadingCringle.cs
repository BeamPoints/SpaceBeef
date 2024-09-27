using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCringle : MonoBehaviour {

	[SerializeField] float rotSpeed = 300.0f;
	[SerializeField] bool loading = false;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (loading)
		{
			Application.targetFrameRate = 60;
			transform.Rotate(0, 0, rotSpeed * Time.deltaTime, Space.Self);
		}
		else
		{
			Application.targetFrameRate = 244;
		}
	}
}
