using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostItem : MonoBehaviour
{
    [SerializeField] private ParticleSystem boostParticles;

    private void OnTriggerEnter(Collider WheelBarrow)
    {
        if (WheelBarrow.CompareTag("wheelBarrow"))
        {
            //test change
            Instantiate(boostParticles, transform.position, transform.rotation);
        }
    }
}
