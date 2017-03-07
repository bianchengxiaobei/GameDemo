using UnityEngine;
using System.Collections;
using Game.Common;
using Utility;
using System;
/// <summary>
/// 登陆界面UI
/// </summary>
public class LoginWindow : BaseWindow
{
    #region 登陆界面
    private UIInput m_Input_UsernameInput;
    private UIInput m_Input_PasswordInput;
    private UIButton m_Button_EnterGame;
    #endregion
    //-------Select Panel-------//
    #region 选择服务器界面
    private UIPanel m_Panel_Select;//选择服务器界面整体Panel
    private UIButton m_Button_Select;//确认选择按钮
    private UIButton m_Button_LeftButton;
    private UIButton m_BUtton_RightButton;
    private UIButton m_Button_ServerListButton;//服务器列表按钮
    private XUIList m_List_Dianxin;
    private XUIList m_List_Wangtong;
    private XUISprite m_Sprite_IconBG;
    private XUISprite m_Sprite_ServerName;
    private XUISprite m_Sprite_Icon;
    private XUILabel m_Label_ServerName;
    private XUILabel m_Label_NetworkSpeed;//测试速度Label
    #endregion
    #region 变量定义
    private bool m_bShowServerList = false;//是否显示服务器列表
    private bool m_bHasLoadedServerList = false;//是否已经加载过服务器列表
    private int m_selectedServerId = -1;
    private int m_lastSelectServerId = SystemConfig.SelectedServerIndex;
    private int m_reDianxinServerId = -1;//电信推荐服务器id
    private int m_reWangtongServerId = -1;//网通推荐服务器idn
    #endregion
    public LoginWindow()
    {
        this.mResName = "Guis/LoginWindow";
        this.mResident = false;
    }
    public override void Init()
    {
        EventCenter.AddListener(EGameEvent.eGameEvent_LoginEnter, Show);
        EventCenter.AddListener(EGameEvent.eGameEvent_LoginExit, Hide);
    }
    public override void Realse()
    {
        EventCenter.RemoveListener(EGameEvent.eGameEvent_LoginEnter, Show);
        EventCenter.RemoveListener(EGameEvent.eGameEvent_LoginExit,Hide);
    }
    protected override void InitWidget()
    {
        this.m_Input_UsernameInput = this.mRoot.FindChild("Login/UserNameInput").GetComponent<UIInput>();
        this.m_Input_PasswordInput = this.mRoot.FindChild("Login/PasswordInput").GetComponent<UIInput>();
        this.m_Button_EnterGame = this.mRoot.FindChild("Login/LoginButton").GetComponent<UIButton>();

        this.m_Button_ServerListButton = this.mRoot.FindChild("Select/Button/ServerListButton").GetComponent<UIButton>();
        this.m_Button_Select = this.mRoot.FindChild("Select/Button/SelectButton").GetComponent<UIButton>();
        this.m_Panel_Select = this.mRoot.FindChild("Select").GetComponent<UIPanel>();
        this.m_List_Dianxin = this.mRoot.FindChild("Select/ServerList/Table/Dianxin/DianxinGrid").GetComponent<XUIList>();
        this.m_List_Wangtong = this.mRoot.FindChild("Select/ServerList/Table/Wangtong/WangtongGrid").GetComponent<XUIList>();
        this.m_Sprite_IconBG = this.mRoot.FindChild("Select/Signal/IconBG").GetComponent<XUISprite>();
        this.m_Sprite_Icon = this.mRoot.FindChild("Select/Signal/IconAnim").GetComponent<XUISprite>();
        this.m_Sprite_ServerName = this.mRoot.FindChild("Select/Signal/Name/ServerNameIcon").GetComponent<XUISprite>();
        this.m_List_Dianxin.RegisterListOnClickEventHandler(new ListOnClickEventHandler(this.OnServerListItemClick));
        UIEventListener.Get(this.m_Button_EnterGame.gameObject).onClick += OnLoginSumbit;
        UIEventListener.Get(this.m_Button_Select.gameObject).onClick += OnLoginServer;
        UIEventListener.Get(this.m_Button_ServerListButton.gameObject).onClick = (x) => { this.m_bShowServerList = !this.m_bShowServerList; ShowSelectServer(); };
    }
    protected override void RealseWidget()
    {
        
    }
    protected override void OnAddListener()
    {
        EventCenter.AddListener(EGameEvent.eGameEvent_ShowSelectServerList, ShowSelectServer);
    }
    protected override void OnRemoveListener()
    {
        EventCenter.RemoveListener(EGameEvent.eGameEvent_ShowSelectServerList, Show);
    }
    public override void OnEnable()
    {
        
    }
    public override void OnDisable()
    {
        
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }
    public void OnLoginSumbit(GameObject go)
    {
        string username = this.m_Input_UsernameInput.value;
        string password = this.m_Input_PasswordInput.value;
        //如果用户名或者密码为空的话，就显示提示消息
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            CEvent evt = new CEvent(EGameEvent.eGameEvent_ShowMessage);
            evt.AddParam("content", StringConfigManager.GetString("MessageWindow.EMT_SureTip.LoginNullUsernameOrPassword"));
            EventCenter.SendEvent<EMessageType, Action<bool>>(evt, EMessageType.EMT_SureTip, (isOk) => 
            {
                EventCenter.Broadcast(EGameEvent.eGameEvent_HideMessage);
            });
        }
        LoginCtrl.singleton.Login(username, password);
    }
    /// <summary>
    /// 登陆游戏服务器
    /// </summary>
    /// <param name="go"></param>
    public void OnLoginServer(GameObject go)
    {
        if (this.m_selectedServerId >= 0)
        {
            LoginCtrl.singleton.LoginStart(this.m_selectedServerId);
        }
        else 
        {
            Debug.LogError("选择服务器不存在");
        }
    }
    /// <summary>
    /// 显示选择服务器界面
    /// </summary>
    public void ShowSelectServer()
    {
        if (this.m_Panel_Select != null)
        {
            if (this.ShowServerList())
            {
                //服务器列表按钮的sprite替换
                if (this.m_bShowServerList)
                {
                    this.m_Button_ServerListButton.normalSprite = "image 378";
                }
                else 
                {
                    this.m_Button_ServerListButton.normalSprite = "image 383";
                }
                this.m_Panel_Select.enabled = true;
                GameObject serverList = this.m_Panel_Select.transform.FindChild("ServerList").gameObject;
                //serverList.
                serverList.SetActive(this.m_bShowServerList);
                //如果显示服务器列表面板，播放偏移动画
                if (this.m_bShowServerList)
                {
                    serverList.transform.localPosition = new Vector3(14, 0, 0);
                    TweenAlpha.Begin(serverList, 1, 1);
                    TweenPosition.Begin(serverList, 1, Vector3.zero);
                }
            }
            else 
            {
                Debug.LogError("服务器列表还没有初始化");
            }
        }
    }
    /// <summary>
    /// 显示服务器列表
    /// </summary>
    public bool ShowServerList()
    {
        if (this.m_bHasLoadedServerList)
        {
            return true;
        }
        if (SystemConfig.ServerList != null)
        {
            this.m_selectedServerId = SystemConfig.SelectedServerIndex;
            this.m_lastSelectServerId = SystemConfig.SelectedServerIndex;
            int indexDianxin = 0;
            int indexWangtong = 0;
            foreach (var serverInfo in SystemConfig.ServerList)
            {
                IXUIListItem serverItem;
                switch (serverInfo.type)
                {
                    case 0:
                        if (serverInfo.flag == (int)ServerType.Recommend)
                        {
                            this.m_reDianxinServerId = serverInfo.id;
                        }
                        if (indexDianxin < this.m_List_Dianxin.Count)
                        {
                            serverItem = this.m_List_Dianxin.GetItemByIndex(indexDianxin);
                        }
                        else
                        {
                            serverItem = this.m_List_Dianxin.AddListItem();
                        }
                        if (serverItem != null)
                        {
                            serverItem.SetText("ServerName",serverInfo.name);
                            serverItem.SetVisible(true);
                            serverItem.TipParam = new TipParam
                            {
                                Tip = serverItem.Tip
                            };
                            serverItem.Id = serverInfo.id;
                        }
                        else
                        {
                            serverItem.SetVisible(false);
                        }
                        indexDianxin++;
                        break;
                    case 1:
                        if (serverInfo.flag == (int)ServerType.Recommend)
                        {
                            this.m_reWangtongServerId = serverInfo.id;
                        }
                        if (indexWangtong < this.m_List_Wangtong.Count)
                        {
                            serverItem = this.m_List_Wangtong.GetItemByIndex(indexWangtong);
                        }
                        else
                        {
                            serverItem = this.m_List_Wangtong.AddListItem();
                        }
                        if (serverItem != null)
                        {
                            serverItem.SetText("ServerName", serverInfo.name);
                            serverItem.SetVisible(true);
                            serverItem.TipParam = new TipParam 
                            {
                                Tip = serverItem.Tip
                            };
                            serverItem.Id = serverInfo.id;
                        }
                        else
                        {
                            serverItem.SetVisible(false);
                        }
                        indexWangtong++;
                        break;
                }
            }
            this.m_bHasLoadedServerList = true;
            return true;
        }
        else 
        {
            return false;
        }
    }
    /// <summary>
    /// 服务器某个被点击
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool OnServerListItemClick(IXUIListItem item)
    {
        if (null == item)
        {
            return false;
        }
        this.m_selectedServerId = item.Id;
        ServerInfo info = SystemConfig.GetServerInfoById(this.m_selectedServerId);
        bool active = true;
        if (info.flag == (int)ServerType.Close || info.flag == (int)ServerType.Maintain)
        {
            active = false;
        }
        ShowServerSignal(active, info);
        return true;
    }
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="bActive"></param>
    /// <param name="info"></param>
    private void ShowServerSignal(bool bActive,ServerInfo info)
    {
        if (bActive)
        {
            if (info.id == 0)
            {
                this.m_Sprite_Icon.SetSprite("image 967", "Atlas/SelectAtlas/SelectServerAtlas");
                this.m_Sprite_ServerName.SetSprite("image 975");
            }
            else 
            {
                this.m_Sprite_Icon.SetSprite("image 1015", "Atlas/SelectAtlas/SelectServerAtlas");
                this.m_Sprite_ServerName.SetSprite("image 1012");
            }
            if (this.m_Sprite_IconBG != null)
            {
                this.m_Sprite_IconBG.PlayFlash(false);
                this.m_Sprite_ServerName.Alpha = 0f;
                this.m_Sprite_Icon.Alpha = 0f;
                TweenAlpha.Begin(this.m_Sprite_ServerName.gameObject, 0.8f, 1f);
                TweenAlpha.Begin(this.m_Sprite_Icon.gameObject, 0.8f, 1f);          
            }
        }
    }
}
