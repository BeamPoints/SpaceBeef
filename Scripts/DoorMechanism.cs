using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMechanism : MonoBehaviour
{
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private AudioSource doorSound;
    [SerializeField] private float speed = 2;
    [SerializeField] private float delayStep = 0.05f;

    void OnTriggerEnter( Collider col)
    {
        if(col.tag == "Player")
        {
            if (doorSound != null)
                doorSound.Play();

            StopAllCoroutines();
            StartCoroutine(OpenDoorLeft());
            StartCoroutine(OpenDoorRight());
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            if (doorSound != null)
                doorSound.Play();

            StopAllCoroutines();
            StartCoroutine(CloseDoorLeft());
            StartCoroutine(CloseDoorRight());
        }
    }

    private IEnumerator OpenDoorLeft()
    {
        while (leftDoor != null && leftDoor.transform.localPosition.x >= -2.0f)
        {
            leftDoor.transform.Translate(leftDoor.transform.right * -speed, Space.World);
            yield return new WaitForSeconds(delayStep);
        }
    }

    private IEnumerator OpenDoorRight()
    {
        while (rightDoor != null && rightDoor.transform.localPosition.x <= 2.0f)
        {
            rightDoor.transform.Translate(rightDoor.transform.right * speed, Space.World);
            yield return new WaitForSeconds(delayStep);
        }
    }

    private IEnumerator CloseDoorLeft()
    {
        while (leftDoor != null && leftDoor.transform.localPosition.x <= 0.0f)
        {
            leftDoor.transform.Translate(leftDoor.transform.right * speed, Space.World);
            yield return new WaitForSeconds(delayStep);
        }
    }

    private IEnumerator CloseDoorRight()
    {
        while (rightDoor != null && rightDoor.transform.localPosition.x >= 0.0f)
        {
            rightDoor.transform.Translate(rightDoor.transform.right * -speed, Space.World);
            yield return new WaitForSeconds(delayStep);
        }
    }
}
