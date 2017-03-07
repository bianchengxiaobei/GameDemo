using UnityEngine;
using System.Collections;
using System;
using Game.Common;
/// <summary>
/// 驱动脚本
/// </summary>
public class LOLGameDriver : MonoBehaviour 
{
    /// <summary>
    /// 静态单例属性
    /// </summary>
    public static LOLGameDriver Instance
    {
        get;
        set;
    }
    void Awake ()
    {
        //如果单例不为空，说明存在两份的单例，就删除一份
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;//初始化单例
        DontDestroyOnLoad(this.gameObject);
        Application.runInBackground = true;//可以在后台运行
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//设置屏幕永远亮着
        Application.targetFrameRate = 60;//这是帧为60
        InvokeRepeating("Tick", 1, 0.02f);
        Init();
        TryInit();
    }
	void Start () 
    {

	}
	void Update () 
    {
        
	}
    void FixedUpdate()
    {
        NetworkManager.singleton.FixedUpdate();
    }
    void Init()
    {
        WindowManager.singleton.Init();//界面UI初始化
        StringConfigManager.singleton.Init();//字符管理器初始化
        NetworkManager.singleton.Init();//网络管理器初始化
    }
    public void TryInit()
    {
        //说明网络可以
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            CheckTimeout checkTimeout = new CheckTimeout();
            checkTimeout.AsynIsNetworkTimeout((result) =>
            {
                //网络良好
                if (result)
                {
                    //开始更新检测
                    DoInit();
                }
                else //说明网络错误
                {
                    Debug.Log("网络错误");
                    //开始消息提示框，重试和退出
                    CEvent evt = new CEvent(EGameEvent.eGameEvent_ShowMessage);
                    evt.AddParam("title", StringConfigManager.GetString("MessageWindow.EMT_NetTryAgain.Title"));
                    evt.AddParam("content", StringConfigManager.GetString("MessageWindow.EMT_NetTryAgain.Content"));
                    EventCenter.SendEvent<EMessageType, Action<bool>>(evt, EMessageType.EMT_Double, (isOk) =>
                    {
                        if (isOk)
                        {
                            TryInit();//重试
                        }
                        else
                        {
                            Application.Quit();//退出
                        }
                    });
                }
            });
        }
        else 
        {
            CEvent evt = new CEvent(EGameEvent.eGameEvent_ShowMessage);
            evt.AddParam("title", StringConfigManager.GetString("MessageWindow.EMT_NetTryAgain.Title"));
            evt.AddParam("content", StringConfigManager.GetString("MessageWindow.EMT_NetTryAgain.Content"));
            EventCenter.SendEvent<EMessageType, Action<bool>>(evt, EMessageType.EMT_Double, (isOk) =>
            {
                if (isOk)
                {
                    TryInit();//重试
                }
                else
                {
                    Application.Quit();//退出
                }
            });
        }
    }
    public void DoInit()
    {
        bool result = SystemConfig.Init();
        if (result)
        {
            VersionManager.singleton.Init();
            VersionManager.singleton.LoadLocalVersion();
            CheckVersion(CheckVersionFinished);
        }
        else 
        {
            Debug.Log("系统配置出问题！！");
        }
    }
    public static void Invoke(Action action)
    {
        TimerHeap.AddTimer(0, 0, action);
    }
    private void CheckVersion(Action finished)
    {
        //添加一个解压文件界面提示回调
        Action<bool> fileDecompress = (finish) => 
        {
            LOLGameDriver.Invoke(() => 
            {
                EventCenter.Broadcast(EGameEvent.eGameEvent_HideMessage);
                if (finish)
                {
                    CEvent evt = new CEvent(EGameEvent.eGameEvent_ShowMessage);
                    evt.AddParam("content", StringConfigManager.GetString("EMessageType.EMT_UpdateTip.Content2"));
                    EventCenter.SendEvent<EMessageType, Action<bool>>(evt, EMessageType.EMT_Tip, null);
                    Debug.Log("正在更新本地文件");
                }
                else 
                {
                    CEvent evt = new CEvent(EGameEvent.eGameEvent_ShowMessage);
                    evt.AddParam("content", StringConfigManager.GetString("EMessageType.EMT_UpdateTip.Content3"));
                    EventCenter.SendEvent<EMessageType, Action<bool>>(evt, EMessageType.EMT_Tip, null);
                    Debug.Log("数据读取中");
                }
            });
        };
        Action<int, int, string> taskProgress = (total, index, fileName) => 
        {
            LOLGameDriver.Invoke(() => 
            {
                //正在下载更新文件
                Debug.Log(string.Format("正在下载更新文件（{0}/{1}:{2}）", index + 1, total, fileName));
                CEvent evt = new CEvent(EGameEvent.eGameEvent_ShowMessage);
                evt.AddParam("index", index + 1);
                evt.AddParam("total", total);
                evt.AddParam("fileName", fileName);
                EventCenter.SendEvent<EMessageType, Action<bool>>(evt, EMessageType.EMT_UpdateDownload, (isOk) =>
                {
                    if (isOk)
                    {
                        Application.OpenURL("http://www.baidu.com");
                    }
                    else
                    {

                    }
                });
            });
        };
        Action<int, long, long> progress = (ProgressPercentage, TotalBytesToReceive, BytesReceive) => 
        {
            //处理进度条
            Debug.Log(string.Format("进度：{0}%" ,ProgressPercentage));
        };
        Action<Exception> error = (ex) => 
        {
            Debug.Log(ex);
        };
        //界面提示版本检查中
        //EventCenter.Broadcast<EMessageType, Action<bool>>(EGameEvent.eGameEvent_ShowMessage, EMessageType.EMT_UpdateTip,null);
        CEvent cEvent = new CEvent(EGameEvent.eGameEvent_ShowMessage);
        cEvent.AddParam("content", StringConfigManager.GetString("EMessageType.EMT_UpdateTip.Content1"));
        EventCenter.SendEvent<EMessageType, Action<bool>>(cEvent, EMessageType.EMT_Tip, null);
        Debug.Log("版本检查中...");
        VersionManager.singleton.CheckVersion(fileDecompress, taskProgress, progress, finished, error);
    }

    //完成版本检测后的处理
    private void CheckVersionFinished()
    {
        Debug.Log("版本更新完成处理");
        GameStateManager.singleton.EnterDefaultState();
    }
    private void Tick()
    {
        TimerHeap.Tick();
    }
}
