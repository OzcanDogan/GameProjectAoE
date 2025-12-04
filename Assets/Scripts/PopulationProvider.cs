using UnityEngine;

public class PopulationProvider : MonoBehaviour
{
    [Header("Nüfus Kapasitesi")]
    [SerializeField] private int populationProvided = 10;

    private void Start()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddPopulationCap(populationProvided);
        }
        else
        {
            Debug.LogError("PopulationProvider: ResourceManager bulunamadý!");
        }
    }
}
