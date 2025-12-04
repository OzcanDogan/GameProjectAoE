using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Current Resources")]
    [SerializeField] private int gold = 0;
    [SerializeField] private int population = 0;

    [Header("Limits")]
    [SerializeField] private int maxPopulation = 0;

    public int Gold => gold;
    public int Population => population;
    public int MaxPopulation => maxPopulation;

    // UI gibi þeyler dinleyebilsin diye event
    public event Action OnResourceChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Eðer sahne geçiþlerinde de ayný ResourceManager kalsýn istiyorsan:
        // DontDestroyOnLoad(gameObject);
    }

    // -------- GOLD --------

    public void AddGold(int amount)
    {
        if (amount == 0) return;

        gold = Mathf.Max(0, gold + amount);
        OnResourceChanged?.Invoke();
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;

        if (gold < amount)
            return false;

        gold -= amount;
        OnResourceChanged?.Invoke();
        return true;
    }

    // -------- POPULATION --------

    public bool TryAddPopulation(int amount)
    {
        if (amount <= 0) return true;

        if (population + amount > maxPopulation)
            return false;

        population += amount;
        OnResourceChanged?.Invoke();
        return true;
    }

    public void RemovePopulation(int amount)
    {
        if (amount <= 0) return;

        population = Mathf.Max(0, population - amount);
        OnResourceChanged?.Invoke();
    }

    public void AddPopulationCap(int amount)
    {
        if (amount == 0) return;

        maxPopulation = Mathf.Max(0, maxPopulation + amount);
        OnResourceChanged?.Invoke();
    }

    public void SetBasePopulationCap(int amount)
    {
        maxPopulation = Mathf.Max(0, amount);
        OnResourceChanged?.Invoke();
    }

    // Test için (Inspector’dan çaðýrmak istersen)
    public void DebugAddGold(int amount)
    {
        AddGold(amount);
    }
}
