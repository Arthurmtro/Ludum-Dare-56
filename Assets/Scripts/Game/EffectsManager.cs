using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem waveParticles;

    public void StartWaveParticles()
    {
        waveParticles.Play();
    }

    public void StopWaveParticles()
    {
        waveParticles.Stop();
    }
}
