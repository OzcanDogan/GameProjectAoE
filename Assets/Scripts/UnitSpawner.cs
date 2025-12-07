using UnityEngine;
using Photon.Pun;

public class UnitSpawner : MonoBehaviourPun
{
    [Header("Spawn Noktası")]
    public Transform spawnPoint;

    [Header("Maliyetler")]
    public int villagerCost = 50;
    public int soldierCost = 100;
    public int populationCost = 1; // Her asker kaç nüfus yiyor?

    // RTSManager bu fonksiyonu çağıracak
    public void TrySpawnUnit(string unitType)
    {
        // 1. Yetki Kontrolü
        if (!photonView.IsMine)
        {
            Debug.LogWarning("Bu bina senin değil!");
            return;
        }

        if (ResourceManager.Instance == null) return;

        int cost = 0;
        string prefabName = "";

        if (unitType == "Villager")
        {
            cost = villagerCost;
            prefabName = "Player"; // Resources'daki prefab adı
        }
        else if (unitType == "Soldier")
        {
            cost = soldierCost;
            prefabName = "Soldier"; // Resources'daki prefab adı
        }

        // --- İŞTE DÜZELTME BURADA ---

        // 1. Önce Altın Kontrolü
        if (ResourceManager.Instance.Gold >= cost)
        {
            // 2. Sonra Nüfus Eklenebiliyor mu Kontrolü (TryAddPopulation true dönerse eklemiştir)
            if (ResourceManager.Instance.TryAddPopulation(populationCost))
            {
                // 3. Başarılıysa Parayı Harca
                ResourceManager.Instance.TrySpendGold(cost);

                // 4. Ve Askeri Oluştur
                PhotonNetwork.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation);

                Debug.Log(prefabName + " üretildi. Nüfus arttı.");
            }
            else
            {
                Debug.Log("Nüfus Limiti Dolu! (Population Limit Reached)");
            }
        }
        else
        {
            Debug.Log("Altın Yetersiz!");
        }
    }
}