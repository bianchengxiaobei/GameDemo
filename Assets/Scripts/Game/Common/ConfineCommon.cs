using UnityEngine;
using System.Collections;
namespace Game.Common 
{
    public delegate void Callback();
    public delegate void Callback<T>(T arg1);
    public delegate void Callback<T, U>(T arg1, U arg2);
    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
    public delegate void Callback<T, U, V, X>(T arg1, U arg2, V arg3, X arg4);

    public delegate bool ListSelectEventHandler(IXUIListItem listItem);
    public delegate bool ListOnClickEventHandler(IXUIListItem listItem);
    /// <summary>
    /// UI界面类型
    /// </summary>
    public enum EWindowType
    {
        e_LoginWindow,
        e_MessageWindow
    }
    /// <summary>
    /// 资源类型，Asset,Prefab,Level
    /// </summary>
    public enum ResourceType
    {
        ASSET,
        PREFAB,
        LEVELASSET,
        LEVEL,
    }
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum EMessageType
    {
        EMT_None = -1,
        EMT_UpdateDownload, //更新下载提示
        EMT_Single, //单个按钮
        EMT_Double, //多个按钮
        EMT_Tip,     //提示
        EMT_SureTip //确定提示
    }
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum EGameEvent
    {
        #region 消息事件
        eGameEvent_ShowMessage, //显示MessageBox
        eGameEvent_HideMessage, //隐藏消息
        #endregion
        #region 登陆事件
        eGameEvent_LoginEnter,
        eGameEvent_LoginExit,
        eGameEvent_ShowSelectServerList
        #endregion
    }
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameStateType
    {
        GS_Continue,//中间状态，也就是说介于两个状态转换的中间状态
        GS_Login,//登陆状态
        GS_User,//创建用户状态
        GS_Lobby,//大厅，类似于LOL主界面状态
        GS_Room,//创建房间状态
        GS_Hero,//选择英雄
        GS_Loading,//加载状态
        GS_Play,//对战状态
        GS_Over,//对战结束
    }
    /// <summary>
    /// 调试级别
    /// </summary>
    public enum EnumLogLevel
    {
        eLogLevel_Debug,
        eLogLevel_Info,
        eLogLevel_Error,
        eLogLevel_Fatal,
        eLogLevel_Max
    }
    /// <summary>
    /// 服务器类型
    /// </summary>
    public enum ServerType : int
    {
        Hot = 1,
        Normal = 2,
        Close = 3,
        Maintain = 4,//维护
        Recommend = 5//推荐
    }
    /// <summary>
    /// 服务器运营类型
    /// </summary>
    public enum ServerBusType 
    {
        Dianxin,   //电信
        Wangtong    //网通
    }
    /// <summary>
    /// 提示类型
    /// </summary>
    public enum TipEnumType 
    {
        eTipType_Common,//普通
        eTipType_Title//标题
    }
}
