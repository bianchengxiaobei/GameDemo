using UnityEngine;
using System.Collections;
using Game;
using Game.Common;
/// <summary>
/// 登陆控制器
/// </summary>
public class LoginCtrl : Singleton<LoginCtrl>
{
    private string username;
    private string password;
    private IXLog m_log = XLog.GetLog<LoginCtrl>();
    public void Enter()
    {
        EventCenter.Broadcast(EGameEvent.eGameEvent_LoginEnter);
    }
    public void Exit()
    {
        EventCenter.Broadcast(EGameEvent.eGameEvent_LoginExit);
    }
    public void Login(string account, string password)
    {
        this.username = account;
        this.password = password;
        LOLGameDriver.Instance.StartCoroutine(CheckUserPass());
    }
    IEnumerator CheckUserPass()
    {
        if (string.IsNullOrEmpty(this.username) || string.IsNullOrEmpty(this.password))
        {
            yield break;
        }
        WWW www = new WWW("http://127.0.0.1/LOLGameDemo/check.php");
        bool success = false;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    if (!string.IsNullOrEmpty(www.text))
                    {
                        if (www.text == "success")
                        {
                            //加载服务器信息
                            SystemConfig.LoadServerList();
                            success = true;
                            EventCenter.Broadcast(EGameEvent.eGameEvent_ShowSelectServerList);
                        }
                        else 
                        {
                            //登陆失败
                        }
                    }
                }
                else 
                {
                    this.m_log.Error(www.error.ToString());
                }
                break;
            }
        }
        if (www != null)
        {
            www.Dispose();
            www = null;
        }
        //如果不成功
        if (!success)
        {
 
        }
        yield break;
    }
    public void LoginStart(int serverId)
    {
        //如果已经连接上，就直接发送登陆消息请求
        if (Singleton<NetworkManager>.singleton.NetState == SocketState.State_Connected)
        {
            //NetworkManager.singleton.SendLogin(this.username, this.password, serverId);
            SystemConfig.SelectedServerIndex = serverId;//保存上次选择的服务器id
            SystemConfig.LocalSetting.SelectedServer = serverId;//保存到本地配置
            //发送登陆消息请求
            NetworkManager.singleton.SendLogin(this.username,this.password,serverId);
        }
        else 
        {
            Singleton<NetworkManager>.singleton.ConnectToServer();//开始连接服务器
        }
    }
}
