using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    public bool isSelected = false;

    // pivot sorunu için ekledik
    public Transform selectionPoint;

    private OutlineController outline;

    void Start()
    {
        outline = GetComponent<OutlineController>();
        if (outline != null)
            outline.EnableOutline(false);
    }

    public void SetSelected(bool value)
    {
        isSelected = value;

        if (outline != null)
            outline.EnableOutline(value);
    }

    // ekran pozisyonu getter
    public Vector2 GetScreenPoint(Camera cam)
    {
        if (selectionPoint != null)
            return cam.WorldToScreenPoint(selectionPoint.position);

        return cam.WorldToScreenPoint(transform.position);
    }
}
