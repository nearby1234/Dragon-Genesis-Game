using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Call Funtion of Class", story: "Call [Funtion] of [Class]", category: "Action", id: "62109fbb3988e7f0941683ee79219ada")]
public partial class CallFuntionOfClassAction : Action
{
    [SerializeReference]public  BlackboardVariable<String> Funtion;
    [SerializeReference]public  BlackboardVariable<MonoBehaviour> Class;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Class?.Value == null || string.IsNullOrEmpty(Funtion?.Value))
        {
            Debug.LogWarning("Class hoặc Funtion chưa được thiết lập");
            return Status.Failure;
        }

        var method = Class.Value.GetType().GetMethod(Funtion.Value,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

        if (method == null)
        {
            Debug.LogWarning($"Không tìm thấy hàm '{Funtion.Value}' trong class '{Class.Value.GetType().Name}'");
            return Status.Failure;
        }

        try
        {
            method.Invoke(Class.Value, null);
            return Status.Success;
        }
        catch (Exception e)
        {
            Debug.LogError($"Gọi hàm '{Funtion.Value}' thất bại: {e.Message}");
            return Status.Failure;
        }
    }

    protected override void OnEnd()
    {
    }
}

