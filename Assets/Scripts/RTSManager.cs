using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems; // UI Tıklaması kontrolü için gerekli

public class RTSManager : MonoBehaviour
{
    public static RTSManager Instance;
    private Camera cam;

    [Header("Seçim Bilgisi")]
    public UnitSpawner selectedBarracks;

    [Header("Unit Seçimi")]
    public List<SelectableUnit> selectedUnits = new List<SelectableUnit>();

    [Header("Bina İnşa Ayarları")]
    public GameObject buildingPrefab;

    // YENİ EKLENDİ: Binanın ağaçların tepesine çıkmaması için sadece zemini görecek katman
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
        // 1. Standart seçim ve hareket işlemleri
        HandleClickSelection();
        HandleMovement();

        // 2. Eğer inşaat modundaysak, bina mouse'u takip etmeli
        if (isPlacingBuilding)
        {
            HandleBuildingPlacement();
        }
    }

    // ---------------------------------------------------
    // 1. BİNA İNŞA SİSTEMİ (BUGFIX YAPILMIŞ HALİ)
    // ---------------------------------------------------

    // Bu fonksiyonu UI Butonuna bağla
    public void BuildHouse()
    {
        // Eğer zaten elimizde yerleşmeyi bekleyen bina varsa yenisini yaratma
        if (isPlacingBuilding) return;

        // --- GÜVENLİK KONTROLLERİ ---
        if (buildingPrefab == null) { Debug.LogError("HATA: RTSManager'da Building Prefab atanmamış!"); return; }

        Building bd = buildingPrefab.GetComponent<Building>();
        if (bd == null) { Debug.LogError("HATA: Prefab üzerinde 'Building' scripti yok!"); return; }

        if (ResourceManager.Instance == null) { Debug.LogError("HATA: Sahnede ResourceManager yok!"); return; }

        // Parası yetiyor mu kontrolü (Ama henüz harcamıyoruz)
        if (ResourceManager.Instance.Gold < bd.goldCost)
        {
            Debug.Log("Altın yetersiz!");
            return;
        }

        // --- BİNAYI YARATMA ---
        previewBuilding = Instantiate(buildingPrefab);

        // --- CRITICAL FIX: COLLIDER KAPATMA ---
        // Bina mouse ucundayken Raycast (Lazer) kendi çatısına çarpmasın diye hayalet yapıyoruz.
        foreach (var col in previewBuilding.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        // Takip modunu başlat
        isPlacingBuilding = true;
        Debug.Log("İnşa modu başladı. Yer seçiniz...");
    }

    void HandleBuildingPlacement()
    {
        if (previewBuilding == null)
        {
            isPlacingBuilding = false;
            return;
        }

        // 1. KAMERADAN IŞIN ÇIKAR
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // 2. MATEMATİKSEL ZEMİN OLUŞTUR (Yüksekliği 0 olan sonsuz bir düzlem)
        // Bu yöntem Terrain Collider'a veya Layer ayarlarına ihtiyaç duymaz!
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        // 3. IŞIN BU DÜZLEME ÇARPTI MI?
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            // Çarptığı noktayı bul
            Vector3 point = ray.GetPoint(rayDistance);

            // Binayı o noktaya taşı
            previewBuilding.transform.position = point;
        }

        // --- SOL TIK: YERLEŞTİRME ---
        if (Input.GetMouseButtonDown(0))
        {
            // UI üzerine tıklıyorsak binayı oraya bırakma
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

            Building bd = buildingPrefab.GetComponent<Building>();

            // Para kontrolü ve harcama
            if (ResourceManager.Instance.TrySpendGold(bd.goldCost))
            {
                // Colliderları geri aç (Artık katı olsun)
                foreach (var col in previewBuilding.GetComponentsInChildren<Collider>())
                {
                    col.enabled = true;
                }

                // İnşaatı bitir
                previewBuilding = null;
                isPlacingBuilding = false;
                Debug.Log("Bina matematiksel düzleme yerleştirildi!");
            }
            else
            {
                Debug.Log("Paran yetmedi, yerleştiremedin!");
            }
        }

        // --- SAĞ TIK: İPTAL ---
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(previewBuilding);
            previewBuilding = null;
            isPlacingBuilding = false;
            Debug.Log("İptal edildi.");
        }
    }

    // ---------------------------------------------------
    // 2. SEÇİM SİSTEMİ (SENİN KODLARIN - AYNI KALDI)
    // ---------------------------------------------------
    void HandleClickSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // İnşaat modundaysak seçim yapma, binayı koymaya çalışıyoruzdur
            if (isPlacingBuilding) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 1. Unit Seçimi
                SelectableUnit su = hit.transform.GetComponentInParent<SelectableUnit>();
                if (su != null)
                {
                    selectedBarracks = null;
                    ClearSelection();
                    AddToSelection(su);
                    return;
                }

                // 2. Barracks Seçimi
                UnitSpawner building = hit.transform.GetComponentInParent<UnitSpawner>();
                if (building != null)
                {
                    if (building.photonView.IsMine)
                    {
                        selectedBarracks = building;
                        ClearSelection();
                        Debug.Log("Kışla Seçildi: " + building.name);
                    }
                }
                else
                {
                    selectedBarracks = null;
                    ClearSelection();
                }
            }
        }
    }

    // ---------------------------------------------------
    // UI BUTON FONKSİYONLARI
    // ---------------------------------------------------
    public void UI_Button_SpawnVillager()
    {
        if (selectedBarracks != null) selectedBarracks.TrySpawnUnit("Villager");
    }

    public void UI_Button_SpawnSoldier()
    {
        if (selectedBarracks != null) selectedBarracks.TrySpawnUnit("Soldier");
    }

    // ---------------------------------------------------
    // UNIT HAREKET FONKSİYONLARI
    // ---------------------------------------------------
    void HandleMovement()
    {
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            // İnşaat modundaysak hareket emri verme (iptal tuşu ile karışmasın)
            if (isPlacingBuilding) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                foreach (SelectableUnit su in selectedUnits)
                {
                    if (su.GetComponent<UnitController>())
                        su.GetComponent<UnitController>().MoveTo(hit.point);
                }
            }
        }
    }

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
}