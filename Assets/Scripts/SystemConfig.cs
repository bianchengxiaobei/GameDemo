using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using Utility;
/// <summary>
/// 系统参数配置
/// </summary>
public class SystemConfig
{
    public readonly static string VersionPath = Application.persistentDataPath + "/version.xml";
    public readonly static string ServerVersionPath = Application.persistentDataPath + "/serverVersion.xml";
    public readonly static string CfgPath = Application.persistentDataPath + "/cfg.xml";
    public readonly static string LocalCfgPath = Application.persistentDataPath + "/LocalCfg.xml";
    public readonly static string StringPath = ResourceFolder + "/Config/string.xml";
    public readonly static string LogPath = ResourceFolder + "locallog.txt";
    public static List<CfgInfo> CfgInfoList = new List<CfgInfo>();
    public static List<ServerInfo> ServerList = new List<ServerInfo>();
    private static IXLog m_log = XLog.GetLog<SystemConfig>();
    private static LocalSetting m_oLocalSetting;
    /// <summary>
    /// 资源路径
    /// </summary>
    public static string ResourceFolder 
    {
        get 
        {
            //如果是编辑器下的话，就是Resources下
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return Application.dataPath + "/Resources/";
            }
            //如果是window下的话，因为以后我们会将资源全部打包成ab，导出到一个特定的文件夹
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return Application.dataPath + "/../bin/";
            }
            return null;
        }
    }
    /// <summary>
    /// 本地设置实例
    /// </summary>
    public static LocalSetting LocalSetting 
    {
        get 
        {
            if (m_oLocalSetting == null)
            {
                m_oLocalSetting = new LocalSetting();
            }
            return m_oLocalSetting;
        }
        set 
        {
            m_oLocalSetting = value;
        }
    }
    /// <summary>
    /// 已经选择的服务器id
    /// </summary>
    public static int SelectedServerIndex
    {
        get;
        set;
    }
    public static bool Init()
    {
        try
        {
            LoadCfgInfo();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
        return true;
    }
    /// <summary>
    /// 根据名字取得服务端配置信息Url
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetCfgInfoUrlByName(string name)
    {
        string result = "";
        foreach (var item in CfgInfoList)
        {
            if (item.name == name)
            {
                result = item.url;
                break;
            }
        }
        return result;
    }
    /// <summary>
    /// 根据id取得服务端配置信息Url
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetCfgInfoUrlById(int id)
    {
        string result = "";
        foreach (var item in CfgInfoList)
        {
            if (item.id == id)
            {
                result = item.url;
                break;
            }
        }
        return result;
    }
    /// <summary>
    /// 加载服务配置
    /// </summary>
    /// <returns></returns>
    private static bool LoadCfgInfo()
    {
        string cfgStr = null;
        //如果存在持久路径，就直接加载文本
        if (File.Exists(CfgPath))
        {
            cfgStr = UnityTools.LoadFileText(CfgPath);
        }
        else 
        {
            //从Resources从加载配置文本
            TextAsset cfgUrl = Resources.Load("Cfg") as TextAsset;
            if (cfgUrl)
            {
                //从网页上下载与服务端有关的所有配置xml字符串
                cfgStr = DownloadMgr.Instance.DownLoadHtml(cfgUrl.text);
            }
            else
            {
                cfgStr = null;
            }
        }
        //加载xml内容为列表类
        CfgInfoList = LoadXMLText<CfgInfo>(cfgStr);
        return CfgInfoList != null && CfgInfoList.Count > 0 ? true : false;
    }
    /// <summary>
    /// 加载本地配置
    /// </summary>
    private static void LoadLocalConfig()
    {
        try
        {
            var local = LoadXMLText<LocalSetting>(UnityTools.LoadFileText(LocalCfgPath));
            if (local == null || local.Count == 0)
            { 
                
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    private static void InitConfig()
    {
        if (File.Exists(LocalCfgPath))
        {
            File.Delete(LocalCfgPath);
        }
        
    }
    /// <summary>
    /// 加载服务器列表
    /// </summary>
    public static void LoadServerList()
    {
        try
        {
            List<ServerInfo> servers;
            var url = GetCfgInfoUrlByName("ServerList");
            string xmlSerList = "";
            if (!string.IsNullOrEmpty(url))
            {
                xmlSerList = DownloadMgr.Instance.DownLoadHtml(url);
            }
            servers = LoadXMLText<ServerInfo>(xmlSerList);
            if (servers.Count != 0)
            {
                ServerList = servers;
            }
            for (int i = 0; i < ServerList.Count; i++)
            {
                if (ServerList[i].id == LocalSetting.SelectedServer)
                {
                    SelectedServerIndex = ServerList[i].id;
                    break;
                }
            }
        }
        catch (Exception e)
        {
            m_log.Error(e.ToString());
        }
    }
    public static ServerInfo GetServerInfoById(int id)
    {
        ServerInfo serverInfo = null;
        if (ServerList != null && ServerList.Count > 0)
        {
            foreach (var server in ServerList)
            {
                if (server.id == id)
                {
                     serverInfo = server;
                     break;
                }
            }
        }
        return serverInfo; 
    }
    /// <summary>
    /// 将xml转换成list<T>列表类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xmlText"></param>
    /// <returns></returns>
    private static List<T> LoadXMLText<T>(string xmlText)
    {
        List<T> list = new List<T>();
        try
        {
            if (string.IsNullOrEmpty(xmlText))
            {
                return list;
            }
            Type type = typeof(T);
            XmlDocument doc = XmlResAdapter.GetXmlDocument(xmlText);
            Dictionary<int,Dictionary<string,string>> map = XmlResAdapter.LoadXMLToMap(doc,xmlText);
            var props = type.GetProperties(~System.Reflection.BindingFlags.Static);
            foreach (var item in map)
            {
                var obj = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var prop in props)
                {
                    if (prop.Name == "id")
                    {
                        prop.SetValue(obj,item.Key,null);
                    }
                    else
                    {
                        try
                        {
                            if (item.Value.ContainsKey(prop.Name))
                            {
                                var value = UnityTools.GetValue(item.Value[prop.Name],prop.PropertyType);
                                prop.SetValue(obj,value,null);
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
                list.Add((T)obj);
            }
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
        return list;
    }
}
public class CfgInfo
{
    public int id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
}
public class ServerInfo 
{
    public int id { get; set; }
    public string name { get; set; }
    public int type { get; set; }
    public int flag { get; set; }
    public string text { get; set; }
}
public class LocalSetting
{
    public int id { get; set; }
    public int SelectedServer { get; set; }
    public bool IsSaveAccount { get; set; }
    public float SoundVolume { get; set; }
    public float MusicVolume { get; set; }
    public LocalSetting()
    {
        SoundVolume = 0.5f;
        MusicVolume = 0.5f;
        IsSaveAccount = false;
    }
}
