using UnityEngine;

public class Building : MonoBehaviour
{
    public int populationBonus = 3;  // Bu bina population cap'i ne kadar yükseltecek?
    public int goldCost = 10;        // Binanın maliyeti

    void Start()
    {
        // Bina kurulduğu anda population cap'i artır
        ResourceManager.Instance.AddPopulationCap(populationBonus);
    }
}
    