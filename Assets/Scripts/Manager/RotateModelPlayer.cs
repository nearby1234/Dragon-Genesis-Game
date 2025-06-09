using System;
using UnityEngine;

public class RotateModelPlayer : MonoBehaviour
{

    [SerializeField] private Transform centerObject;
    [SerializeField] private Transform cameraRotate;
    [SerializeField] private float rotationSpeed = 20f;
    private Vector3 axis = Vector3.up;
   

    public void RotateCamera(bool isInvert)
    {
        if (isInvert)
        {
            cameraRotate.RotateAround(centerObject.position, axis, -rotationSpeed * Time.deltaTime);
        }
        else
        {
            cameraRotate.RotateAround(centerObject.position, axis, rotationSpeed * Time.deltaTime);
        }
    }    
}
