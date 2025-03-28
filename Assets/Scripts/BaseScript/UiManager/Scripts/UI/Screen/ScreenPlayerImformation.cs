using UnityEngine;

public class ScreenPlayerImformation: BaseScreen
{
    public override void Show(object data)
    {
        base.Show(data);
    }

    public override void Hide()
    {
        base.Hide();
    }
    public override void Init()
    {
        base.Init();
    }

    public void ShowScreenPlayerImfor()
    {
        if(UIManager.HasInstance)
        {
            UIManager.Instance.ShowScreen<ScreenPlayerImformation>();
        }
    }
}
