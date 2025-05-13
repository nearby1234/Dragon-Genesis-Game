using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager<T> : MonoBehaviour where T : BaseManager<T>
{
    private static T instance;
    private static bool isInitialized = false;

    public static T Instance
    {
        get
        {
            if (!isInitialized)
            {
                instance = FindFirstObjectByType<T>();
                isInitialized = true;

                if (instance == null)
                {
                    Debug.LogWarning($"No {typeof(T).Name} Singleton Instance");
                }
            }
            return instance;
        }
    }

    public static bool HasInstance => instance != null;

    protected virtual void Awake()
    {
        if (CheckInstance())
        {
            isInitialized = true;
        }
    }

    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(this.gameObject);
            return true;
        }
        else if (instance == this)
        {
            return true;
        }

        Destroy(this.gameObject);
        return false;
    }
}
