using UnityEngine;
using UnityEngine.UI;

public class UnitSpawner : MonoBehaviour
{
    [Header("Spawn Ayarları")]
    public GameObject unitPrefab;   // Spawn edilecek karakter prefab
    public Transform spawnPoint;    // Nerede doğacak

    [Header("Maliyetler")]
    public int goldCost = 50;
    public int populationCost = 1;

    [Header("UI")]
    public Button spawnButton;      // İstersen butonu otomatik enable/disable etmek için

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

        // Ön kontrol
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

        GameObject go = Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Unit Spawned: " + go.name);
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
