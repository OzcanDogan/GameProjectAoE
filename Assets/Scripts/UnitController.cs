using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class UnitController : MonoBehaviourPun
{
    private NavMeshAgent agent;
    private Animator animator;

    private readonly int isMovingHash = Animator.StringToHash("isMoving");

    // Uzaktan gelenlerde hız tahmini için
    private Vector3 lastPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("Animator bulunamadı! Modelin içinde Animator olmalı.");

        // 🔥 Bu obje BANA ait değilse, NavMeshAgent'ı kapat
        // Böylece remote client'ta path hesaplamayıp sadece
        // Photon Transform View'dan gelen konumu takip eder
        if (!photonView.IsMine && agent != null)
        {
            agent.enabled = false;
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        if (animator == null) return;

        bool moving = false;

        if (photonView.IsMine)
        {
            // Sadece owner olan tarafta NavMeshAgent bilgisine bak
            if (agent == null) return;

            moving = agent.velocity.magnitude > 0.1f &&
                     agent.remainingDistance > agent.stoppingDistance;
        }
        else
        {
            // Remote objelerde hareketi position değişiminden tahmin et
            float distance = (transform.position - lastPosition).magnitude;
            moving = distance > 0.01f;
        }

        animator.SetBool(isMovingHash, moving);
        lastPosition = transform.position;
    }

    public void MoveTo(Vector3 target)
    {
        // 🔥 Sadece owner olan taraf bu unit'e komut verebilsin
        if (!photonView.IsMine)
            return;

        if (agent != null && agent.enabled)
        {
            agent.SetDestination(target);
        }
    }
}
