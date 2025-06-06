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
    WORM_DIE,
    BULLTANK_DIE,
}
public class PopupMessage
{
    public PopupType popupType;
    public string WormDie = "Sâu Hắc Ám";
    public string BullTankDie = "Hắc Ngưu Ma Vương";
    public string nameState = "TIÊU DIỆT";
    public string titleLose = "LOSE";
    public string titlePause = "PAUSE";
    public string titlePlayAgain = "Play Again";
    public string titleResume = "Resume";
    public string titleMainMenu = "Main Menu";
    public string hexColorLose = "#FF0000";
    public string hexColorWin = "#002FFF";
    public string hexColorPause = "#FF0000";
    public Color TitleLoseColor => ColorUtility.TryParseHtmlString(hexColorLose, out var color) ? color : Color.red;
    public Color TitleWinColor => ColorUtility.TryParseHtmlString(hexColorWin, out var color) ? color : Color.blue;
    public Color TitlePauseColor => ColorUtility.TryParseHtmlString(hexColorPause, out var color) ? color : Color.red;


    public Action OnPlayAgain;
    public Action OnMainMenu;
    public Action OnExit;
    public Action OnResume;
} 
