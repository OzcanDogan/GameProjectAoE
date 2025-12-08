using Photon.Pun;
using UnityEngine;

public class UnitHealth : MonoBehaviourPun
{
    public int maxHealth = 100;
    private int currentHealth;

    public bool isBuilding = false; // Bina mı?

    void Start()
    {
        currentHealth = maxHealth;
    }

    [PunRPC]
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} öldü.");

        if (isBuilding)
        {
            // Population bonus geri alınır
            Building building = GetComponent<Building>();
            if (building != null)
                ResourceManager.Instance.AddPopulationCap(-building.populationBonus);
        }

        PhotonNetwork.Destroy(gameObject);
    }

    public int GetHealth() => currentHealth;
}
