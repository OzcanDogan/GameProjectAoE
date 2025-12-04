using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    private readonly int isMovingHash = Animator.StringToHash("isMoving");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("Animator bulunamadı! Modelin içinde Animator olmalı.");
    }

    void Update()
    {
        if (agent == null || animator == null) return;

        bool moving = agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;
        animator.SetBool(isMovingHash, moving);
    }

    public void MoveTo(Vector3 target)
    {
        if (agent != null)
            agent.SetDestination(target);
    }
}
