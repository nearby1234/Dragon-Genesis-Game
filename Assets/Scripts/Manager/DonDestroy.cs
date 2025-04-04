using UnityEngine;

public class DonDestroy : MonoBehaviour
{
    private static DonDestroy instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;  // L?u instance ??u tiên
            DontDestroyOnLoad(gameObject);  // Gi? object không b? destroy
        }
        else
        {
            Destroy(gameObject);  // Xóa object m?i t?o n?u ?ã có instance
        }
    }
}
