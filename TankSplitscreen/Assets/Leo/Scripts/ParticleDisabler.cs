using UnityEngine;

public class PooledParticle : MonoBehaviour
{
    ParticleSystem[] particles;
    void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        foreach (var ps in particles)
        {
            ps.Play();
        }

        StartCoroutine(DisableWhenDone());
    }

    System.Collections.IEnumerator DisableWhenDone()
    {
        bool alive = true;

        while (alive)
        {
            alive = false;

            foreach (var ps in particles)
            {
                if (ps.IsAlive(true))
                {
                    alive = true;
                    break;
                }
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }
}