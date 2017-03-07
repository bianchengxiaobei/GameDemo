using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml;
using Game;
using Game.Common;
using Utility;
public class VersionManager : Singleton<VersionManager>
{
    /// <summary>
    /// 本地版本信息属性
    /// </summary>
    public VersionManagerInfo LocalVersion { get; private set; }
    /// <summary>
    /// 服务版本信息属性
    /// </summary>
    public VersionManagerInfo ServerVersion { get; private set; }
    /// <summary>
    /// 做些事件监听的处理
    /// </summary>
    public void Init()
    {
            
    }
    public void LoadLocalVersion()
    {
        if (File.Exists(SystemConfig.VersionPath))
        {
            string verXml = UnityTools.LoadFileText(SystemConfig.VersionPath);
            LocalVersion = GetVersionInXml(verXml);
            TextAsset programVer = Resources.Load("version") as TextAsset;
            if (programVer && !string.IsNullOrEmpty(programVer.text))
            {
                LocalVersion.ProgramVersionCode = GetVersionInXml(programVer.text).ProgramVersionCode;
            }
        }
        else 
        {
            LocalVersion = new VersionManagerInfo();
            TextAsset ver = Resources.Load("version") as TextAsset;
            if (ver != null)
            {
                UnityTools.SaveText(SystemConfig.VersionPath, ver.text);
            }
        }
    }
    public void CheckVersion(Action<bool> fileDecompress,Action<int,int,string> taskProgress,Action<int,long,long> progress,Action finished,Action<Exception> error)
    {
        BeforeCheck((result) =>
        {
            if (result)
            {
                //需要更新
                Debug.Log("需要更新");
                EventCenter.Broadcast(EGameEvent.eGameEvent_HideMessage);
                CheckAndDownload(fileDecompress, taskProgress, progress, finished, error);
            }
            else
            {
                EventCenter.Broadcast(EGameEvent.eGameEvent_HideMessage);
                //不需要更新
                Debug.Log("不需要更新");
                if (finished != null)
                {
                    finished();
                }
            }
        }, () => { error(new Exception("下载版本文件超时！")); });
    }
    public bool CheckAndDownload(Action<bool> fileDecompress, Action<int, int, string> taskProgress, Action<int, long, long> progress, Action finished, Action<Exception> error)
    {
        //资源需要更新，还有就是程序需要更新我不写了，基本上用不到
        if (ServerVersion.ResourceVersionCodeInfo.Compare(LocalVersion.ResourceVersionCodeInfo) > 0)
        {
            //开始下载资源包
            AsynDownloadUpdatePackage(fileDecompress, taskProgress, progress, finished, error);
            return true;
        }
        if (finished != null)
        {
            finished();
        }
        return false;
    }
    private void AsynDownloadUpdatePackage(Action<bool> fileDecompress, Action<int, int, string> taskProgress, Action<int, long, long> progress, Action finished, Action<Exception> error)
    {
        DownloadPackageInfoList((packageList) =>
        {
            //如果资源包最低版本比游戏的版本高，资源包的最高要求版本比游戏低则需要更新
            var downloadList = (from packageInfo in packageList
                                where packageInfo.LowVersion.Compare(LocalVersion.ResourceVersionCodeInfo) >= 0 && packageInfo.HighVersion.Compare(ServerVersion.ResourceVersionCodeInfo) <= 0
                                select new KeyValuePair<string, string>(packageInfo.HighVersion.ToString(), packageInfo.Name)).ToList();
            string packageUrl = ServerVersion.PackageUrl;
            if (downloadList.Count != 0)
            {
                Debug.Log("开始下载资源包列表");
                DownloadPackageList(fileDecompress, packageUrl, downloadList, taskProgress, progress, finished, error);
            }
            else 
            {
                Debug.Log("更新资源包数目为0");
                if (finished != null)
                {
                    finished();
                }
            }
        }, () => { error(new Exception("下载资源包信息出错")); });
    }
    private void DownloadPackageList(Action<bool> fileDecompress, string packageUrl, List<KeyValuePair<String, String>> downloadList, Action<int, int, string> taskProgress, Action<int, long, long> progress, Action finished, Action<Exception> error)
    {
        //下载列表
        List<DownloadTask> allTasks = new List<DownloadTask>();
        for (int i = 0; i < downloadList.Count; i++)
        {
            KeyValuePair<string, string> kvp = downloadList[i];
            string localFile = string.Concat(SystemConfig.ResourceFolder, kvp.Value);//dataPath+"/Resources/"+文件名
            //下载完成之后回调委托
            Action OnDownloadFinished =()=>
            {
                //进行解压，以后再来
                if (File.Exists(localFile))
                {
                    //开始解压文件
                    FileAccessManager.DecompressFile(localFile);
                }
                if (File.Exists(localFile))
                {
                    File.Delete(localFile);
                }
                //更新本地版本信息
                LocalVersion.ResourceVersionCodeInfo = new VersionCodeInfo(kvp.Key);
                //保存版本信息
                SaveVersion(LocalVersion);
            };
            string fileUrl = string.Concat(packageUrl, kvp.Value);//"http://127.0.0.1/LOLGameDemo/LOLPackage/"+文件名
            //初始化任务
            var task = new DownloadTask
            {
                FileName = localFile,//dataPath+"/Resources/"+文件名
                Url = fileUrl,//下载url
                Finished = OnDownloadFinished,//解压更新版本信息
                Error = error,
                TotalProgress = progress
            };
            string fileNameNoExtension = kvp.Value;
            if (ServerVersion.PackageMd5Dic.ContainsKey(fileNameNoExtension))
            {
                task.MD5 = ServerVersion.PackageMd5Dic[fileNameNoExtension];
                allTasks.Add(task);
            }
            else 
            {
                error(new Exception("下载包不存在：" + fileNameNoExtension));
                return;
            }
        }
        //全部任务下载完成回调
        Action AllFinished = () => 
        {
            Debug.Log("全部下载完成");
            finished();
        };
        //添加taskProgress的回调
        Action<int, int, string> TaskProgress = (total, current, filename) =>
        {
            if (taskProgress != null)
                taskProgress(total, current, filename);
        };
        //添加文件解压的回调函数
        Action<bool> filedecompress = (decompress) =>
        {
            if (fileDecompress != null)
                fileDecompress(decompress);
        };
        DownloadMgr.Instance.Tasks = allTasks;
        DownloadMgr.Instance.AllDownloadFinished = AllFinished;
        DownloadMgr.Instance.TaskProgress = TaskProgress;
        DownloadMgr.Instance.FileDecompress = filedecompress;
        DownloadMgr.Instance.CheckDownloadList();
    }
    /// <summary>
    /// 取得下载包的信息
    /// </summary>
    /// <param name="AsynResult"></param>
    /// <param name="OnError"></param>
    private void DownloadPackageInfoList(Action<List<PackageInfo>> AsynResult, Action OnError)
    {
        //从服务器版本信息中取得下载Url，然后初始化下载包的信息
        DownloadMgr.Instance.AsynDownLoadHtml(ServerVersion.PackageMd5List, 
        (content) => 
        {
            XmlDocument doc = XmlResAdapter.GetXmlDocument(content);
            if (null == doc)
            {
                if (OnError != null)
                {
                    OnError();
                }
            }
            else 
            {
                List<PackageInfo> packagesList = new List<PackageInfo>();
                foreach (XmlNode node in doc.SelectSingleNode("root").ChildNodes)
                {
                    PackageInfo package = new PackageInfo();
                    string packagetName = node.Attributes["name"].Value;
                    package.Name = packagetName;
                    //从第7个开始，也就是说前7个是/名称,这里我取名/LOLPa/，放资源包的文件夹，后4位是索引/001
                    string version = packagetName.Substring(7, packagetName.Length - 11);
                    //中间是低版本和高版本===>比如0.0.0.0-0.0.0.3
                    string firstVersion = version.Substring(0,version.IndexOf("-"));
                    package.LowVersion = new VersionCodeInfo(firstVersion);
                    string endVersion = version.Substring(firstVersion.Length + 1);
                    package.HighVersion = new VersionCodeInfo(endVersion);
                    //然后内容是md5码
                    package.MD5 = node.InnerText;
                    packagesList.Add(package);
                    ServerVersion.PackageMd5Dic.Add(package.Name, package.MD5);
                }
                AsynResult(packagesList);
            }
        }, OnError);
    }
    /// <summary>
    /// 检测网络状况并对照版本信息是否一致
    /// </summary>
    /// <param name="AsynResult">版本信息是否一致的处理委托</param>
    /// <param name="OnError">错误处理委托</param>
    public void BeforeCheck(Action<bool> AsynResult, Action OnError)
    {
        CheckTimeout checkTimeout = new CheckTimeout();
        checkTimeout.AsynIsNetworkTimeout((success) => 
        {
            //如果网络良好，开始下载服务器版本xml
            if (success)
            {
                DownloadMgr.Instance.AsynDownLoadHtml(SystemConfig.GetCfgInfoUrlByName("version"), 
                (serverVersion) => 
                {
                    //如果本地存在服务端的版本信息文本，覆盖下载的服务器文本
                    if (File.Exists(SystemConfig.ServerVersionPath))
                    {
                        serverVersion = UnityTools.LoadFileText(SystemConfig.ServerVersionPath);
                    }
                    //将文本转换成版本信息类
                    ServerVersion = GetVersionInXml(serverVersion);
                    bool programVersion = ServerVersion.ProgramVersionCodeInfo.Compare(LocalVersion.ProgramVersionCodeInfo) > 0;
                    bool resourceVersion = ServerVersion.ResourceVersionCodeInfo.Compare(LocalVersion.ResourceVersionCodeInfo) > 0;
                    //执行是否更新的委托
                    AsynResult(programVersion || resourceVersion);
                },OnError);
            }
            else 
            {
                if (OnError != null)
                {
                    OnError();
                }
            }
        });
    }
    private VersionManagerInfo GetVersionInXml(string verXml)
    {
        XmlDocument doc = XmlResAdapter.GetXmlDocument(verXml);
        if (doc != null && doc.ChildNodes != null && doc.ChildNodes.Count != 0)
        {
            VersionManagerInfo version = new VersionManagerInfo();
            var props = typeof(VersionManagerInfo).GetProperties();
            foreach (XmlNode item in doc.SelectSingleNode("root").ChildNodes)
            {
                var prop = props.FirstOrDefault(t => t.Name == item.Name);
                if (prop != null)
                {
                    prop.SetValue(version, item.InnerText, null);
                }
            }
            return version;
        }
        else 
        {
            return new VersionManagerInfo();
        }
    }
    /// <summary>
    /// 保存版本信息到xml文件中
    /// </summary>
    /// <param name="version"></param>
    private void SaveVersion(VersionManagerInfo version)
    {
        var props = typeof(VersionManagerInfo).GetProperties();
        XmlDocument doc = new XmlDocument();
        XmlDeclaration newChild = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.AppendChild(newChild);
        XmlElement root = doc.CreateElement("root");
        doc.AppendChild(root);
        foreach (var prop in props)
        {
            XmlElement e = doc.CreateElement(prop.Name);
            string value = prop.GetGetMethod().Invoke(version,null) as string;
            e.InnerText = value;
            root.AppendChild(e);
        }
        UnityTools.SaveText(SystemConfig.VersionPath, doc.InnerXml);
    }

}
public class VersionManagerInfo 
{
    /// <summary>
    /// 游戏程序版本号，基本上我们不会替换游戏程序，除非非得重新下载客户端
    /// </summary>
    public VersionCodeInfo ProgramVersionCodeInfo;
    /// <summary>
    /// 游戏资源版本号
    /// </summary>
    public VersionCodeInfo ResourceVersionCodeInfo;
    public string ProgramVersionCode
    {
        get 
        {
            return ProgramVersionCodeInfo.ToString();
        }
        set 
        {
            ProgramVersionCodeInfo = new VersionCodeInfo(value);
        }
    }
    public string ResourceVersionCode 
    {
        get 
        {
            return ResourceVersionCodeInfo.ToString();
        }
        set 
        {
            ResourceVersionCodeInfo = new VersionCodeInfo(value);
        }
    }
    /// <summary>
    /// 资源包列表
    /// </summary>
    public string PackageList { get; set; }
    /// <summary>
    /// 资源包地址
    /// </summary>
    public string PackageUrl { get; set; }
    /// <summary>
    /// 资源包md5码列表
    /// </summary>
    public string PackageMd5List { get; set; }
    /// <summary>
    /// 资源包字典key=>url,value=>md5
    /// </summary>
    public Dictionary<string, string> PackageMd5Dic = new Dictionary<string, string>();
    public VersionManagerInfo()
    {
        ProgramVersionCodeInfo = new VersionCodeInfo("0.0.0.1");
        ResourceVersionCodeInfo = new VersionCodeInfo("0.0.0.0");
        PackageList = string.Empty;
        PackageUrl = string.Empty;
    }
}
/// <summary>
/// 版本号
/// </summary>
public class VersionCodeInfo 
{
    /// <summary>
    /// 版本号列表
    /// </summary>
    private List<int> m_listCodes = new List<int>();
    /// <summary>
    /// 初始化版本号
    /// </summary>
    /// <param name="version"></param>
    public VersionCodeInfo(string version)
    {
        if (string.IsNullOrEmpty(version))
        {
            return;
        }
        string[] versions = version.Split('.');
        for (int i = 0; i < versions.Length; i++)
        {
            int code;
            if (int.TryParse(versions[i], out code))
            {
                this.m_listCodes.Add(code);
            }
            else 
            {
                Debug.LogError("版本号不是数字");
                this.m_listCodes.Add(code);
            }
        }
    }
    /// <summary>
    /// 比较版本号，自己大返回1，自己小返回-1，一样返回0
    /// </summary>
    /// <param name="codeInfo"></param>
    /// <returns></returns>
    public int Compare(VersionCodeInfo codeInfo)
    {
        int count = this.m_listCodes.Count < codeInfo.m_listCodes.Count ? this.m_listCodes.Count : codeInfo.m_listCodes.Count;
        for (int i = 0; i < count; i++)
        {
            if (this.m_listCodes[i] == codeInfo.m_listCodes[i])
            {
                continue;
            }
            else 
            {
                return this.m_listCodes[i] > codeInfo.m_listCodes[i] ? 1 : -1;
            }
        }
        return 0;
    }
    /// <summary>
    /// 重写ToString()方法，输出版本号字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var code in this.m_listCodes)
        {
            sb.AppendFormat("{0}.", code);
        }
        //移除多余出来的.号
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
}
/// <summary>
/// 资源包
/// </summary>
public class PackageInfo 
{
    /// <summary>
    /// 包名
    /// </summary>
    public string Name;
    /// <summary>
    /// 资源适用最低版本号
    /// </summary>
    public VersionCodeInfo LowVersion;
    /// <summary>
    /// 资源适用最高版本号
    /// </summary>
    public VersionCodeInfo HighVersion;
    /// <summary>
    /// 资源md5码
    /// </summary>
    public string MD5;
}
