using UnityEngine;

public class VegetationBreaker : MonoBehaviour
{
    [SerializeField] GameObject intact;
    [SerializeField] GameObject broken;

    bool sink=false;

    private void Awake()
    {
       intact.SetActive(true);
       broken.SetActive(false);
    }
    public void Break()
    {
        intact.SetActive(false);
        broken.SetActive(true);
        Invoke("Kill",5f);
    }

    void Kill()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ground"))
        {
            Break();
        }
    }
}
