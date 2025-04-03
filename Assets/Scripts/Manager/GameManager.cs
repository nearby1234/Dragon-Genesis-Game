using Unity.Cinemachine;
using UnityEngine;

public class GameManager : BaseManager<GameManager>
{
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;

    protected override void Awake()
    {
        base.Awake();
        cinemachineImpulseSource = FindAnyObjectByType<CinemachineImpulseSource>();
    }

    public void ShakeCamera()
    {
        cinemachineImpulseSource.GenerateImpulse();
    }
}
