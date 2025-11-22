using UnityEngine;
using UnityEngine.UI;

public class RTSMultiSelect : MonoBehaviour
{
    public RectTransform selectionBox;
    private Camera cam;

    private Vector2 startPos;
    private Vector2 endPos;

    private RTSManager manager;

    void Start()
    {
        cam = Camera.main;
        manager = GetComponent<RTSManager>();
        selectionBox.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            endPos = startPos;

            selectionBox.gameObject.SetActive(true);
            UpdateBoxVisual();
        }

        if (Input.GetMouseButton(0))
        {
            endPos = Input.mousePosition;
            UpdateBoxVisual();
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectionBox.gameObject.SetActive(false);
            SelectUnits();
        }
    }
    void UpdateBoxVisual()
    {
        float minX = Mathf.Min(startPos.x, endPos.x);
        float minY = Mathf.Min(startPos.y, endPos.y);
        float maxX = Mathf.Max(startPos.x, endPos.x);
        float maxY = Mathf.Max(startPos.y, endPos.y);

        Vector2 size = new Vector2(maxX - minX, maxY - minY);
        Vector2 pos = new Vector2(minX, minY);

        selectionBox.sizeDelta = size;
        selectionBox.anchoredPosition = pos;
    }


    void SelectUnits()
    {
        float minX = Mathf.Min(startPos.x, endPos.x);
        float maxX = Mathf.Max(startPos.x, endPos.x);
        float minY = Mathf.Min(startPos.y, endPos.y);
        float maxY = Mathf.Max(startPos.y, endPos.y);

        foreach (var unit in FindObjectsOfType<SelectableUnit>())
        {
            Vector3 sp = cam.WorldToScreenPoint(
                unit.selectionPoint ? unit.selectionPoint.position : unit.transform.position
            );

            if (sp.x >= minX && sp.x <= maxX && sp.y >= minY && sp.y <= maxY)
            {
                manager.AddToSelection(unit);
            }
        }
    }
}
