using UnityEngine;
using Photon.Pun;

public class GameSetupManager : MonoBehaviourPunCallbacks
{
    [Header("Spawn Noktalarý")]
    public Transform spawnPointPlayer1; // Inspector'dan Sol Köþeyi ata
    public Transform spawnPointPlayer2; // Inspector'dan Sað Köþeyi ata

    [Header("Prefab Ayarý")]
    // Resources klasöründeki Barracks prefabýnýn tam adý
    public string barracksPrefabName = "Barracks";

    public override void OnJoinedRoom()
    {
        Debug.Log("Odaya girildi, Barracks oluþturuluyor...");
        CreateBarracks();
    }

    void CreateBarracks()
    {
        // Oyuncu numarasýný al (1, 2, 3...)
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        Transform selectedSpawnPoint = null;

        // Basit bir mantýk: 1 numara P1 noktasýna, diðerleri P2 noktasýna
        // (Daha fazla oyuncu varsa burayý switch-case ile geniþletebilirsin)
        if (actorNumber == 1)
        {
            selectedSpawnPoint = spawnPointPlayer1;
        }
        else
        {
            selectedSpawnPoint = spawnPointPlayer2;
        }

        if (selectedSpawnPoint != null)
        {
            // PhotonNetwork.Instantiate kullanan kiþi o objenin SAHÝBÝ olur.
            // UnitSpawner scripti de bu sahipliði kontrol eder.
            GameObject myBarracks = PhotonNetwork.Instantiate(
                barracksPrefabName,
                selectedSpawnPoint.position,
                selectedSpawnPoint.rotation
            );

            Debug.Log($"Oyuncu {actorNumber} için Barracks oluþturuldu.");
        }
    }
}