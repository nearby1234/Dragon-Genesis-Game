using UnityEditor.Rendering;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public Animator GetAnimator()
    {
        return animator;
    }    
    public void ResetAnimPlayerIdle()
    {
        Debug.Log($"ResetAnimPlayerIdle : called");
       animator.SetTrigger("Reset");
    }
}
