using Unity.Behavior;
using UnityEngine;


public class SpawnAxeModel : MonoBehaviour
{
    [SerializeField] private GameObject m_AxePrefabs;
    [SerializeField] private Transform player;        // Tham chiếu đến Player
    [SerializeField] private Transform m_ParentAxe;
    [SerializeField] private float m_Scale;
    [SerializeField] private float speedAxe;
    [SerializeField] private float torqueForceAxe;
    [SerializeField] private Vector3 angelRotate;
    [SerializeField] private Vector3 AxePos;
    [SerializeField] private BehaviorTreeSO bullTankSetting;
    [SerializeField] private BehaviorGraphAgent graphAgent;

    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Register(ListenType.CLICK_BUTTON_PLAYAGAIN, OnEventClickButtonPlayAgain);
        }
    }
    private void OnDestroy()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.Unregister(ListenType.CLICK_BUTTON_PLAYAGAIN, OnEventClickButtonPlayAgain);
        }
    }
    public void SpawnAxe()
    {

        Vector3 spawnPos = m_ParentAxe.position + AxePos;
        spawnPos.x += GetRandomYOffsetByPhase();
        Quaternion spawnRot = m_ParentAxe.rotation;
        GameObject gameObject = Instantiate(m_AxePrefabs, spawnPos, spawnRot);
        gameObject.transform.localScale = Vector3.one * m_Scale;

        // 3. Tính hướng bay đến player
        Vector3 dir = (player.position - m_ParentAxe.position).normalized;
        Vector3 defaultForward = m_AxePrefabs.transform.forward;
        Quaternion rot = Quaternion.FromToRotation(defaultForward, dir);
        rot *= m_AxePrefabs.transform.rotation; // Giữ nguyên offset xoay sẵn có của prefab
        rot *= Quaternion.Euler(angelRotate); // Bù thêm xoay trục Y
        gameObject.transform.rotation = rot;

        gameObject.SetActive(true);
        if (gameObject.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = dir * speedAxe;
            rb.AddRelativeTorque(Vector3.up * torqueForceAxe, ForceMode.VelocityChange);
        }

        Destroy(gameObject, 0.45f);
    }
    private float GetRandomYOffsetByPhase()
    {
        if (graphAgent.BlackboardReference
        .GetVariable<PhaseState>("PhaseStateBoss", out var bbVar))
        {
            // bbVar là BlackboardVariable<PhaseState>
            PhaseState phaseState = bbVar.Value;
            return phaseState switch
            {
                PhaseState.First => Random.Range(-bullTankSetting.rangeFirstPhase, bullTankSetting.rangeFirstPhase),
                PhaseState.Second => Random.Range(-bullTankSetting.rangeSecondPhase, bullTankSetting.rangeSecondPhase),
                PhaseState.Third => Random.Range(-bullTankSetting.rangeThirdPhase, bullTankSetting.rangeThirdPhase),
                _ => 0f,
            };
        }
        else
        {
            Debug.LogWarning("Không tìm thấy biến 'PhaseStateBoss' trên Blackboard");
        }
        return 0f;

    }
    public void HideAxe() => m_AxePrefabs.SetActive(false);
    public void ShowAxe()
    {
        m_AxePrefabs.SetActive(true);
       
        if (graphAgent.BlackboardReference
       .GetVariable<PhaseState>("PhaseStateBoss", out var bbVar))
        {
            PhaseState phaseState = bbVar.Value;
            if(phaseState == PhaseState.Third)
            {
                ParticleSystem particleSystem = m_AxePrefabs.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    particleSystem.Play();
                }
            } 
        }    
    }
    private void OnEventClickButtonPlayAgain(object value)
    {
        if(!m_AxePrefabs.activeSelf)
        {
            m_AxePrefabs.SetActive(true);
            if(m_AxePrefabs.TryGetComponent<ParticleSystem>(out var particleSystem))
            {
                if(particleSystem.isPlaying)
                {
                    particleSystem.Stop();
                }
                
            }

        }
    }
}
