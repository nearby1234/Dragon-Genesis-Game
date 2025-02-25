using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class MiniBoss : MonoBehaviour
{
    private FSM fSM;
    [SerializeField] private float m_Range;
    [SerializeField] private float m_Speed;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject m_Player;

    public Animator Animator => animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        m_Player = GameObject.Find("Player");
    }
    void Start()
    {
        fSM = new FSM();
        fSM.ChangeState(new IdleState(this,fSM));
    }

    // Update is called once per frame
    void Update()
    {
        fSM.Update();
    }

    public bool PlayerInRange()
    {
        return (Vector3.Distance(this.transform.position, m_Player.transform.position) <= m_Range);
    }

    public void TestButoon()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            fSM.ChangeState(new WalkState(this, fSM));
        }
    }    

    public void MoveToPlayer()
    {
        Debug.Log("boss enter walk");
        transform.position = Vector3.MoveTowards(transform.position,m_Player.transform.position, m_Speed * Time.deltaTime);
    }    
}
