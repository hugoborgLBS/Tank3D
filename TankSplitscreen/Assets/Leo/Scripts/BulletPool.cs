using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] GameObject[] bullets;

    public void SpawnBullet()
    {
        foreach (var bullet in bullets)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return;
            }
        }
    }
}
