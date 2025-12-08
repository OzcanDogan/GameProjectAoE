using UnityEngine;
using Photon.Pun;

public class GoldMineSpawner : MonoBehaviourPunCallbacks
{
    [Header("Gold Mine Ayarları")]
    public GameObject goldMinePrefab;
    public int mineCount = 10;

    [Header("Alan Sınırı")]
    public Transform bottomLeft;
    public Transform topRight;

    [Header("Rotation Ayarı")]
    public float yRotation = 180f;

    // ODAYA GİRİLDİĞİNDE ÇALIŞIR
    public override void OnJoinedRoom()
    {
        Debug.Log("💛 Odaya girildi → GoldMineSpawner çalışıyor.");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("👑 MASTER → Gold mine’lar spawn edilecek.");
            SpawnMines();
        }
        else
        {
            Debug.Log("❌ Bu oyuncu master değil → spawn etmeyecek.");
        }
    }

    void SpawnMines()
    {
        Debug.Log("🔨 SpawnMines() çağrıldı. Spawn edilecek mine sayısı: " + mineCount);

        for (int i = 0; i < mineCount; i++)
        {
            Vector3 pos = GetRandomPositionOnTerrain();
            Quaternion rot = Quaternion.Euler(0f, yRotation, 0f);

            Debug.Log($"🪨 {i + 1}. mine spawn deneniyor → Pos: {pos}");

            GameObject obj = PhotonNetwork.InstantiateRoomObject("mine", pos, rot);

            if (obj != null)
                Debug.Log($"✅ Mine {i + 1} başarıyla oluşturuldu: {obj.name}");
            else
                Debug.Log($"❌ Mine {i + 1} oluşturulamadı! InstantiateRoomObject NULL döndü.");
        }

        Debug.Log("💛 Spawn döngüsü tamamlandı.");
    }

    Vector3 GetRandomPositionOnTerrain()
    {
        float randomX = Random.Range(bottomLeft.position.x, topRight.position.x);
        float randomZ = Random.Range(bottomLeft.position.z, topRight.position.z);

        Vector3 pos = new Vector3(randomX, 0f, randomZ);

        if (Terrain.activeTerrain != null)
        {
            pos.y = Terrain.activeTerrain.SampleHeight(pos);
            Debug.Log("🌍 Terrain yüksekliği uygulandı → Y: " + pos.y);
        }
        else
        {
            Debug.Log("⚠ Terrain yok → Raycast ile yükseklik bulunacak.");
            Ray ray = new Ray(pos + Vector3.up * 100f, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                pos.y = hit.point.y;
                Debug.Log("📌 Raycast yüksekliği → " + pos.y);
            }
            else
            {
                Debug.Log("❌ Raycast ile yükseklik bulunamadı, Y=0 kullanılıyor.");
            }
        }

        return pos;
    }
}
