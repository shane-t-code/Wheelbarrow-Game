using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointItem : MonoBehaviour
{
    [SerializeField] private ParticleSystem pointParticles;
    private void OnTriggerEnter(Collider WheelBarrow)
    {
        if (WheelBarrow.CompareTag("wheelBarrow"))
        {
            Instantiate(pointParticles, transform.position, transform.rotation);
        }
    }
}
