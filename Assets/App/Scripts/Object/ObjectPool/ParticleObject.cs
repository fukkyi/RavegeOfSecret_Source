using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : PoolableObject
{
    [SerializeField]
    protected ParticleSystem particle;

    protected void Awake()
    {
        if (particle == null)
        {
            particle = GetComponent<ParticleSystem>();
        }
    }

    public void PlayOfPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        particle.Play();
    }

    private void OnParticleSystemStopped()
    {
        DisableObject();
    }
}
