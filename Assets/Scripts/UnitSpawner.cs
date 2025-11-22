using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unitPrefab;   // Spawn edilecek karakter prefab
    public Transform spawnPoint;    // Nerede doğacak

    public void SpawnUnit()
    {
        if (unitPrefab == null || spawnPoint == null)
        {
            Debug.LogError("UnitSpawner: Prefab veya SpawnPoint eksik!");
            return;
        }

        GameObject go = Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Unit Spawned: " + go.name);
    }
}
