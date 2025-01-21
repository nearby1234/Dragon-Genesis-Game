using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private RandomNavMeshMovement randomNavMeshMovement;
   
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        randomNavMeshMovement = GetComponent<RandomNavMeshMovement>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Animator GetAnimator() => animator;
    public NavMeshAgent GetNavMeshAgent() => agent;
    public RandomNavMeshMovement GetrandomNavMeshMovement() => randomNavMeshMovement;

        
}
