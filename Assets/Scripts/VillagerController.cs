using UnityEngine;
using UnityEngine.AI;

public class VillagerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    private bool isMining = false;
    private GoldMine currentMine = null;
    private float mineTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isMining)
        {
            // Madende kazma animasyonu oynat
            animator.SetBool("isMining", true);
            animator.SetBool("isMoving", false);

            mineTimer += Time.deltaTime;
            if (mineTimer >= currentMine.tickInterval)
            {
                ResourceManager.Instance.AddGold(currentMine.goldPerTick);
                mineTimer = 0f;
            }

            // Madende olduğunda hareket etmesin
            agent.SetDestination(transform.position);
            return;
        }

        // Normal yürüyüş animasyonu
        bool isMoving = agent.velocity.sqrMagnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isMining", false);
    }

    public void MoveTo(Vector3 target)
    {
        isMining = false;
        animator.SetBool("isMining", false);

        agent.SetDestination(target);
    }

    public void StartMining(GoldMine mine)
    {
        isMining = true;
        currentMine = mine;
        mineTimer = 0f;
    }

    public void StopMining()
    {
        isMining = false;
        animator.SetBool("isMining", false);
        currentMine = null;
    }
    public void Hit()
    {
        // intentionally left blank
    }
}
