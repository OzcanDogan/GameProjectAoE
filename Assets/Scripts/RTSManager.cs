using UnityEngine;
using System.Collections.Generic;

public class RTSManager : MonoBehaviour
{
    private Camera cam;
    public GameObject buildingPrefab;
    public List<SelectableUnit> selectedUnits = new List<SelectableUnit>();
    private GameObject previewBuilding;   // Önizleme nesnesi
    private bool isPlacingBuilding = false;

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
        if (isPlacingBuilding)
        {
            HandleBuildingPlacement();
        }
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
    void PlaceBuilding()
    {
        previewBuilding.GetComponent<Collider>().enabled = true;

        previewBuilding = null;
        isPlacingBuilding = false;

        Debug.Log("Bina yerleştirildi!");
    }
    void CancelPlacement()
    {
        Building bd = buildingPrefab.GetComponent<Building>();
        ResourceManager.Instance.AddGold(bd.goldCost);

        Destroy(previewBuilding);
        previewBuilding = null;
        isPlacingBuilding = false;

        Debug.Log("Bina yerleştirme iptal edildi.");
    }
    void HandleBuildingPlacement()
    {
        // Raycast ile mouse pozisyonunu bul
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 2000f))
        {
            previewBuilding.transform.position = hit.point;
        }

        // Sol tık → bina yerleşsin
        if (Input.GetMouseButtonDown(0))
        {
            PlaceBuilding();
        }

        // Sağ tık → iptal
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }
    public void BuildHouse()
    {
        if (isPlacingBuilding) return; // Zaten bina yerleştiriliyorsa tekrar başlama

        // Maliyet kontrolü
        Building bd = buildingPrefab.GetComponent<Building>();
        if (!ResourceManager.Instance.TrySpendGold(bd.goldCost))
        {
            Debug.Log("Yetersiz altın!");
            return;
        }

        // Önizleme objesini oluştur
        previewBuilding = Instantiate(buildingPrefab);
        foreach (var col in previewBuilding.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        } // Taşıma sırasında çarpışma olmasın
        isPlacingBuilding = true;
    }
}
