using UnityEngine;
using Photon.Pun;

public class PopulationProvider : MonoBehaviourPun
{
    [Header("Nüfus Kapasitesi")]
    public int populationProvided = 10;

    [Header("Ana Bina mı? (TownCenter)")]
    public bool isTownCenter = false;

    private void Start()
    {
        // Population sadece sahibi için artsın
        if (photonView.IsMine)
        {
            ResourceManager.Instance.AddPopulationCap(populationProvided);
            Debug.Log($"🏛 {name}: Population +{populationProvided} eklendi (bina bana ait).");
        }
        else
        {
            Debug.Log($"👁 {name}: Bu bina bana ait değil, population eklemiyorum.");
        }
    }
}
