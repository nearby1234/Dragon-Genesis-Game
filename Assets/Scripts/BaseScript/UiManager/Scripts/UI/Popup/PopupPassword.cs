using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopupPassword : BasePopup
{
    [SerializeField] private Button exitBtn;
    [SerializeField] private TMPro.TMP_InputField passwordInputField;
    [SerializeField] private PlayerStatSO playerStatSO;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        exitBtn.onClick.AddListener(OnExitButtonClicked);
        passwordInputField.onEndEdit.AddListener(OnPasswordInput);
       
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        passwordInputField.characterLimit = playerStatSO.maxLengthPassword;
       
    }

    private void OnExitButtonClicked()
    {
        if (ListenerManager.HasInstance)
        {
            ListenerManager.Instance.BroadCast(ListenType.UI_DISABLE_SHOWUI, null);
        }
        if (AudioManager.HasInstance)
        {
            AudioManager.Instance.PlaySE("ExitSound");
        }
        Hide();
    }
   
    private void OnPasswordInput(string password)
    {
        // Handle password input logic here
        Debug.Log("Password entered: " + password);

        if (UIManager.HasInstance)
        {
            if (password == playerStatSO.passwordAdmin)
            {
                UIManager.Instance.ShowPopup<PopupCheat>();
              
               
                this.Hide();
            }
            else
            {
                animator.Play("Shake");
                NotifyMessageMission<PopupPassword> notifyMessage = new()
                {
                    message = "Mật khẩu không chính xác!",
                };
                UIManager.Instance.ShowNotify<NotifySystem>(notifyMessage,true);
            }
        }

    }
   
}
