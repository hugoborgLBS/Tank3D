using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shell : MonoBehaviour
{
    [SerializeField] private ShellData shellData;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        // apply initial forward velocity if data available
        if (shellData != null && rb != null)
        {
            rb.linearVelocity = transform.forward * shellData.Speed;
        }
    }

    public void Initialize(ShellData data)
    {
        shellData = data;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        float aoe = shellData != null ? shellData.AoE : 0f;
        float damage = shellData != null ? shellData.Damage : 0f;

        // apply damage to objects within AoE
        Collider[] hits = Physics.OverlapSphere(transform.position, aoe);
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        // simple effect placeholder: destroy shell
        Destroy(gameObject);
    }
}

public interface IDamageable
{
    void TakeDamage(float amount);
}
