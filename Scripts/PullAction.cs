using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullAction : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private List<Health> neighbourWalls;
    [SerializeField] private GameObject vacuumParticlePrefab;
    [SerializeField] private GameObject vacuumAudioPrefab;
    [SerializeField] private float pullTimer;
    [SerializeField] private bool showNeighbours;
    [SerializeField] private float strenght;
    [SerializeField] private bool oppositeDirection;

    private Vector3 direction;
    private List<Rigidbody> bodies;
    private bool isPullActive;
    private bool stopPull;
    private bool isVacuumParticle;
    
    private void Start ()
    {
        bodies = new List<Rigidbody>();
        CheckNeighbours();
    }

    private void Update()
    {
        if(neighbourWalls.Count > 0 && health == null)
            isPullActive = neighbourWalls.Any(x => x.CurrentHealth > 0);

        if(isPullActive && !stopPull)
        {
            pullTimer -= Time.deltaTime;

            if(!isVacuumParticle)
            {
                if(vacuumParticlePrefab != null)
                {
                    GameObject vacuumParticle = Instantiate(vacuumParticlePrefab, transform);

                    if (!oppositeDirection)
                        vacuumParticle.transform.localRotation = Quaternion.Euler(0.0f, -90.0f, 180f);
                    else
                        vacuumParticle.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                }

                if(vacuumAudioPrefab != null)
                    Instantiate(vacuumAudioPrefab, transform);

                isVacuumParticle = true;
            }

            if(pullTimer <= 0.0f)
                stopPull = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isPullActive || stopPull)
            return;

        foreach (var rb in bodies)
            if(rb != null)
            {
                if (oppositeDirection)
                    direction = transform.position - rb.transform.position;
                else
                    direction = transform.position - rb.transform.position;

                rb.AddForce(direction * strenght);
            }
    }

    private void OnTriggerStay(Collider _coll)
    {
        if (isPullActive && !stopPull)
            if (_coll.CompareTag("Player"))
                if(_coll.GetComponent<Rigidbody>() && !bodies.Contains(_coll.gameObject.GetComponent<Rigidbody>()))
                    bodies.Add(_coll.gameObject.GetComponent<Rigidbody>());
    }

    private void OnTriggerExit(Collider _coll)
    {
        if (_coll.CompareTag("Player"))
        {
            if (bodies == null && bodies.Count < 1)
                return;

            if(bodies.Count > 0 && bodies.Contains(_coll.gameObject.GetComponent<Rigidbody>()))
                bodies.Remove(_coll.gameObject.GetComponent<Rigidbody>());
        }
    }

    private void CheckNeighbours()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 4);

        foreach (var collider in colliders)
            if (collider.CompareTag("OuterWalls"))
                if (!neighbourWalls.Contains(collider.GetComponent<Health>()) && collider.GetComponent<Health>() != health)
                    neighbourWalls.Add(collider.GetComponent<Health>());
    }

    private void OnDrawGizmos()
    {
        if (!showNeighbours)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(neighbourWalls[0].transform.position + (Vector3.forward * -2), 1);
        Gizmos.DrawSphere(neighbourWalls[1].transform.position + (Vector3.forward * -2), 1);
    }
}