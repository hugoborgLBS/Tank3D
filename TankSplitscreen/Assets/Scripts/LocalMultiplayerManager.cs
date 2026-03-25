using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Registers to the PlayerInputManager join events and assigns simple identity data to each joined player.
/// Attach this to a persistent GameObject (e.g. "Managers").
/// Requires a PlayerInputManager in the scene that spawns player prefabs.
/// </summary>
public class LocalMultiplayerManager : MonoBehaviour
{
    [Tooltip("Colors assigned to players in join order")]
    [SerializeField] private Color[] playerColors = new Color[] { Color.red, Color.blue };

    private PlayerInputManager pim;
    private readonly List<PlayerInput> joined = new List<PlayerInput>();

    private void Awake()
    {
        pim = FindObjectOfType<PlayerInputManager>();
        if (pim == null)
        {
            Debug.LogWarning("LocalMultiplayerManager: No PlayerInputManager found in scene. Please add one to enable local joining.");
            return;
        }

        pim.onPlayerJoined += OnPlayerJoined;
        pim.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDestroy()
    {
        if (pim != null)
        {
            pim.onPlayerJoined -= OnPlayerJoined;
            pim.onPlayerLeft -= OnPlayerLeft;
        }
    }

    private void OnPlayerJoined(PlayerInput pi)
    {
        if (pi == null) return;

        // assign id based on join order (1 or 2)
        int id = joined.Count + 1;
        joined.Add(pi);

        // clamp id to available colors
        Color c = playerColors.Length > 0 ? playerColors[(id - 1) % playerColors.Length] : Color.white;

        // set player name
        pi.gameObject.name = $"Player {id}";

        // ensure a PlayerIdentity component is present and set values
        var identity = pi.gameObject.GetComponent<PlayerIdentity>();
        if (identity == null) identity = pi.gameObject.AddComponent<PlayerIdentity>();
        identity.SetId(id);
        identity.SetColor(c);

        Debug.Log($"Player joined: {pi.gameObject.name} (devices: {pi.devices.Count})");
    }

    private void OnPlayerLeft(PlayerInput pi)
    {
        if (pi == null) return;

        joined.Remove(pi);

        // optional: renumber remaining players' identity ids
        for (int i = 0; i < joined.Count; i++)
        {
            var p = joined[i];
            p.gameObject.name = $"Player {i + 1}";
            var identity = p.GetComponent<PlayerIdentity>();
            if (identity != null)
            {
                identity.SetId(i + 1);
                identity.SetColor(playerColors.Length > 0 ? playerColors[i % playerColors.Length] : Color.white);
            }
        }

        Debug.Log($"Player left: {pi.gameObject.name}");
    }
}
