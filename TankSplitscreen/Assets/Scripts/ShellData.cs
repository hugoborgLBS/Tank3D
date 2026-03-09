using UnityEngine;

[CreateAssetMenu(fileName = "ShellData", menuName = "Game/Shell Data", order = 1)]
public class ShellData : ScriptableObject
{
    [Tooltip("Number of rounds available. If -1 then infinite.")]
    public int AmmoCount = -1;

    [Tooltip("Area of Effect radius for explosions (units)")]
    public float AoE = 2f;

    [Tooltip("Damage applied by the shell on direct hit")]
    public float Damage = 25f;

    [Tooltip("Shots per second")]
    public float FireRate = 1f; // shots per second

    [Tooltip("Initial forward speed applied to the shell when spawned (units/sec)")]
    public float Speed = 20f;
}