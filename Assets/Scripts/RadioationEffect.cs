using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioationEffect : MonoBehaviour
{
    private ParticleSystem radiationParticles;

    void Start()
    {
        radiationParticles = GetComponentInChildren<ParticleSystem>();
        radiationParticles.Stop();
    }

    public void ActivateRadiation()
    {
        if (radiationParticles != null)
        {
            radiationParticles.Play();
        }
    }

    public void DeactivateRadiation()
    {
        if (radiationParticles != null)
        {
            radiationParticles.Stop();
        }
    }
}
