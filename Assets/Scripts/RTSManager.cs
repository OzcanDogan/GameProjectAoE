using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Photon.Pun;

public class RTSManager : MonoBehaviour
{
    public static RTSManager Instance;
    private Camera cam;

    [Header("Seçim Bilgisi")]
    public UnitSpawner selectedBarracks;

    [Header("Unit Seçimi")]
    public List<SelectableUnit> selectedUnits = new List<SelectableUnit>();

    [Header("Bina İnşa Ayarları")]
    public GameObject buildingPrefab; // RESOURCES içinde olacak
    public LayerMask groundLayer;

    private GameObject previewBuilding;
    private bool isPlacingBuilding = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleClickSelection();
        HandleMovement();

        if (isPlacingBuilding)
            HandleBuildingPlacement();
    }

    // ---------------------------------------------------
    // BİNA İNŞA BAŞLATMA
    // ---------------------------------------------------
    public void BuildHouse()
    {
        if (isPlacingBuilding) return;
        if (buildingPrefab == null) { Debug.LogError("Building Prefab atanmadı!"); return; }

        Building bd = buildingPrefab.GetComponent<Building>();
        if (bd == null) { Debug.LogError("Prefabta Building script yok!"); return; }

        if (ResourceManager.Instance.Gold < bd.goldCost)
        {
            Debug.Log("Altın yetersiz!");
            return;
        }

        // Preview bina oluştur (LOCAL)
        previewBuilding = Instantiate(buildingPrefab);

        // Colliders kapatılmazsa raycast çarpıyor!
        foreach (var col in previewBuilding.GetComponentsInChildren<Collider>())
            col.enabled = false;

        isPlacingBuilding = true;

        Debug.Log("İnşa modu başlatıldı.");
    }

    // ---------------------------------------------------
    // BİNA YERLEŞTİRME
    // ---------------------------------------------------
    void HandleBuildingPlacement()
    {
        if (previewBuilding == null)
        {
            isPlacingBuilding = false;
            return;
        }

        // Mouse ray
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;

        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            previewBuilding.transform.position = point;
        }

        // SOL TIK → BİNA KUR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Building bd = buildingPrefab.GetComponent<Building>();

            if (ResourceManager.Instance.TrySpendGold(bd.goldCost))
            {
                // Final pozisyonu al
                Vector3 pos = previewBuilding.transform.position;
                Quaternion rot = previewBuilding.transform.rotation;

                // Preview yok et
                Destroy(previewBuilding);
                previewBuilding = null;
                isPlacingBuilding = false;

                // 🔥 GERÇEK MULTIPLAYER BİNA OLUŞTUR
                PhotonNetwork.Instantiate(buildingPrefab.name, pos, rot);

                Debug.Log("🏛 Multiplayer bina yerleştirildi!");
            }
            else
            {
                Debug.Log("Yetersiz altın!");
            }
        }

        // SAĞ TIK → İPTAL
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(previewBuilding);
            previewBuilding = null;
            isPlacingBuilding = false;
            Debug.Log("İnşa iptal edildi.");
        }
    }

    // ---------------------------------------------------
    // SOL TIK → UNIT / BARRACKS SEÇİMİ
    // ---------------------------------------------------
    void HandleClickSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (isPlacingBuilding) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Unit seçimi
                SelectableUnit su = hit.transform.GetComponentInParent<SelectableUnit>();
                if (su != null)
                {
                    selectedBarracks = null;
                    ClearSelection();
                    AddToSelection(su);
                    return;
                }

                // Barracks seçimi
                UnitSpawner building = hit.transform.GetComponentInParent<UnitSpawner>();
                if (building != null && building.photonView.IsMine)
                {
                    selectedBarracks = building;
                    ClearSelection();
                    Debug.Log("Kışla seçildi: " + building.name);
                    return;
                }

                selectedBarracks = null;
                ClearSelection();
            }
        }
    }

    // ---------------------------------------------------
    // SAĞ TIK → UNIT HAREKET ETTİRME
    // ---------------------------------------------------
    void HandleMovement()
    {
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            if (isPlacingBuilding) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                foreach (SelectableUnit su in selectedUnits)
                {
                    UnitController uc = su.GetComponent<UnitController>();
                    if (uc) uc.MoveTo(hit.point);
                }
            }
        }
    }

    // ---------------------------------------------------
    // UNIT SEÇİM FONKSİYONLARI
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
            if (u != null) u.SetSelected(false);
        }
        selectedUnits.Clear();
    }

    // ---------------------------------------------------
    // UI BUTTON → UNIT SPAWN
    // ---------------------------------------------------
    public void UI_Button_SpawnVillager()
    {
        if (selectedBarracks != null)
            selectedBarracks.TrySpawnUnit("Villager");
    }

    public void UI_Button_SpawnSoldier()
    {
        if (selectedBarracks != null)
            selectedBarracks.TrySpawnUnit("Soldier");
    }
}
