using UnityEngine;

public class TankHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private TankData tankData;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = tankData != null ? tankData.Health : 100f;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        // simple placeholder: destroy tank
        Destroy(gameObject);
    }
}