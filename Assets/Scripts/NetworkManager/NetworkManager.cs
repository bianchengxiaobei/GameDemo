using UnityEngine;
using System.Collections;
using System;
using Game;
public class NetworkManager : Singleton<NetworkManager>
{
    private WWW m_wwwRequest = null;
    private float m_fLastTimeForHeartBeat = 0f;
    public static string serverURL = "http://127.0.0.1/LOLGameDemo/serverUrl.xml";
    public static string serverIP = "127.0.0.1";
    public static int serverPort = 8000;
    public CNetProcessor m_netProcessor = null;
    public CNetwork m_network = null;
    private IXLog m_log = XLog.GetLog<NetworkManager>();
    public SocketState NetState
    {
        get
        {
            SocketState result;
            if (null != this.m_network)
            {
                SocketState state = this.m_network.GetState();
                result = state;
            }
            else
            {
                result = SocketState.State_Closed;
            }
            return result;
        }
    }
    public void Init()
    {
        CRegister.RegistProtocol();
        CNetObserver cNetObserver = new CNetObserver();
        this.m_netProcessor = new CNetProcessor();
        this.m_netProcessor.Observer = cNetObserver;
        cNetObserver.oProc = this.m_netProcessor;
        this.m_network = new CNetwork();
        CPacketBreaker oBreaker = new CPacketBreaker();
        string newFullPath = SystemConfig.ResourceFolder;
        if (!this.m_network.Init(this.m_netProcessor, oBreaker, 65536u, 65536u, newFullPath, true))
        {
            this.m_log.Fatal("oNet.Init Error");
        }
        else
        {
            this.m_log.Debug("oNet.Init Success!");
            this.m_netProcessor.Network = this.m_network;
        }
    }
    public void FixedUpdate()
    {
        try
        {
            if (this.m_netProcessor != null && null != this.m_netProcessor.Network)
            {
                this.m_netProcessor.Network.ProcessMsg();
                //this.m_netProcessor.Network.CheckHeartBeat(CommonDefine.IsMobilePlatform);
            }
        }
        catch (Exception ex)
        {
            this.m_log.Fatal(ex.ToString());
        }
    }
    private IEnumerator ConnectToURL()
    {
        Debug.Log("ConnectToURL:" + NetworkManager.serverURL);
        this.m_wwwRequest = new WWW(NetworkManager.serverURL);
        bool flag = false;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (this.m_wwwRequest.isDone)
            {
                if (string.IsNullOrEmpty(this.m_wwwRequest.error))
                {
                    if (!string.IsNullOrEmpty(this.m_wwwRequest.text))
                    {
                        string[] array = this.m_wwwRequest.text.Split(new char[]
						{
							':'
						});
                        if (array.Length == 2)
                        {
                            NetworkManager.serverIP = array[0];//游戏网关服务器ip地址
                            NetworkManager.serverPort = Convert.ToInt32(array[1]);
                            flag = true;
                            this.RealConnectToServer();
                        }
                        else
                        {
                            Debug.LogError("m_wwwRequest.text=:" + this.m_wwwRequest.text);
                        }
                    }
                }
                else
                {
                    Debug.LogError(this.m_wwwRequest.error.ToString());
                }
                break;
            }
        }
        if (null != this.m_wwwRequest)
        {
            this.m_wwwRequest.Dispose();
            this.m_wwwRequest = null;
        }
        if (!flag)
        {
            /*if (Singleton<Login>.singleton.IsLogined)
            {
                Singleton<ReConnect>.singleton.OnConnectToURLFailed();
            }
            else
            {
                Singleton<Login>.singleton.OnConnectToURLFailed();
            }*/
        }
        yield break;
    }
    private void RealConnectToServer()
    {
        this.m_log.Info(string.Format("ConnectToServer:{0}:{1}", NetworkManager.serverIP, NetworkManager.serverPort));
        if (!this.m_netProcessor.Network.Connect(NetworkManager.serverIP, NetworkManager.serverPort))
        {
            Debug.Log("Failed");
            /*if (!Singleton<Login>.singleton.IsLogined)
            {
                Singleton<Login>.singleton.OnConnectFailed();
            }
            else
            {
                Singleton<ReConnect>.singleton.OnConnectFailed();
            }*/
        }
        else
        {
            this.m_log.Info("Connecting...");
        }
    }
    public void ConnectToServer()
    {
        if (string.IsNullOrEmpty(NetworkManager.serverURL))
        {
            this.m_log.Error("serverURL IsNullOrEmpty");
        }
        LOLGameDriver.Instance.StartCoroutine(this.ConnectToURL());
    }
    public void SendMsg(CProtocol ptc)
    {
        try
        {
            if (null != this.m_netProcessor)
            {
                this.m_netProcessor.Send(ptc);
            }
        }
        catch (Exception ex)
        {
            this.m_log.Fatal(ex.ToString());
        }
    }
    public void Close()
	{
		Debug.Log("NetWorkManager::Close");
		if (null != this.m_network)
		{
			this.m_network.Close();
		}
		if (null != this.m_wwwRequest)
		{
			this.m_wwwRequest.Dispose();
			this.m_wwwRequest = null;
		}
	}
    public void UnInit()
    {
        Debug.Log("NetWorkManager::OnApplicationQuit");
        if (null != this.m_network)
        {
            this.m_network.UnInit();
            this.m_network = null;
        }
        this.m_netProcessor = null;
    }
    /// <summary>
    /// 发送登陆请求
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="serverId"></param>
    public void SendLogin(string username, string password, int serverId)
    {
        CptcC2GReq_Login instance = new CptcC2GReq_Login() 
        {
            m_strUsername = username,
            m_strPassword = password,
            m_dwServerId = serverId
        };
        SendMsg(instance);
    }
}