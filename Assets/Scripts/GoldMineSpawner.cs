using UnityEngine;

public class GoldMineSpawner : MonoBehaviour
{
    [Header("Gold Mine Ayarlarý")]
    public GameObject goldMinePrefab;
    public int mineCount = 10;

    [Header("Alan Sýnýrý")]
    public Transform bottomLeft;   // Sol-alt köþe
    public Transform topRight;     // Sað-üst köþe

    [Header("Rotation Ayarý")]
    public float yRotation = 180f;  // Y ekseni rotasyonu (Inspector’dan ayarlanabilir)

    private void Start()
    {
        SpawnMines();
    }

    void SpawnMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            Vector3 pos = GetRandomPositionOnTerrain();

            // Y ekseninde rotasyon oluþtur
            Quaternion rot = Quaternion.Euler(0f, yRotation, 0f);

            // Maden oluþtur
            Instantiate(goldMinePrefab, pos, rot);
        }
    }

    Vector3 GetRandomPositionOnTerrain()
    {
        // X ve Z'yi köþe objelerinin aralýðýnda seç
        float randomX = Random.Range(bottomLeft.position.x, topRight.position.x);
        float randomZ = Random.Range(bottomLeft.position.z, topRight.position.z);

        Vector3 pos = new Vector3(randomX, 0f, randomZ);

        // Terrain yüksekliðini ayarla
        if (Terrain.activeTerrain != null)
        {
            float y = Terrain.activeTerrain.SampleHeight(pos);
            pos.y = y;
        }
        else
        {
            Ray ray = new Ray(pos + Vector3.up * 100f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                pos.y = hit.point.y;
            }
        }

        return pos;
    }
}
