using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float m_TurnSpeed;
    [SerializeField] private Camera Camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        RotationPlayer();
    }

    private void RotationPlayer()
    {
        
        float yourCamera = Camera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yourCamera, 0), m_TurnSpeed * Time.deltaTime);
    }
}
