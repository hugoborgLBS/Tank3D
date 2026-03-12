using UnityEngine;
using UnityEngine.InputSystem;

public class TankGraphics : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AttackFxPool fxPool;
    [SerializeField] BulletPool bulletPool;

    InputAction fireAction;

    private void Start()
    {
        fireAction = InputSystem.actions.FindAction("Fire");
    }
    private void Update()
    {
        //debug
        if(fireAction.WasPerformedThisFrame())
        {
            AttackVisuals();
        }

    }
    public void AttackVisuals()
    {
        anim.SetTrigger("attack");
        fxPool.SpawnFX();
        bulletPool.SpawnBullet();
    }
}
