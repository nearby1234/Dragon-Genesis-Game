using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : BaseManager<CameraManager>
{
    [SerializeField] private List<GameObject> cameras = new();
    [SerializeField] private Camera m_PATCamera;
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
    [SerializeField] private CinemachineInputAxisController inputAxisController;

    protected override void Awake()
    {
        base.Awake();
        FindChildObject();
        cinemachineImpulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        GetTypeObjectInList<CinemachineInputAxisController>();
        m_PATCamera = GetObject("PATCamera").GetComponent<Camera>();
    }
    private void Start()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.CAMERA_SEND_VALUE, m_PATCamera);
        }
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

}
