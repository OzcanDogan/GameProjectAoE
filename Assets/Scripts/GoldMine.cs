using System.Collections.Generic;
using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [Header("Settings")]
    public float miningRange = 3f;      // Köylü şu mesafeye gelirse çalışsın
    public float interval = 3f;         // 3 saniyede bir gold
    private float timer;
    public int goldPerTick = 1;
    public float tickInterval = 3f;

    private List<UnitController> allUnits;    // sahnedeki tüm unitler bulmak için

    void Start()
    {
        // Tüm UnitController’ları bul (performans sorunu yok, az unit var)
        allUnits = new List<UnitController>(FindObjectsOfType<UnitController>());
    }
    private void OnTriggerEnter(Collider other)
    {
        var villager = other.GetComponent<VillagerController>();
        if (villager != null)
        {
            villager.StartMining(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var villager = other.GetComponent<VillagerController>();
        if (villager != null)
        {
            villager.StopMining();
        }
    }

    void Update()
    {
        int workerCount = 0;

        // Her unit için bak → Köylü mü? Yakın mı?
        foreach (var unit in allUnits)
        {
            if (unit.CompareTag("Villager"))
            {
                float dist = Vector3.Distance(unit.transform.position, transform.position);
                if (dist <= miningRange)
                    workerCount++;
            }
        }

        if (workerCount == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;

            ResourceManager.Instance.AddGold(workerCount);

            Debug.Log("Gold üretildi! WorkerCount: " + workerCount);
        }
    }
}
