using UnityEngine;

[System.Serializable]
public class DialogSaveData : IKeyed<DialogMission>
{
    public DialogMission dialogKey;
    public bool isClickAcceptButton;
    public bool isClickDenyButton;

    public DialogMission Key => dialogKey;

    public static DialogSaveData FromSO(DialogSystemSO so)
    {

        var data = new DialogSaveData
        {
            dialogKey = so.dialogMission,
            isClickAcceptButton = so.isClickAcceptButton,
            isClickDenyButton = so.isClickDenyButton
            
        };
        return data;
        

    }
    public static void OverwriteToSO(DialogSaveData data, DialogSystemSO so)
    {
        so.dialogMission = data.dialogKey;
        so.isClickAcceptButton = data.isClickAcceptButton;
        so.isClickDenyButton = data.isClickDenyButton;
    }
}

