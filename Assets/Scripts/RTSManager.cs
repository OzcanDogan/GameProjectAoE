using UnityEngine;
using System.Collections.Generic;

public class RTSManager : MonoBehaviour
{
    private Camera cam;

    public List<SelectableUnit> selectedUnits = new List<SelectableUnit>();

    void Start()
    {
        cam = Camera.main;

        if (cam == null)
            Debug.LogError("Main Camera bulunamadı! Tag'i MainCamera yap.");
    }

    void Update()
    {
        HandleClickSelection();
        HandleMovement();
    }

    // ---------------------------------------------------
    // 🟦 TEK TIK İLE SEÇİM (Box seçimi yokken çalışır)
    // ---------------------------------------------------
    void HandleClickSelection()
    {
        // Eğer seçim kutusu açıkken tıklanırsa tek tık seçimi devre dışı bırakalım
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast ile seçim
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                SelectableUnit su = hit.transform.GetComponentInParent<SelectableUnit>();

                // Bir unit’e tıklanmışsa
                if (su != null)
                {
                    ClearSelection();
                    AddToSelection(su);
                }
                else
                {
                    // Boş yere tıklayınca her şeyi deselect yap
                    ClearSelection();
                }
            }
        }
    }

    // ---------------------------------------------------
    // 🟩 SAĞ TIK GRUP HAREKETİ
    // ---------------------------------------------------
    void HandleMovement()
    {
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                foreach (SelectableUnit su in selectedUnits)
                {
                    su.GetComponent<UnitController>().MoveTo(hit.point);
                }
            }
        }
    }

    // ---------------------------------------------------
    // 🟡 MULTI SELECT FONKSIYONLARI (RTSMultiSelect kullanır)
    // ---------------------------------------------------
    public void AddToSelection(SelectableUnit unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            unit.SetSelected(true);
            selectedUnits.Add(unit);
        }
    }

    public void ClearSelection()
    {
        foreach (var u in selectedUnits)
        {
            if (u != null)
                u.SetSelected(false);
        }

        selectedUnits.Clear();
    }
}
