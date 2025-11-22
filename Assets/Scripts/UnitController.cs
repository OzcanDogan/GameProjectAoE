using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    // Animator parametresi — senin Animator içinde “isMoving” parametresi olacak
    private readonly int isMovingHash = Animator.StringToHash("isMoving");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>(); // Model içindeyse en doğrusu budur

        if (animator == null)
            Debug.LogError("Animator bulunamadı! Soldier modelinin içinde olmalı.");
    }

    void Update()
    {
        // Eğer agent yoksa boşuna devam etmeyelim
        if (agent == null || animator == null) return;

        // Hız kontrolü → karakter hareket ediyor mu?
        bool moving = agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;

        animator.SetBool(isMovingHash, moving);
    }

    // RTSManager burayı kullanıyor
    public void MoveTo(Vector3 target)
    {
        if (agent != null)
            agent.SetDestination(target);
    }
}
