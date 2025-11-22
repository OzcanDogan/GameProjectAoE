using UnityEngine;
using UnityEngine.AI;

public class SoldierController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("Animator bulunamadı! Model içinde Animator olduğundan emin ol.");
    }

    void Update()
    {
        // Harekete göre animasyon değiştir
        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);
    }

    public void MoveTo(Vector3 target)
    {
        agent.SetDestination(target);
    }
}
