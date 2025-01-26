using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RandomNavMeshMovement : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private NavMeshSurface surface;
    [SerializeField] private float stopDistance = 1f; 

    private Vector3 center; 
    private Vector3 size;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        surface = GameObject.Find("NavmeshSurface").GetComponent<NavMeshSurface>();
    }

    void Start()
    {
        enemyController.GetNavMeshAgent().agentTypeID = GetAgentTypeID();
        size = surface.size;
        center = surface.transform.position + surface.center;
        //MoveToRandomPosition();
    }
   public void MoveToRandomPosition()
    {
        Vector3 randomPoint = GetRandomPointInVolume();
        NavMeshHit hit;

        // Kiểm tra vị trí có hợp lệ trên NavMesh hay không
        if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
        {
            enemyController.GetNavMeshAgent().SetDestination(hit.position); // Di chuyển tới vị trí hợp lệ
        }
    }

   private Vector3 GetRandomPointInVolume()
    {
        // Tạo điểm ngẫu nhiên trong vùng NavMesh Surface (Volume)
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = center.y; // Giữ nguyên độ cao
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, y, z);
    }
    public void EnemyMoveTarget()
    {
        // Kiểm tra nếu đã tới đích
        if (!enemyController.GetNavMeshAgent().pathPending 
            && enemyController.GetNavMeshAgent().remainingDistance <= stopDistance)
        {
            MoveToRandomPosition();
        }
    }

    private int GetAgentTypeID()
    {
        int settingsCount = NavMesh.GetSettingsCount(); // Số lượng Agent Type hiện có
        string agentName = "Creep";
        for (int i = 0; i < settingsCount; i++)
        {
            // Lấy thông tin của Agent Type theo index
            NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(i);
            string agentTypeName = NavMesh.GetSettingsNameFromID(settings.agentTypeID);
            if (agentTypeName == agentName)
            {
                return settings.agentTypeID;
            }

        }
        return 0;
    }

}
