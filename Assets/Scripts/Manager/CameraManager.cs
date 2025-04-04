using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : BaseManager<CameraManager>
{
    [SerializeField] private List<GameObject> cameras = new();
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;

    protected override void Awake()
    {
        base.Awake();
        FindChildObject();
        cinemachineImpulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }

    private void FindChildObject()
    {
        Transform[] childTranform = GetComponentsInChildren<Transform>();
        foreach (Transform tranform in childTranform)
        {
            cameras.Add(tranform.gameObject);
        }
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

}
