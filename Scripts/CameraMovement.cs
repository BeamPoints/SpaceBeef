using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public List<Transform> targets;       // Array mit allen spielern
	[SerializeField] float cameraOffset = 20.0f;

	float aspectRatio;
	float tanFov;

	void Start()
	{
		aspectRatio = Screen.width / Screen.height;
		tanFov = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f);
	}

	void LateUpdate()
	{
        Vector3 vectorBetweenPlayers1 = Vector3.zero;
        Vector3 vectorBetweenPlayers2 = Vector3.zero;
        Vector3 vectorBetweenPlayers = Vector3.zero;
        Vector3 middlePoint = Vector3.zero;
        
		switch (targets.Count)
		{
			case 1:
				vectorBetweenPlayers = targets[0].position;
				break;
			case 2:
				vectorBetweenPlayers = targets[0].position - targets[1].position;
                break;

			case 3:
				vectorBetweenPlayers1 = targets[0].position - targets[1].position;
				vectorBetweenPlayers2 = targets[2].position;
				vectorBetweenPlayers = vectorBetweenPlayers1 - vectorBetweenPlayers2;
				break;

			case 4:
				vectorBetweenPlayers1 = targets[0].position - targets[1].position;
				vectorBetweenPlayers2 = targets[2].position - targets[3].position;
				vectorBetweenPlayers = vectorBetweenPlayers1 - vectorBetweenPlayers2;
                break;
		}

        for (int i = 0; i < targets.Count; i++)
        {
            middlePoint.x += targets[i].position.x / targets.Count;
            middlePoint.y += targets[i].position.y / targets.Count;
            middlePoint.z += targets[i].position.z / targets.Count;
        }


        // Position the camera in the center.
        Vector3 newCameraPos = transform.position;
		newCameraPos.x = middlePoint.x;
		newCameraPos.y = middlePoint.y;
		transform.position = newCameraPos;

		// Calculate the new distance.
		float distanceBetweenPlayers = vectorBetweenPlayers.magnitude;
		float cameraDistance = (distanceBetweenPlayers / 4.0f / aspectRatio) / tanFov;

		// Set camera to new position.
		Vector3 dir = (transform.position - middlePoint).normalized;
		transform.position = middlePoint + dir * (cameraDistance + cameraOffset);
	}

}
