using UnityEngine;
using UnityEngine.UI;

public class PopupInstructions : BasePopup
{
    [SerializeField] private Button exitBtn;

    private void Start()
    {
        exitBtn.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnExitButtonClicked()
    {
        Hide();
    }
}
