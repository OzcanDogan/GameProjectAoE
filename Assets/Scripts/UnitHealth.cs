using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealth : MonoBehaviourPun
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Building?")]
    public bool isBuilding = false;

    // HEALTH BAR
    private GameObject healthBarCanvas;
    private Image fillImage;
    private float hideTimer = 0f;
    private bool barVisible = false;

    void Start()
    {
        currentHealth = maxHealth;

        healthBarCanvas = transform.Find("HealthBarCanvas")?.gameObject;

        if (healthBarCanvas == null)
        {
            Debug.LogError($"[{name}] HATA: HealthBarCanvas bulunamadı!");
            return;
        }

        Transform fl = healthBarCanvas.transform.Find("Fill");
        if (fl == null)
        {
            Debug.LogError($"[{name}] HATA: Fill objesi bulunamadı.");
            return;
        }

        fillImage = fl.GetComponent<Image>();
        if (fillImage == null)
        {
            Debug.LogError($"[{name}] HATA: Fill üzerinde Image component yok.");
            return;
        }

        // Başta gizli
        healthBarCanvas.SetActive(false);
    }

    [PunRPC]
    public void TakeDamage(int amount)
    {

        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"{name} DAMAGE ALDI → {currentHealth} / {maxHealth}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (fillImage == null) return;

        fillImage.fillAmount = (float)currentHealth / maxHealth;

        healthBarCanvas.SetActive(true);
        barVisible = true;
        hideTimer = 0f;
    }

    void Update()
    {
        if (healthBarCanvas != null && Camera.main != null)
            healthBarCanvas.transform.LookAt(Camera.main.transform);

        if (barVisible)
        {
            hideTimer += Time.deltaTime;
            if (hideTimer > 3f)
            {
                healthBarCanvas.SetActive(false);
                barVisible = false;
            }
        }
    }

    void Die()
    {
        Debug.Log($"{name} ÖLDÜ.");
        Debug.Log($"{name} DIE() FONKSİYONU ÇAĞIRILDI !!!");

        // Eğer bir bina ise
        if (isBuilding)
        {
            PopulationProvider pp = GetComponent<PopulationProvider>();

            // --- ANA BİNA KONTROLÜ (2 kat güvenlik) ---
            bool isTownHall =
                (pp != null && pp.isTownCenter == true) ||
                gameObject.name.Contains("pf_build_barracks_01");

            if (isTownHall)
            {
                Debug.Log($"🏛 {name}: TOWNHALL YIKILDI → OYUN BİTİYOR!");

                // Owner null olsa da çalışsın diye güvenli parametre
                string loserName = photonView.Owner != null
                    ? photonView.Owner.NickName
                    : "Unknown";

                GameManager.Instance.photonView.RPC(
                    "EndGame",
                    RpcTarget.All,
                    loserName
                );

                if (photonView.IsMine)
                    PhotonNetwork.Destroy(gameObject);

                return;
            }

            // --- NORMAL BİNA ---
            if (pp != null && photonView.IsMine)
            {
                ResourceManager.Instance.AddPopulationCap(-pp.populationProvided);
                Debug.Log($"[{name}] Küçük bina öldü → population -{pp.populationProvided}");
            }
        }

        // NORMAL ÖLÜM
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    public int GetHealth() => currentHealth;
}
