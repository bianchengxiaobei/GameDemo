using UnityEngine;
using System.Collections;
using System;
using Game.Common;
/// <summary>
/// 消息UI
/// </summary>
public class MessageWindow : BaseWindow
{
    private EMessageType m_eMessageType = EMessageType.EMT_None;
    private UISprite m_frame;//消息背景
    private UILabel m_title;//消息标题
    private UILabel m_content;//消息内容
    private UIButton m_firstButton;//消息第一个按钮
    private UIButton m_secondButton;//消息第二个按钮
    private UIButton m_centerButton;//中间按钮
    private Action<bool> m_actCallBack;//委托回调
    public MessageWindow()
    {
        mResName = "Guis/MessageWindow";
        mResident = false;
    }
    public override void Init()
    {
        EventCenter.AddListener<CEvent,EMessageType,Action<bool>>(EGameEvent.eGameEvent_ShowMessage, ShowMessage);      
    }
    protected override void InitWidget()
    {
        this.m_frame = this.mRoot.FindChild("Frame").GetComponent<UISprite>();
        this.m_title = this.mRoot.FindChild("Frame/Title").GetComponent<UILabel>();
        this.m_content = this.mRoot.FindChild("Frame/Content").GetComponent<UILabel>();
        this.m_firstButton = this.mRoot.FindChild("Frame/FirstButton").GetComponent<UIButton>();
        this.m_secondButton = this.mRoot.FindChild("Frame/SecondButton").GetComponent<UIButton>();
        this.m_centerButton = this.mRoot.FindChild("Frame/CenterButton").GetComponent<UIButton>();
        EventDelegate.Add(this.m_firstButton.onClick, OnFirstBtn);
        EventDelegate.Add(this.m_secondButton.onClick, OnSecondBtn);
        EventDelegate.Add(this.m_centerButton.onClick, OnFirstBtn);
    }
    protected override void OnAddListener()
    {
        EventCenter.AddListener(EGameEvent.eGameEvent_HideMessage, Hide);
    }
    protected override void OnRemoveListener()
    {
        EventCenter.RemoveListener(EGameEvent.eGameEvent_HideMessage, Hide);
    }
    public override void OnEnable()
    {
       
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }
    public override void OnDisable()
    {
        
    }
    protected override void RealseWidget()
    {
        
    }
    public override void Realse()
    {
        EventCenter.RemoveListener<CEvent,EMessageType,Action<bool>>(EGameEvent.eGameEvent_ShowMessage, ShowMessage);
    }
    public void ShowMessage(CEvent evt,EMessageType type,Action<bool> callback = null)
    {
        //如果已经显示了，就直接返回
        if (mVisible && evt.GetParamCount() == 0)
        {
            return;
        }
        this.m_eMessageType = type;
        this.m_actCallBack = callback;
        Show();
        //根据不同的消息类型，显示不同的提示消息
        switch (this.m_eMessageType)
        {
                //如果是重试消息的话
            case EMessageType.EMT_Double:
                this.m_firstButton.gameObject.SetActive(true);
                this.m_secondButton.gameObject.SetActive(true);
                this.m_centerButton.gameObject.SetActive(false);
                this.m_firstButton.normalSprite = StringConfigManager.GetString("MessageWindow.EMT_Double.TryAgainButton");
                this.m_secondButton.normalSprite = StringConfigManager.GetString("MessageWindow.EMT_Double.QuitButton");
                this.m_title.text = evt.GetParam("title") as string;//StringConfigManager.GetString("MessageWindow.EMT_NetTryAgain.Title");
                this.m_content.text = evt.GetParam("content") as string; //StringConfigManager.GetString("MessageWindow.EMT_NetTryAgain.Content");
                break;
            case EMessageType.EMT_Tip:
                this.m_frame.spriteName = StringConfigManager.GetString("EMessageType.EMT_Tip.Frame");
                this.m_firstButton.gameObject.SetActive(false);
                this.m_secondButton.gameObject.SetActive(false);
                this.m_centerButton.gameObject.SetActive(false);
                this.m_title.gameObject.SetActive(false);
                this.m_content.alignment = NGUIText.Alignment.Center;
                this.m_content.text = evt.GetParam("content") as string;//StringConfigManager.GetString("EMessageType.EMT_UpdateTip.Content1");
                break;
            case EMessageType.EMT_SureTip:
                this.m_frame.spriteName = StringConfigManager.GetString("EMessageType.EMT_Tip.Frame");
                this.m_firstButton.gameObject.SetActive(false);
                this.m_secondButton.gameObject.SetActive(false);
                this.m_centerButton.gameObject.SetActive(true);
                this.m_title.gameObject.SetActive(false);
                this.m_content.alignment = NGUIText.Alignment.Center;
                this.m_content.text = evt.GetParam("content") as string;
                break;
            case EMessageType.EMT_UpdateDownload:
                this.m_frame.spriteName = StringConfigManager.GetString("EMessageType.EMT_UpdateTip.Frame");
                this.m_firstButton.gameObject.SetActive(true);
                this.m_secondButton.gameObject.SetActive(true);
                this.m_firstButton.normalSprite = StringConfigManager.GetString("MessageWindow.EMT_UpdateDownload.LookNewButton");
                this.m_secondButton.normalSprite = StringConfigManager.GetString("MessageWindow.EMT_Double.TryAgainButton");
                this.m_title.gameObject.SetActive(false);
                this.m_content.alignment = NGUIText.Alignment.Center;
                this.m_content.text = string.Format(StringConfigManager.GetString("EMessageType.EMT_UpdateDownload.Content"), evt.GetParam("index"), evt.GetParam("total"), evt.GetParam("fileName"));
                break;
            case EMessageType.EMT_None:
                break;
        }
    }
    public void OnFirstBtn()
    {
        switch (this.m_eMessageType)
        {
            //如果是重试消息的话
            case EMessageType.EMT_Double:
                this.m_actCallBack(true);
                Hide();
                break;
            case EMessageType.EMT_None:
                break;
        }
    }
    public void OnSecondBtn()
    {
        switch (this.m_eMessageType)
        {
            //如果是重试消息的话
            case EMessageType.EMT_Double:
                this.m_actCallBack(false);
                    Hide();
                break;
            case EMessageType.EMT_None:
                break;
        }
    }
}
