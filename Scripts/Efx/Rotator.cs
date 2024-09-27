using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

	private void Update ()
    {
        transform.Rotate(new Vector3(0.0f, rotationSpeed * Time.deltaTime, 0.0f), Space.Self);
	}
}
