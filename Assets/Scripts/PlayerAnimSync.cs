using UnityEngine;
using Photon.Pun;

public class PlayerAnimSync : MonoBehaviourPun
{
    private Animator animator;
    private Vector3 lastPosition;
    private readonly int isMovingHash = Animator.StringToHash("isMoving");

    [SerializeField] private float moveThreshold = 0.01f;   // ne kadar hareket ederse "yürüyor" saysın

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("[PlayerAnimSync] Animator bulunamadı! Modelin child'ında Animator olmalı.");

        lastPosition = transform.position;
    }

    void Update()
    {
        if (animator == null) return;

        // Pozisyon değişimine bakarak hareket ediyor mu hesabı
        float distance = (transform.position - lastPosition).magnitude;
        bool isMoving = distance > moveThreshold;

        animator.SetBool(isMovingHash, isMoving);

        lastPosition = transform.position;
    }
}
