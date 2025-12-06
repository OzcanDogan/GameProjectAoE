using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UnitSpawner : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    public GameObject unitPrefab;   // Spawn edilecek karakter prefab (Resources içinde olmalı)
    public Transform spawnPoint;    // Nerede doğacak

    [Header("Maliyetler")]
    public int goldCost = 50;
    public int populationCost = 1;

    [Header("UI")]
    public Button spawnButton;      // İstersen butonu otomatik enable/disable etmek için

    private void Awake()
    {
        // İstersen butona otomatik bağlanalım (Inspector'dan vermişsen sorun değil, üstüne yazar)
        if (spawnButton != null)
        {
            // Önce eski listener'ları temizle, aynı fonksiyonu 10 kere eklemesin
            spawnButton.onClick.RemoveAllListeners();
            spawnButton.onClick.AddListener(SpawnUnit);
        }
    }

    private void OnEnable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += UpdateButtonState;
        }

        UpdateButtonState();
    }

    private void OnDisable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= UpdateButtonState;
        }
    }

    public void SpawnUnit()
    {
        // 🔍 ÖNCE PHOTON DURUMUNU LOGLAYALIM
        Debug.Log($"[UnitSpawner] SpawnUnit çağrıldı. IsConnectedAndReady={PhotonNetwork.IsConnectedAndReady}, InRoom={PhotonNetwork.InRoom}, PlayerCount={PhotonNetwork.CurrentRoom?.PlayerCount}");

        // Odaya bağlı değilsek spawnlama
        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
        {
            Debug.LogWarning("[UnitSpawner] Odaya bağlı değilken spawn denendi, iptal.");
            return;
        }

        if (unitPrefab == null || spawnPoint == null)
        {
            Debug.LogError("UnitSpawner: Prefab veya SpawnPoint eksik!");
            return;
        }

        var rm = ResourceManager.Instance;
        if (rm == null)
        {
            Debug.LogError("UnitSpawner: ResourceManager yok!");
            return;
        }

        // Resource kontrolleri
        if (rm.Gold < goldCost)
        {
            Debug.Log("Yetersiz altın!");
            return;
        }

        if (rm.Population + populationCost > rm.MaxPopulation)
        {
            Debug.Log("Nüfus limiti dolu! Yeni unit basılamaz.");
            return;
        }

        // Harcamayı dene
        if (!rm.TrySpendGold(goldCost))
        {
            Debug.Log("UnitSpawner: TrySpendGold başarısız.");
            return;
        }

        if (!rm.TryAddPopulation(populationCost))
        {
            // Çok düşük ihtimal ama rollback yapalım
            rm.AddGold(goldCost);
            Debug.LogWarning("UnitSpawner: Nüfus eklenemedi, gold geri verildi.");
            return;
        }

        // Prefab ismini logla (Resources içinde mi test ediyoruz)
        Debug.Log("[UnitSpawner] Photon instantiate name = " + unitPrefab.name);
        Debug.Log("[UnitSpawner] Prefab path test = Resources.Load(\"" + unitPrefab.name + "\") = " + Resources.Load(unitPrefab.name));

        // 🔥 SADECE PHOTON INSTANTIATE
        GameObject go = PhotonNetwork.Instantiate(
            unitPrefab.name,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Debug.Log("[UnitSpawner] Unit Spawned: " + go.name);
    }

    private void UpdateButtonState()
    {
        if (spawnButton == null || ResourceManager.Instance == null)
            return;

        var rm = ResourceManager.Instance;
        bool canAfford = rm.Gold >= goldCost &&
                         rm.Population + populationCost <= rm.MaxPopulation;

        spawnButton.interactable = canAfford;
    }
}
