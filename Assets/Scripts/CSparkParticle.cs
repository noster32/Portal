using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSparkParticle : CComponent
{
    [SerializeField] private ParticleSystem sparkParticle1;
    [SerializeField] private ParticleSystem sparkParticle2;

    public void PlayParticle(int num, Transform transform)
    {
        if (num == 1)
            StartCoroutine(SparkParticleCoroutine(sparkParticle1, transform));
        else if (num == 2)    
            StartCoroutine(SparkParticleCoroutine(sparkParticle2 , transform));
    }

    private IEnumerator SparkParticleCoroutine(ParticleSystem particle, Transform target)
    {
        ParticleSystem particleInstance = Instantiate(particle, target.position, target.rotation, this.transform);
        particleInstance.Play();

        yield return new WaitForSeconds(0.5f);

        particleInstance.Stop();

        yield return new WaitForSeconds(2f);

        Destroy(particleInstance.gameObject);

        yield return null;
    }
}
