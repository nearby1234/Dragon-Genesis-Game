using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    public GameObject FirePoint;
    public float MaxLength;
    public GameObject[] Prefabs;
    public GameObject start;
    GameObject startLaser;

    private int Prefab;
    private GameObject Instance;
    private Hovl_Laser LaserScript;
    private Hovl_Laser2 LaserScript2;


    // Lưu vị trí của model frame trước để tính chuyển động
    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position;
        Prefab = 0;
    }
    

    void Update()
    {
        
    }

    // Phương thức này sẽ được gọi từ Animation Event để kích hoạt laser
    public void FireLaser()
    {
        // Hủy instance laser cũ nếu có
        if (Instance != null)
        {
            Destroy(Instance);
        }
        startLaser = Instantiate(start, FirePoint.transform.position, FirePoint.transform.rotation, FirePoint.transform);
        //Instance.transform.parent = transform;);
        Instance = Instantiate(Prefabs[Prefab], FirePoint.transform.position, FirePoint.transform.rotation, FirePoint.transform);
        Instance.SetActive(true);
        //Instance.transform.parent = transform;
        LaserScript = Instance.GetComponent<Hovl_Laser>();
        LaserScript2 = Instance.GetComponent<Hovl_Laser2>();
    }

    // Phương thức này sẽ được gọi từ Animation Event để tắt laser
    public void StopLaser()
    {
        if (LaserScript != null) LaserScript.DisablePrepare();
        if (LaserScript2 != null) LaserScript2.DisablePrepare();
        Destroy(Instance);
        Destroy(startLaser);
    }

    // Xoay FirePoint dựa vào hướng chuyển động của model
    void RotateToMovementDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // Sử dụng Lerp để xoay mượt
        FirePoint.transform.rotation = Quaternion.Lerp(FirePoint.transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    // Nếu cần, có thể thêm phương thức thay đổi prefab laser
    public void NextLaser()
    {
        Prefab++;
        if (Prefab >= Prefabs.Length)
        {
            Prefab = 0;
        }
    }

    public void PreviousLaser()
    {
        Prefab--;
        if (Prefab < 0)
        {
            Prefab = Prefabs.Length - 1;
        }
    }

   
}
