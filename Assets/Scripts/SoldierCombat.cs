using UnityEngine;
using Photon.Pun;

public class SoldierCombat : MonoBehaviourPun
{
    public int damage = 10;
    public float attackRange = 2f;
    public float attackRate = 1f;

    private float attackTimer = 0f;

    void Update()
    {
        if (!photonView.IsMine) return; // sadece benim askerim saldırabilir

        attackTimer += Time.deltaTime;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            PhotonView targetView = hit.GetComponent<PhotonView>();
            if (targetView == null) continue;
            if (targetView.Owner == photonView.Owner) continue; // takım arkadaşı → es geç

            UnitHealth health = hit.GetComponent<UnitHealth>();
            if (health == null) continue;

            // SADECE ASKER saldırabilir (kundaktaki çocuk bile köylüyü öldüremesin diye)
            if (attackTimer >= attackRate)
            {
                attackTimer = 0f;

                photonView.RPC("RPC_Attack", RpcTarget.All, targetView.ViewID);
            }
        }
    }

    [PunRPC]
    void RPC_Attack(int targetId)
    {
        PhotonView target = PhotonView.Find(targetId);
        if (target != null)
        {
            UnitHealth health = target.GetComponent<UnitHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
