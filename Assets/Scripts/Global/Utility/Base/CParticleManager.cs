using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CParticleManager : CSingleton<CParticleManager>
{
    public void PlayParticle(ParticleSystem particle, Vector3 position, Quaternion rotation, float time = 0.5f)
    {
        StartCoroutine(ParticleCoroutine(particle, position, rotation, time));
    }

    public void PlayParticleEmit(ParticleSystem particle, Vector3 position, Quaternion rotation)
    {
        StartCoroutine(ParticleCoroutineEmit(particle, position, rotation));
    }

    private IEnumerator ParticleCoroutine(ParticleSystem particle, Vector3 position, Quaternion rotation, float time)
    {
        ParticleSystem particleInstance = Instantiate(particle, position, rotation);
        particleInstance.Play();

        yield return new WaitForSeconds(time);

        particleInstance.Stop();

        yield return new WaitForSeconds(2f);

        Destroy(particleInstance.gameObject);

        yield return null;
    }
    
    private IEnumerator ParticleCoroutineEmit(ParticleSystem particle, Vector3 position, Quaternion rotation)
    {
        ParticleSystem particleInstance = Instantiate(particle, position, rotation);
        particleInstance.Emit(1);
        Destroy(particleInstance.gameObject, 0.5f);
        yield return null;
    }

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance != null)
        {
            Debug.Log("There can only be one ParticleSpawn");
        }
        else
            m_oInstance = this;
    }
}
