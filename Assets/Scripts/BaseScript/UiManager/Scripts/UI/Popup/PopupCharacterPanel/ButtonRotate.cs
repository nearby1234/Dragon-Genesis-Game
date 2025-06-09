using UnityEngine;
using UnityEngine.UI;

public class ButtonRotate : MonoBehaviour
{
    [SerializeField] private PressingButton leftArrowButton;
    [SerializeField] private PressingButton rightArrowButton;

    private void Update()
    {
        if (RenderManager.HasInstance)
        {
            if (leftArrowButton.IsHeld)
            {
                RenderManager.Instance.RotateModelPlayer.RotateCamera(false);
            }
            else if (rightArrowButton.IsHeld)
            {
                RenderManager.Instance.RotateModelPlayer.RotateCamera(true);
            }
        }
    }
}
