using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI populationText;

    private void Start()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceUI: ResourceManager.Instance null! Sahneye ResourceManager ekli mi?");
            return;
        }

        ResourceManager.Instance.OnResourceChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        if (ResourceManager.Instance == null) return;

        var rm = ResourceManager.Instance;

        if (goldText != null)
            goldText.text = $"GOLD : {rm.Gold}";

        if (populationText != null)
            populationText.text = $"POPULATION : {rm.Population}/{rm.MaxPopulation}";
    }
}
