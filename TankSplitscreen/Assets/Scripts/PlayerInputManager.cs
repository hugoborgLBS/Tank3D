using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Tooltip("Player 1 tank reference")]
    [SerializeField] private TankController player1Tank;

    [Tooltip("Player 2 tank reference")]
    [SerializeField] private TankController player2Tank;

    [Tooltip("Input Actions asset for player 1")]
    [SerializeField] private InputActionAsset player1Actions;

    [Tooltip("Input Actions asset for player 2")]
    [SerializeField] private InputActionAsset player2Actions;

    [Tooltip("Names of actions in the action map")]
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string aimActionName = "Aim";

    [SerializeField] private TankController.AimInputType player1AimType = TankController.AimInputType.ScreenPosition;
    [SerializeField] private TankController.AimInputType player2AimType = TankController.AimInputType.DeltaOrStick;

    [Tooltip("Optional CameraController GameObject. If assigned, its SetTargets(Transform,Transform) will be called.")]
    [SerializeField] private GameObject cameraControllerObject;

    private void Start()
    {
        if (player1Tank != null && player1Actions != null)
        {
            var moveRef = player1Actions.FindAction(moveActionName);
            var aimRef = player1Actions.FindAction(aimActionName);
            if (moveRef != null) player1Tank.SetMoveActionReference(InputActionReference.Create(moveRef));
            if (aimRef != null) player1Tank.SetAimActionReference(InputActionReference.Create(aimRef), player1AimType);
        }

        if (player2Tank != null && player2Actions != null)
        {
            var moveRef = player2Actions.FindAction(moveActionName);
            var aimRef = player2Actions.FindAction(aimActionName);
            if (moveRef != null) player2Tank.SetMoveActionReference(InputActionReference.Create(moveRef));
            if (aimRef != null) player2Tank.SetAimActionReference(InputActionReference.Create(aimRef), player2AimType);
        }

        // assign camera controller targets if provided
        if (cameraControllerObject != null)
        {
            var comp = cameraControllerObject.GetComponent<CameraController>();
            if (comp != null)
            {
                comp.SetTargets(player1Tank != null ? player1Tank.transform : null, player2Tank != null ? player2Tank.transform : null);
            }
            return;
        }

        // Otherwise search the scene for a CameraController using a non-deprecated method
        CameraController found = null;
        var allControllers = Resources.FindObjectsOfTypeAll<CameraController>();
        foreach (var c in allControllers)
        {
            // prefer active in hierarchy and in a loaded scene
            if (c == null) continue;
            if (!c.gameObject.activeInHierarchy) continue;
            if (!c.gameObject.scene.isLoaded) continue;
            found = c;
            break;
        }

        if (found != null)
        {
            found.SetTargets(player1Tank != null ? player1Tank.transform : null, player2Tank != null ? player2Tank.transform : null);
        }
    }
}
