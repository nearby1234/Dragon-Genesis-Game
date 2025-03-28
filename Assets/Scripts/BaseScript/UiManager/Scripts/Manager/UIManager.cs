using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    public GameObject cScreen, cPopup, cNotify;

    private Dictionary<string, BaseScreen> screens = new Dictionary<string, BaseScreen>();
    private Dictionary<string, BasePopup> popups = new Dictionary<string, BasePopup>();
    private Dictionary<string, BaseNotify> notifies = new Dictionary<string, BaseNotify>();

    private List<string> rmScreens = new List<string>();
    private List<string> rmPopups = new List<string>();
    private List<string> rmNotifies = new List<string>();

    private const string SCREEN_RESOURCES_PATH = "Prefabs/UI/Screen/";
    private const string POPUP_RESOURCES_PATH = "Prefabs/UI/Popup/";
    private const string NOTIFY_RESOURCES_PATH = "Prefabs/UI/Notify/";

    private BaseScreen curScreen;
    private BasePopup curPopup;
    private BaseNotify curNotify;

    public Dictionary<string, BaseScreen> Screens => screens;
    public Dictionary<string, BasePopup> Popups => popups;
    public Dictionary<string, BaseNotify> Notifies => notifies;
    public BaseScreen CurScreen => curScreen;
    public BasePopup CurPopup => curPopup;
    public BaseNotify CurNotify => curNotify;


    #region Screen
    public void ShowScreen<T>(object data = null, bool forceShowData = false) where T: BaseScreen
    {
        string nameScreen = typeof(T).Name;
        BaseScreen result = null;

        if(curScreen != null)
        {
            var curName = curScreen.GetType().Name;
            if (curName.Equals(nameScreen))
            {
                result = curScreen;
            }
            else
            {
                RemoveScreen(curName);
            }
        }

        if(result == null)
        {
            if (!screens.ContainsKey(nameScreen))
            {
                BaseScreen screenScr = GetNewScreen<T>();
                if (screenScr != null)
                {
                    screens.Add(nameScreen, screenScr);
                }
            }

            if (screens.ContainsKey(nameScreen))
            {
                result = screens[nameScreen];
            }
        }

        bool isShow = false;
        if(result != null)
        {
            if (forceShowData)
            {
                isShow = true;
            }
            else
            {
                if (result.IsHide)
                {
                    isShow = true;
                }
            }
        }

        if (isShow)
        {
            curScreen = result;
            result.transform.SetAsLastSibling();
            result.Show(data);
        }
    }

    private BaseScreen GetNewScreen<T>() where T : BaseScreen
    {
        string nameScreen = typeof(T).Name;
        GameObject pfScreen = GetUIPrefab(UIType.Screen, nameScreen);
        if(pfScreen == null || !pfScreen.GetComponent<BaseScreen>())
        {
            throw new MissingReferenceException("Cant found " + nameScreen + "screen. !!!");
        }
        GameObject ob = Instantiate(pfScreen) as GameObject;
        ob.transform.SetParent(this.cScreen.transform);
        ob.transform.localScale = Vector3.one;
        ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
        ob.name = "SCREEN_" + nameScreen;
#endif
        BaseScreen sceneScr = ob.GetComponent<BaseScreen>();
        sceneScr.Init();
        return sceneScr;
    }

    public void HideAllScreens()
    {
        BaseScreen screenScr = null;
        foreach (KeyValuePair<string, BaseScreen> item in screens)
        {
            screenScr = item.Value;
            if (screenScr == null || screenScr.IsHide)
                continue;
            screenScr.Hide();

            if (screens.Count <= 0)
                break;
        }
    }

    public T GetExistScreen<T>() where T : BaseScreen
    {
        string nameScreen = typeof(T).Name;
        if (screens.ContainsKey(nameScreen))
        {
            return screens[nameScreen] as T;
        }
        return null;
    }

    private void RemoveScreen(string v)
    {
        for (int i = 0; i < rmScreens.Count; i++)
        {
            if (rmScreens[i].Equals(v))
            {
                if (screens.ContainsKey(v))
                {
                    Destroy(screens[v].gameObject);
                    screens.Remove(v);

                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }
                break;
            }
        }
    }
    #endregion
    #region Popup
    public void ShowPopup<T>(object data = null, bool forceShowData = false) where T : BasePopup
    {
        string namePopup = typeof(T).Name;
        BasePopup result = null;

        if (curPopup != null)
        {
            var curName = curPopup.GetType().Name;
            if (curName.Equals(namePopup))
            {
                result = curPopup;
            }
            else
            {
                RemovePopup(curName);
            }
        }

        if (result == null)
        {
            if (!popups.ContainsKey(namePopup))
            {
                BasePopup popupScr = GetNewPopup<T>();
                if (popupScr != null)
                {
                    popups.Add(namePopup, popupScr);
                }
            }

            if (popups.ContainsKey(namePopup))
            {
                result = popups[namePopup];
            }
        }

        bool isShow = false;
        if (result != null)
        {
            if (forceShowData)
            {
                isShow = true;
            }
            else
            {
                if (result.IsHide)
                {
                    isShow = true;
                }
            }
        }

        if (isShow)
        {
            curPopup = result;
            result.transform.SetAsLastSibling();
            result.Show(data);
        }
    }

    private BasePopup GetNewPopup<T>() where T : BasePopup
    {
        string namePopup = typeof(T).Name;
        GameObject pfPopup = GetUIPrefab(UIType.Popup, namePopup);
        if (pfPopup == null || !pfPopup.GetComponent<BasePopup>())
        {
            throw new MissingReferenceException("Cant found " + namePopup + "screen. !!!");
        }
        GameObject ob = Instantiate(pfPopup) as GameObject;
        ob.transform.SetParent(this.cPopup.transform);
        ob.transform.localScale = Vector3.one;
        ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
        ob.name = "POPUP_" + namePopup;
#endif
        BasePopup popupScr = ob.GetComponent<BasePopup>();
        popupScr.Init();
        return popupScr;
    }

    public void HideAllPopups()
    {
        BasePopup popupScr = null;
        foreach (KeyValuePair<string, BasePopup> item in popups)
        {
            popupScr = item.Value;
            if (popupScr == null || popupScr.IsHide)
                continue;
            popupScr.Hide();

            if (popups.Count <= 0)
                break;
        }
    }

    public T GetExistPopup<T>() where T : BasePopup
    {
        string namePopup = typeof(T).Name;
        if (popups.ContainsKey(namePopup))
        {
            return popups[namePopup] as T;
        }
        return null;
    }

    private void RemovePopup(string v)
    {
        for (int i = 0; i < rmPopups.Count; i++)
        {
            if (rmPopups[i].Equals(v))
            {
                if (popups.ContainsKey(v))
                {
                    Destroy(popups[v].gameObject);
                    popups.Remove(v);

                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }
                break;
            }
        }
    }
    #endregion
    #region Notify
    public void ShowNotify<T>(object data = null, bool forceShowData = false) where T : BaseNotify
    {
        string nameNotify = typeof(T).Name;
        BaseNotify result = null;

        if (curNotify != null)
        {
            var curName = curNotify.GetType().Name;
            if (curName.Equals(nameNotify))
            {
                result = curNotify;
            }
            else
            {
                RemoveNotify(curName);
            }
        }

        if (result == null)
        {
            if (!notifies.ContainsKey(nameNotify))
            {
                BaseNotify notifyScr = GetNewNotify<T>();
                if (notifyScr != null)
                {
                    notifies.Add(nameNotify, notifyScr);
                }
            }

            if (notifies.ContainsKey(nameNotify))
            {
                result = notifies[nameNotify];
            }
        }

        bool isShow = false;
        if (result != null)
        {
            if (forceShowData)
            {
                isShow = true;
            }
            else
            {
                if (result.IsHide)
                {
                    isShow = true;
                }
            }
        }

        if (isShow)
        {
            curNotify = result;
            result.transform.SetAsLastSibling();
            result.Show(data);
        }
    }

    private BaseNotify GetNewNotify<T>() where T : BaseNotify
    {
        string nameNotify = typeof(T).Name;
        GameObject pfNotify = GetUIPrefab(UIType.Notify, nameNotify);
        if (pfNotify == null || !pfNotify.GetComponent<BaseNotify>())
        {
            throw new MissingReferenceException("Cant found " + nameNotify + "screen. !!!");
        }
        GameObject ob = Instantiate(pfNotify) as GameObject;
        ob.transform.SetParent(this.cNotify.transform);
        ob.transform.localScale = Vector3.one;
        ob.transform.localPosition = Vector3.zero;
#if UNITY_EDITOR
        ob.name = "NOTIFY_" + nameNotify;
#endif
        BaseNotify notifyScr = ob.GetComponent<BaseNotify>();
        notifyScr.Init();
        return notifyScr;
    }

    public void HideAllNotify()
    {
        BaseNotify notifyScr = null;
        foreach (KeyValuePair<string, BaseNotify> item in notifies)
        {
            notifyScr = item.Value;
            if (notifyScr == null || notifyScr.IsHide)
                continue;
            notifyScr.Hide();

            if (notifies.Count <= 0)
                break;
        }
    }

    public T GetExistNotify<T>() where T : BaseNotify
    {
        string nameNotify = typeof(T).Name;
        if (notifies.ContainsKey(nameNotify))
        {
            return notifies[nameNotify] as T;
        }
        return null;
    }

    private void RemoveNotify(string v)
    {
        for (int i = 0; i < rmNotifies.Count; i++)
        {
            if (rmNotifies[i].Equals(v))
            {
                if (notifies.ContainsKey(v))
                {
                    Destroy(notifies[v].gameObject);
                    notifies.Remove(v);

                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }
                break;
            }
        }
    }
    #endregion

    private GameObject GetUIPrefab(UIType t, string uiName)
    {
        GameObject result = null;
        var defaultPath = "";
        if(result == null)
        {
            switch (t)
            {
                case UIType.Screen:
                    {
                        defaultPath = SCREEN_RESOURCES_PATH + uiName;
                    }
                    break;
                case UIType.Popup:
                    {
                        defaultPath = POPUP_RESOURCES_PATH + uiName;
                    }
                    break;
                case UIType.Notify:
                    {
                        defaultPath = NOTIFY_RESOURCES_PATH + uiName;
                    }
                    break;
            }

            result = Resources.Load(defaultPath) as GameObject;
        }

        return result;
    }
}
