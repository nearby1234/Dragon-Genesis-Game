using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : BaseManager<CameraManager>
{
    [SerializeField] private List<GameObject> cameras = new();
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
    [SerializeField] private CinemachineInputAxisController inputAxisController;

    protected override void Awake()
    {
        base.Awake();
        FindChildObject();
        cinemachineImpulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        GetTypeObjectInList<CinemachineInputAxisController>();
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
    private GameObject GetObject(string name)
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
