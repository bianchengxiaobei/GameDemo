using UnityEngine;
using System.Collections.Generic;
using Game;
using Game.Common;
public class WindowManager : Singleton<WindowManager>
{
    private Dictionary<EWindowType, BaseWindow> mWidowDic;
    public WindowManager()
    {
        mWidowDic = new Dictionary<EWindowType, BaseWindow>();
        mWidowDic[EWindowType.e_MessageWindow] = new MessageWindow();
        mWidowDic[EWindowType.e_LoginWindow] = new LoginWindow();
    }
    public void Init()
    {
        foreach (var pWindow in this.mWidowDic.Values)
        {
            pWindow.Init();
            if (pWindow.IsResident())
            {
                pWindow.PreLoad();
            }
        }
    }
    public void Update(float deltaTime)
    {
        foreach (BaseWindow pWindow in mWidowDic.Values)
        {
            if (pWindow.IsVisible())
            {
                pWindow.Update(deltaTime);
            }
        }
    }
}
