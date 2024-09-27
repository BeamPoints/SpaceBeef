using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeath : MonoBehaviour
{
    [SerializeField] private bool enterDeath;

    private void OnTriggerStay(Collider _other)
    {
        if (!enterDeath)
            return;

        if (_other.GetComponent<Health>() != null)
            _other.GetComponent<Health>().GetDamage(int.MaxValue);
    }

    private void OnTriggerExit(Collider _other)
    {
        if (enterDeath)
            return;

        if (_other.GetComponent<Health>() != null)
            _other.GetComponent<Health>().GetDamage(int.MaxValue);
    }
}
