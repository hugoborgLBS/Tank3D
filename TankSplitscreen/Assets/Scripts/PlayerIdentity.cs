using UnityEngine;

/// <summary>
/// Simple component to store player id and color for visibility.
/// Movement logic remains untouched; other systems can read PlayerIdentity.
/// </summary>
public class PlayerIdentity : MonoBehaviour
{
    [SerializeField] private int id = 0;
    [SerializeField] private Color color = Color.white;

    public int Id => id;
    public Color Color => color;

    public void SetId(int newId)
    {
        id = newId;
    }

    public void SetColor(Color c)
    {
        color = c;
        ApplyColor();
    }

    private void ApplyColor()
    {
        // try to set a renderer color if available (works with simple materials)
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null && rend.material != null)
        {
            rend.material.color = color;
        }
    }
}
