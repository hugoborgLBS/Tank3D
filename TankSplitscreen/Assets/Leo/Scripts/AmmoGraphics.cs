using UnityEngine;

public class AmmoGraphics : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float gravity = 3.3f;

    Vector3 startPos;
    Vector3 velocity;

    private void Awake()
    {
        startPos = transform.position;
    }
    void OnEnable()
    {
        velocity = transform.forward * speed;
        velocity.y = 0;
    }

    void Update()
    {
        CheckCollision();

        velocity += Vector3.down * gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity.normalized;
    }

    void CheckCollision()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, velocity.normalized, out hit, velocity.magnitude * Time.deltaTime))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                HitGround();
                return;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            print("hit ground");
            HitGround();
        }
    }

    void HitGround()
    {
        transform.position = startPos;
        gameObject.SetActive(false);
    }
}
