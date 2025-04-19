using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : BaseManager<CameraManager>
{
    [SerializeField] private List<GameObject> cameras = new();
    [SerializeField] private Camera m_PATCamera;
    [SerializeField] private CinemachineVirtualCameraBase m_FreelookCamera;
    [SerializeField] private CinemachineVirtualCameraBase m_StartCamera;
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private bool m_IsRotatePlayer;
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
    [SerializeField] private CinemachineInputAxisController inputAxisController;

    protected override void Awake()
    {
        base.Awake();
        FindChildObject();
        cinemachineImpulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        GetTypeObjectInList<CinemachineInputAxisController>();
        m_PATCamera = GetObject("PATCamera").GetComponent<Camera>();
        orbitalFollow = m_FreelookCamera.GetComponent<CinemachineOrbitalFollow>();
        
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.CAMERA_SEND_VALUE, m_PATCamera);
        }
       StartCoroutine(DelayCamera());
    }

    public void SetActiveInputAxisController(bool value)
    {
        if (inputAxisController != null)
        {
            inputAxisController.enabled = value;
        }
    }
    public CinemachineVirtualCameraBase GetCameraCinemachine(string name)
    {
        GameObject obj = GetObject(name);
        CinemachineVirtualCameraBase virtualCameraBase = obj.GetComponent<CinemachineVirtualCameraBase>();
        return virtualCameraBase;
    }
    public void ShakeCamera()
    {
        cinemachineImpulseSource.GenerateImpulse();
    }
    public GameObject GetObject(string name)
    {
        foreach (GameObject tranform in cameras)
        {
            if (tranform.name.Equals(name))
            {
                return tranform;
            }
        }
        return null;
    }
    private void FindChildObject()
    {
        Transform[] childTranform = GetComponentsInChildren<Transform>();
        foreach (Transform tranform in childTranform)
        {
            cameras.Add(tranform.gameObject);
        }
    }
    private T GetTypeObjectInList<T>()
    {
        for(int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i].GetComponent<T>() != null)
            {
                inputAxisController = cameras[i].GetComponent<T>() as CinemachineInputAxisController;
            }
        }
        return default;
    }
    IEnumerator DelayCamera()
    {
        yield return new WaitForSeconds(1f);
        m_StartCamera.Priority = 10;
        orbitalFollow.HorizontalAxis.Value = 85;
        orbitalFollow.VerticalAxis.Value = 25;
    }    

}
