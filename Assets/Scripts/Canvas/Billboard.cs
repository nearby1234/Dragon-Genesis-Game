using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform m_target;
    [SerializeField] private bool m_Invert;

    private void Awake()
    {
        m_target = GameObject.Find("Player").GetComponent<Transform>();
    }
    private void Update()
    {
        RotationTarget();
    }
    private void RotationTarget()
    {
        Vector3 direction = transform.position - m_target.position;
        direction.y = 0;
        if (m_Invert)
        {
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

    }
}
