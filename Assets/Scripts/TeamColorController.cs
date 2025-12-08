using UnityEngine;
using Photon.Pun;

public class TeamColorController : MonoBehaviour
{
    public enum UnitType
    {
        Soldier,
        Villager
    }

    [Header("Bu unit tipi nedir?")]
    public UnitType unitType;

    [Header("Rengi deðiþecek kýsýmlar")]
    public Renderer[] parts; // MeshRenderer veya SkinnedMeshRenderer buraya atanacak

    private void Start()
    {
        // Unit'in sahibini bul
        PhotonView pv = GetComponentInParent<PhotonView>();
        if (pv == null || pv.Owner == null)
        {
            Debug.LogWarning($"{name} üzerinde PhotonView veya Owner yok, renk atanamadý.");
            return;
        }

        int actorId = pv.OwnerActorNr; // Player 1 = 1, Player 2 = 2 ...

        // Aktöre ve tipe göre renk seç
        Color colorToApply = GetColorFor(actorId, unitType);

        // Renderer'lara uygula
        foreach (var rend in parts)
        {
            if (rend == null) continue;
            rend.material.color = colorToApply;
        }
    }

    private Color GetColorFor(int actorId, UnitType type)
    {
        // ASKERLER
        if (type == UnitType.Soldier)
        {
            if (actorId == 1) // Player 1 asker
                return Color.green; // yeþil
            else if (actorId == 2) // Player 2 asker
                return Color.red;   // kýrmýzý
        }
        // KÖYLÜLER
        else if (type == UnitType.Villager)
        {
            if (actorId == 1) // Player 1
                return Color.green;   // yeþil köylü üst kýyafeti
            else if (actorId == 2) // Player 2
                return Color.red;
        }

        // Beklenmeyen durum (3. oyuncu vs.) – debug olsun diye mor
        return Color.magenta;
    }
}
