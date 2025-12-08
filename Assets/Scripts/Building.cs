using UnityEngine;
using Photon.Pun;

public class Building : MonoBehaviourPun
{
    public int populationBonus = 3;
    public int goldCost = 10;

    void Start()
    {
        // Population sadece bina sahibinde artsın
        if (photonView.IsMine)
        {
            ResourceManager.Instance.AddPopulationCap(populationBonus);
            Debug.Log("🏛 Population +" + populationBonus + " artırıldı (Bu bina bana ait).");
        }
        else
        {
            Debug.Log("👁 Bu bina bana ait değil → population artırmıyorum.");
        }
    }
}
