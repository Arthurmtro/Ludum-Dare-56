using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem waveParticles;
    [SerializeField] private ParticleSystem deathParticles;

    public void StartWaveParticles()
    {
        waveParticles.Play();
    }

    public void StopWaveParticles()
    {
        waveParticles.Stop();
    }

    public void StartDeathParticles()
    {
        deathParticles.Play();
    }

    public void StopDeathParticles()
    {
        deathParticles.Stop();
    }
}
