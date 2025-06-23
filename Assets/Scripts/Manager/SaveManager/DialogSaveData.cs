using UnityEngine;

[System.Serializable]
public class DialogSaveData : ISaveKeyed<DialogMission>
{
    public DialogMission dialogKey;
    public DialogState dialogState;
    //public bool isClickAcceptButton;
    //public bool isClickDenyButton;

    public DialogMission Key => dialogKey;

    public static DialogSaveData FromSO(DialogSystemSO so)
    {

        var data = new DialogSaveData
        {
            dialogKey = so.dialogMission,
            //isClickAcceptButton = so.isClickAcceptButton,
            //isClickDenyButton = so.isClickDenyButton
            //dialogState = so.currentDialogState,

        };
        return data;
        

    }
    public static void OverwriteToSO(DialogSaveData data, DialogSystemSO so)
    {
        so.dialogMission = data.dialogKey;
        //so.isClickAcceptButton = data.isClickAcceptButton;
        //so.isClickDenyButton = data.isClickDenyButton;
        //so.currentDialogState = data.dialogState;
    }
}

