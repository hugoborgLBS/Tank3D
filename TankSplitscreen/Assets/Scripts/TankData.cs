using UnityEngine;

[CreateAssetMenu(fileName = "TankData", menuName = "Game/Tank Data", order = 0)]
public class TankData : ScriptableObject
{
    [Tooltip("Starting health for the tank")]
    public float Health = 100f;

    [Tooltip("Maximum forward speed (units/sec)")]
    public float MaxSpeed = 6f;

    [Tooltip("How quickly the tank reaches its target speed (units/sec^2)")]
    public float Acceleration = 10f;

    [Tooltip("Turn rate in degrees per second")]
    public float TurnRate = 120f;

    [Tooltip("Turret rotation speed in degrees per second")]
    public float TurretTurnRate = 180f;
}