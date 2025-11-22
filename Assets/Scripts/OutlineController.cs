using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 0.03f;

    private Renderer[] renderers;

    void Awake()
    {
        // Tüm Renderer’ları otomatik bul (skinned + mesh)
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void EnableOutline(bool enable)
    {
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                if (enable)
                {
                    m.SetFloat("_Outline", outlineWidth);
                    m.SetColor("_OutlineColor", outlineColor);
                }
                else
                {
                    m.SetFloat("_Outline", 0f);
                }
            }
        }
    }
}
