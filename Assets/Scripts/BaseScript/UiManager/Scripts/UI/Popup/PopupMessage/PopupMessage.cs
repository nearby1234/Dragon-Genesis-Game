using System;
using Unity.VisualScripting;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

public enum PopupType
{
    Confirm,
    OK,
    WIN,
    LOSE,
    PAUSE,
}
public class PopupMessage
{
    public PopupType popupType;
    public string titleWin = "WIN";
    public string titleLose = "LOSE";
    public string titlePause = "PAUSE";
    public string titlePlayAgain = "Play Again";
    public string titleResume = "Resume";
    public string titleMainMenu = "Main Menu";
    public string hexColorLose = "#FF0000";
    public string hexColorWin = "#002FFF";
    public Color TitleLoseColor => ColorUtility.TryParseHtmlString(hexColorLose, out var color) ? color : Color.red;


    public Color TitleWinColor => ColorUtility.TryParseHtmlString(hexColorWin, out var color) ? color : Color.blue;


    public Action OnPlayAgain;
    public Action OnMainMenu;
    public Action OnExit;
    public Action OnResume;
} 
