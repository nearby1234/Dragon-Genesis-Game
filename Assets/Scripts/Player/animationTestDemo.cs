using System.Collections;
using System.Threading;
using UnityEngine;

public class animationTestDemo : MonoBehaviour
{
    public Animator animator;
    public GameObject m_slash;
    public Transform m_Tranform;
    public float m_Timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Show()
    {
        GameObject slash = SlashPool.instance.GetSlash();

        if(slash == null)
        {
            Debug.Log("test");
        }    
        slash.transform.position = m_Tranform.position;
        slash.transform.rotation = m_Tranform.rotation;
        StartCoroutine(Delay(slash));

    }    

    IEnumerator Delay(GameObject slash)
    {
        yield return new WaitForSeconds(m_Timer);
        SlashPool.instance.ReturnPool(slash);
    }    
}
