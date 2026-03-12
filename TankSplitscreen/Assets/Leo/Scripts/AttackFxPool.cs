using UnityEngine;

public class AttackFxPool : MonoBehaviour
{
    [SerializeField] GameObject[] fx;

    public void SpawnFX()
    {
        foreach (var f in fx)
        {
            if (!f.activeInHierarchy)
            {
                f.SetActive(true);
                return;
            }
        }
    }
}
