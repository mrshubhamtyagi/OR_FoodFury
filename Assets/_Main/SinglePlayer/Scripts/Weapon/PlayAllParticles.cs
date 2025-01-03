using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAllParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;
    private void OnEnable()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }
}
